
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryPopup : BoardGamePopup
{
	public class Parameter : UIParameter
	{
		public readonly TabType tabType;

		public Parameter(TabType tabType)
		{
			this.tabType = tabType;
		}
	}

	public enum TabType
	{
		None = -1,
		Props,
		Parts,
		Profile,
		Replace,
	}

	
	

	

	[SerializeField] private CharacterView uiCharacterView;
	[SerializeField] private ItemObjectView itemObjectView;

	[SerializeField] private ProfileSetter profileSetter;

	[SerializeField] private ScrollContent scrollContent;

	[SerializeField] private GameObject useButtonObj;
	[SerializeField] private GameObject cameraButtonObj;

	[SerializeField] private TogglePanel equipmentPanel;
	[SerializeField] private TogglePanel profilePanel;

	[SerializeField] private EquipmentBoard equipmentBoard;
	[SerializeField] private EquipmentBoard profileEquipmentBoard;

	private List<KeyValuePair<string, int>> hasItemList = new List<KeyValuePair<string, int>>();
	private TabType currentTabType;

	private bool isToggleOn = true;

	private string currentItemCode = null;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Inventory;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		var myCharacter = BoardGameManager.Instance.GetMyPlayerCharacter();
		if (myCharacter != null)
		{
			myCharacter.gameObject.SetActive(false);
		}

		uiCharacterView.Initialize();
		itemObjectView.Initialize();

		currentItemCode = string.Empty;

		scrollContent.onChangedTab += OnChangedTab;
		scrollContent.onUpdateContents += OnUpdateContents;
		scrollContent.onGetItemCount += GetHasItemCount;

		equipmentPanel.onToggle += OnClickPanelToggle;
		profilePanel.onToggle += OnClickPanelToggle;

		equipmentBoard.onClickItem += OnClickEquippedItem;
		profileEquipmentBoard.onClickItem += OnClickProfileItem;

		if (parameter is Parameter p)
		{
			scrollContent.SelectTab((int)p.tabType);
		}
		else
		{
			scrollContent.SelectTab((int)TabType.Props);
		}
	}

	protected override void OnClose()
	{
		scrollContent.onChangedTab -= OnChangedTab;
		scrollContent.onUpdateContents -= OnUpdateContents;
		scrollContent.onGetItemCount -= GetHasItemCount;

		equipmentPanel.onToggle -= OnClickPanelToggle;
		profilePanel.onToggle -= OnClickPanelToggle;

		equipmentBoard.onClickItem -= OnClickEquippedItem;
		profileEquipmentBoard.onClickItem -= OnClickProfileItem;

		var myCharacter = BoardGameManager.Instance.GetMyPlayerCharacter();
		if (myCharacter != null)
		{
			myCharacter.gameObject.SetActive(true);
			myCharacter.Refresh();
		}

		scrollContent.SelectTab((int)TabType.None);

		itemObjectView.UnsetItem();

		base.OnClose();
	}

	private void OnChangedTab(int tabType)
	{
		currentTabType = (TabType)tabType;

		hasItemList = GetSortingHasItems(currentTabType).ToList();

		if (currentTabType == TabType.Replace)
		{
			currentItemCode = hasItemList.FirstOrDefault().Key;
			itemObjectView.SetItem(currentItemCode);

			hasItemList = GetSortingHasItems(currentTabType).ToList();
		}
		else
		{
			currentItemCode = string.Empty;
		}

		useButtonObj.SetActive(currentTabType == TabType.Replace);

		cameraButtonObj.SetActive(currentTabType == TabType.Props || currentTabType == TabType.Parts);

		equipmentPanel.gameObject.SetActive(currentTabType == TabType.Props || currentTabType == TabType.Parts);
		profilePanel.gameObject.SetActive(currentTabType == TabType.Profile);

		equipmentPanel.SetToggle(isToggleOn);
		profilePanel.SetToggle(isToggleOn);

		if (currentTabType == TabType.Props || currentTabType == TabType.Parts)
		{
			uiCharacterView.gameObject.SetActive(true);
			profileSetter.gameObject.SetActive(false);
			itemObjectView.gameObject.SetActive(false);
		}
		else if (currentTabType == TabType.Profile)
		{
			uiCharacterView.gameObject.SetActive(false);
			profileSetter.gameObject.SetActive(true);
			itemObjectView.gameObject.SetActive(false);
		}
		else if (currentTabType == TabType.Replace)
		{
			uiCharacterView.gameObject.SetActive(false);
			profileSetter.gameObject.SetActive(false);
			itemObjectView.gameObject.SetActive(true);
		}

		scrollContent.Reset();
	}

	private void OnClickPanelToggle(bool isOn)
	{
		isToggleOn = isOn;
	}

	private void OnUpdateContents(int index, GameObject contentObj)
	{
		if (index < 0 || hasItemList.Count <= index)
			return;

		if (contentObj == null)
			return;

		var scrollItem = contentObj.GetComponent<InventoryScrollItem>();
		if (scrollItem == null)
			return;

		var hasItemData = hasItemList[index];

		string itemCode = hasItemData.Key;
		int itemCount = hasItemData.Value;

		scrollItem.SetItemData(itemCode, itemCount);
		scrollItem.SetItemClickCallback(OnClickItem);
		scrollItem.SetItemPressCallback(OnPressItem);
	}

	private void OnClickItem(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return;

		if (currentTabType == TabType.Replace)
		{
			currentItemCode = itemCode;
			itemObjectView.SetItem(currentItemCode);

			hasItemList = GetSortingHasItems(currentTabType).ToList();
			scrollContent.UpdateContents();
		}
		else
		{
			StartCoroutine(Inventory.Instance.TryEquipOn(itemCode, (d) =>
			{
				if (ItemTable.Instance.IsEquipmentItem(itemCode))
				{
					uiCharacterView.RefreshCharacter();
					uiCharacterView.DoIdle(0.3f);
				}

				hasItemList = GetSortingHasItems(currentTabType).ToList();
				scrollContent.UpdateContents();
			}));
		}
	}

	private void OnPressItem(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return;

		UIManager.Instance.TryOpen(PopupType.ItemToolTip, new SellUIParameter(itemCode, OnClickSell));
	}

	private IEnumerator OnClickSell(string itemCode, int count)
	{
		bool isSuccess = Inventory.Instance.TryRemoveItem(itemCode, count);
		if (isSuccess)
		{
			int price = ItemTable.Instance.GetItemSellPrice(itemCode);
			Inventory.Instance.TryAddItem(ItemTable.Coin, price * count);

			hasItemList = GetSortingHasItems(currentTabType).ToList();
			scrollContent.UpdateContents();
		}

		yield return HttpNetworkManager.Instance.TryPostMyPlayerData(null);
	}

	private void OnClickEquippedItem(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return;

		if (ItemTable.Instance.IsEnableEquipOffItem(itemCode) == false)
		{
			Debug.Log($"해당 아이템 [{itemCode}]는 벗을 수 없는 아이템입니다.");
		}
		else
		{
			StartCoroutine(Inventory.Instance.TryEquipOff(itemCode, (d) =>
			{
				uiCharacterView.RefreshCharacter();
				uiCharacterView.DoIdle(0.3f);

				hasItemList = GetSortingHasItems(currentTabType).ToList();
				scrollContent.UpdateContents();
			}));
		}
	}

	private void OnClickProfileItem(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return;

		Debug.Log($"해당 아이템 [{itemCode}]는 벗을 수 없는 아이템입니다.");
	}

	public void OnClickUseItem()
	{
		if (ItemTable.Instance.IsBuffItem(currentItemCode))
		{
			StartCoroutine(Inventory.Instance.TryApplyBuff(currentItemCode, (d) =>
			{
				UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter($"버프 아이템 {currentItemCode}이 사용되었습니다."));

				int currentItemCount = Inventory.Instance.GetHasCount(currentItemCode);
				if (currentItemCount <= 0)
				{
					hasItemList = GetSortingHasItems(currentTabType).ToList();
					scrollContent.UpdateContents();

					currentItemCode = hasItemList.FirstOrDefault().Key;
					itemObjectView.SetItem(currentItemCode);
				}
			}));
		}
		else if (ItemTable.Instance.IsFieldItem(currentItemCode) || ItemTable.Instance.IsDeBuffItem(currentItemCode))
		{
			BoardGameManager.Instance.StartReplaceMode(currentItemCode);
		}
	}

	private int GetHasItemCount(int tabType)
	{
		return GetHasItems((TabType)tabType).Count();
	}

	private IEnumerable<KeyValuePair<string, int>> GetSortingHasItems(TabType tabType)
	{
		var hasItems = GetHasItems(tabType).ToList();

		if (ItemTable.Instance != null)
		{
			hasItems.Sort((a, b) => ItemTable.Instance.Compare(a.Key, b.Key));
		}

		return hasItems;
	}

	private IEnumerable<KeyValuePair<string, int>> GetHasItems(TabType tabType)
	{
		switch(tabType)
		{
			case TabType.Props:

				foreach (var iterator in Inventory.Instance.hasItems)
				{
					var itemCode = iterator.Key;

					if (Inventory.Instance.IsEquippedItem(itemCode))
						continue;

					if (ItemTable.Instance.IsAvatarItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;

			case TabType.Parts:

				foreach (var iterator in Inventory.Instance.hasItems)
				{
					var itemCode = iterator.Key;
					if (Inventory.Instance.IsEquippedItem(itemCode))
						continue;

					if (ItemTable.Instance.IsBeautyItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;

			case TabType.Profile:

				foreach (var iterator in Inventory.Instance.hasItems)
				{
					var itemCode = iterator.Key;
					if (Inventory.Instance.IsEquippedItem(itemCode))
						continue;

					if (ItemTable.Instance.IsProfileItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;

			case TabType.Replace:

				foreach (var iterator in Inventory.Instance.hasItems)
				{
					var itemCode = iterator.Key;
					if (currentItemCode == itemCode)
						continue;

					if (ItemTable.Instance.IsFieldItem(itemCode) || ItemTable.Instance.IsBuffItem(itemCode) || ItemTable.Instance.IsDeBuffItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;
		}
	}
}
