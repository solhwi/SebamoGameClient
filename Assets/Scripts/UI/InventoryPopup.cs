using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPopup : BoardGamePopup
{
	public enum TabType
	{
		None = -1,
		Props,
		Parts,
		Decorate,
		Replace,
	}

	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Inventory inventory;
	[SerializeField] private CharacterView uiCharacterView;
	[SerializeField] private CharacterView gameCharacterView;
	[SerializeField] private GameObject useButtonObj;
	[SerializeField] private ScrollContent scrollContent;

	private Dictionary<string, InventoryScrollItem> scrollItemDictionary = new Dictionary<string, InventoryScrollItem>();
	private List<KeyValuePair<string, int>> hasItemList = new List<KeyValuePair<string, int>>();

	private string currentSelectedItemCode = string.Empty;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupManager.PopupType.Inventory;
	}
	
	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		scrollContent.onChangedTab += OnChangedTab;
		scrollContent.onUpdateContents += OnUpdateContents;
		scrollContent.onRefreshContents += RefreshContents;
		scrollContent.onGetItemCount += GetHasItemCount;

		scrollContent.SelectTab((int)TabType.Props);
	}

	protected override void OnClose()
	{
		scrollContent.onChangedTab -= OnChangedTab;
		scrollContent.onUpdateContents -= OnUpdateContents;
		scrollContent.onRefreshContents -= RefreshContents;
		scrollContent.onGetItemCount -= GetHasItemCount;

		gameCharacterView.RefreshCharacter();

		base.OnClose();
	}

	private void OnChangedTab(int tabType)
	{
		TabType currentTabType = (TabType)tabType;
		hasItemList = GetHasItems(currentTabType).ToList();

		useButtonObj.SetActive(currentTabType == TabType.Replace);

		scrollContent.Reset();
		scrollItemDictionary.Clear();
	}

	private void RefreshContents()
	{
		int focusIndex = GetEquippedItemIndex();
		scrollContent.StartFocusTarget(focusIndex);
	}

	private int GetEquippedItemIndex()
	{
		for(int i = 0; i < hasItemList.Count; i++)
		{
			string itemCode = hasItemList[i].Key;
			if (inventory.IsEquippedItem(itemCode))
			{
				return i;
			}
		}

		return 0;
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

		bool isEquipped = inventory.IsEquippedItem(itemCode);
		scrollItem.SetSelect(isEquipped);

		scrollItemDictionary[itemCode] = scrollItem;
	}

	private void OnClickItem(string itemCode)
	{
		if (currentSelectedItemCode == itemCode)
			return;

		foreach (var iter in scrollItemDictionary)
		{
			if (iter.Key == itemCode)
			{
				currentSelectedItemCode = itemCode;

				scrollItemDictionary[itemCode].SetSelect(true);
				TryEquipItem(itemCode).Wait();
			}
			else
			{
				scrollItemDictionary[iter.Key].SetSelect(false);
			}
		}
	}

	private async Task TryEquipItem(string itemCode)
	{
		await inventory.TryEquipOn(itemCode);
		uiCharacterView.RefreshCharacter();
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
					if (itemTable.IsPropItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;

			case TabType.Parts:

				foreach (var iterator in inventory.hasItems)
				{
					var itemCode = iterator.Key;
					if (itemTable.IsPartsItem(itemCode))
					{
						yield return iterator;
					}
				}

				break;
		}
	}
}
