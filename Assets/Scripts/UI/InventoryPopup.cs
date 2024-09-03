using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSortingComparer : IComparer<string>
{
	private ItemTable itemTable;

	public ItemSortingComparer(ItemTable itemTable)
	{
		this.itemTable = itemTable;
	}

	public int Compare(string itemCode1, string itemCode2)
	{
		int order1 = -1;
		int order2 = -1;

		if (itemTable.propItemDataDictionary.ContainsKey(itemCode1))
		{
			order1 = (int)CharacterPartsType.Max;
		}
		else if (itemTable.partsItemDataDictionary.TryGetValue(itemCode1, out var itemData))
		{
			order1 = (int)itemData.partsType;
		}
		else if(itemTable.profileItemDataDictionary.TryGetValue(itemCode1, out var profileItemData))
		{
			order1 = profileItemData.isFrame + (int)CharacterPartsType.Max;
		}
		else if (itemTable.fieldItemDataDictionary.TryGetValue(itemCode1, out var fieldItemData))
		{
			order1 = (int)CharacterPartsType.Max + 2;
		}
		else if (itemTable.buffItemDataDictionary.TryGetValue(itemCode1, out var buffItemData))
		{
			order1 = (int)CharacterPartsType.Max + 3;
		}

		if (itemTable.propItemDataDictionary.ContainsKey(itemCode2))
		{
			order2 = (int)CharacterPartsType.Max;
		}
		else if (itemTable.partsItemDataDictionary.TryGetValue(itemCode2, out var itemData))
		{
			order2 = (int)itemData.partsType;
		}
		else if (itemTable.profileItemDataDictionary.TryGetValue(itemCode2, out var profileItemData))
		{
			order2 = profileItemData.isFrame + (int)CharacterPartsType.Max;
		}
		else if (itemTable.fieldItemDataDictionary.TryGetValue(itemCode2, out var fieldItemData))
		{
			order2 = (int)CharacterPartsType.Max + 2;
		}
		else if (itemTable.buffItemDataDictionary.TryGetValue(itemCode2, out var buffItemData))
		{
			order2 = (int)CharacterPartsType.Max + 3;
		}

		return order1 >= order2 ? 1 : -1;
	}

}

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

	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Inventory inventory;
	[SerializeField] private FieldItemFactory fieldItemFactory;

	[SerializeField] private CharacterView uiCharacterView;
	[SerializeField] private CharacterView gameCharacterView;
	[SerializeField] private ItemObjectView itemObjectView;

	[SerializeField] private ProfileSetter profileSetter;

	[SerializeField] private ScrollContent scrollContent;

	[SerializeField] private GameObject useButtonObj;
	[SerializeField] private GameObject cameraButtonObj;

	[SerializeField] private TogglePanel equipmentPanel;
	[SerializeField] private TogglePanel profilePanel;

	[SerializeField] private EquipmentBoard equipmentBoard;
	[SerializeField] private EquipmentBoard profileEquipmentBoard;

	private ItemSortingComparer sortingComparer = null;

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

		currentItemCode = string.Empty;
		sortingComparer = new ItemSortingComparer(itemTable);

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

		gameCharacterView.RefreshCharacter();
		gameCharacterView.Replay();

		scrollContent.SelectTab((int)TabType.None);

		itemObjectView.UnsetItem();

		base.OnClose();
	}

	private void OnChangedTab(int tabType)
	{
		currentTabType = (TabType)tabType;

		hasItemList = GetHasItems(currentTabType).OrderByDescending(p => p.Key, sortingComparer).ToList();

		if (currentTabType == TabType.Replace)
		{
			currentItemCode = hasItemList.FirstOrDefault().Key;
			itemObjectView.SetItem(currentItemCode);

			hasItemList = GetHasItems(currentTabType).OrderByDescending(p => p.Key, sortingComparer).ToList();
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
		}
		else
		{
			inventory.TryEquipOn(itemCode).Wait();

			if (itemTable.IsEquipmentItem(itemCode))
			{
				uiCharacterView.RefreshCharacter();
				uiCharacterView.DoIdle(0.3f);
			}
		}

		hasItemList = GetHasItems(currentTabType).OrderByDescending(p => p.Key, sortingComparer).ToList();
		scrollContent.UpdateContents();
	}

	private void OnPressItem(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return;

		UIManager.Instance.TryOpen(PopupType.ItemToolTip, new SellUIParameter(itemCode, OnClickSell));
	}

	private async Task OnClickSell(string itemCode, int count)
	{
		bool isSuccess = inventory.TryRemoveItem(itemCode, count);
		if (isSuccess)
		{
			int price = itemTable.GetItemSellPrice(itemCode);
			inventory.TryAddItem(ItemTable.Coin, price * count);

			hasItemList = GetHasItems(currentTabType).OrderByDescending(p => p.Key, sortingComparer).ToList();
			scrollContent.UpdateContents();
		}

		await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	private void OnClickEquippedItem(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return;

		if (itemTable.IsEnableEquipOffItem(itemCode) == false)
		{
			Debug.Log($"해당 아이템 [{itemCode}]는 벗을 수 없는 아이템입니다.");
		}
		else
		{
			inventory.TryEquipOff(itemCode).Wait();

			uiCharacterView.RefreshCharacter();
			uiCharacterView.DoIdle(0.3f);

			hasItemList = GetHasItems(currentTabType).OrderByDescending(p => p.Key, sortingComparer).ToList();
			scrollContent.UpdateContents();
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
		if (itemTable.IsFieldItem(currentItemCode))
		{
			BoardGameManager.Instance.StartReplaceMode(currentItemCode);
		}
		else if (itemTable.IsBuffItem(currentItemCode))
		{
			UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter($"버프 아이템 {currentItemCode}이 사용되었습니다.", 
			onClickConfirm: () =>
			{
				inventory.TryApplyBuff(currentItemCode).Wait();
			}));
		}
	}

	private int GetHasItemCount(int tabType)
	{
		return GetHasItems((TabType)tabType).Count();
	}

	private IEnumerable<KeyValuePair<string, int>> GetHasItems(TabType tabType)
	{
		switch(tabType)
		{
			case TabType.Props:

				foreach (var iterator in inventory.hasItems)
				{
					var itemCode = iterator.Key;

					if (inventory.IsEquippedItem(itemCode))
						continue;

					if (itemTable.IsAvatarItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;

			case TabType.Parts:

				foreach (var iterator in inventory.hasItems)
				{
					var itemCode = iterator.Key;
					if (inventory.IsEquippedItem(itemCode))
						continue;

					if (itemTable.IsBeautyItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;

			case TabType.Profile:

				foreach (var iterator in inventory.hasItems)
				{
					var itemCode = iterator.Key;
					if (inventory.IsEquippedItem(itemCode))
						continue;

					if (itemTable.IsProfileItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;

			case TabType.Replace:

				foreach (var iterator in inventory.hasItems)
				{
					var itemCode = iterator.Key;
					if (currentItemCode == itemCode)
						continue;

					if (itemTable.IsFieldItem(itemCode) || itemTable.IsBuffItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;
		}
	}
}
