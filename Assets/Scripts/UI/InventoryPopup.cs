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

		if (itemTable.propItemDataDictionary.ContainsKey(itemCode2))
		{
			order2 = (int)CharacterPartsType.Max;
		}
		else if (itemTable.partsItemDataDictionary.TryGetValue(itemCode2, out var itemData))
		{
			order2 = (int)itemData.partsType;
		}

		return order1 >= order2 ? 1 : -1;
	}

}


public class InventoryPopup : BoardGamePopup
{
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
	[SerializeField] private CharacterView uiCharacterView;
	[SerializeField] private CharacterView gameCharacterView;
	[SerializeField] private ScrollContent scrollContent;
	[SerializeField] private List<ItemIcon> equippedItemIcons = new List<ItemIcon>();

	[SerializeField] private GameObject useButtonObj;

	[SerializeField] private TogglePanel equipmentPanel;
	[SerializeField] private TogglePanel profilePanel;

	private ItemSortingComparer sortingComparer = null;

	private List<KeyValuePair<string, int>> hasItemList = new List<KeyValuePair<string, int>>();
	private TabType currentTabType;

	private bool isToggleOn = true;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupManager.PopupType.Inventory;
	}
	
	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		sortingComparer = new ItemSortingComparer(itemTable);

		scrollContent.onChangedTab += OnChangedTab;
		scrollContent.onUpdateContents += OnUpdateContents;
		scrollContent.onGetItemCount += GetHasItemCount;

		equipmentPanel.onToggle += OnClickPanelToggle;
		profilePanel.onToggle += OnClickPanelToggle;

		scrollContent.SelectTab((int)TabType.Props);
	}

	protected override void OnClose()
	{
		scrollContent.onChangedTab -= OnChangedTab;
		scrollContent.onUpdateContents -= OnUpdateContents;
		scrollContent.onGetItemCount -= GetHasItemCount;

		equipmentPanel.onToggle -= OnClickPanelToggle;
		profilePanel.onToggle -= OnClickPanelToggle;

		gameCharacterView.RefreshCharacter();

		base.OnClose();
	}

	private void OnChangedTab(int tabType)
	{
		currentTabType = (TabType)tabType;
		hasItemList = GetHasItems(currentTabType).OrderByDescending(p => p.Key, sortingComparer).ToList();

		useButtonObj.SetActive(currentTabType == TabType.Replace);
		
		equipmentPanel.gameObject.SetActive(currentTabType == TabType.Props || currentTabType == TabType.Parts);
		profilePanel.gameObject.SetActive(currentTabType == TabType.Profile);

		equipmentPanel.SetToggle(isToggleOn);
		profilePanel.SetToggle(isToggleOn);

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
	}

	private void OnClickItem(string itemCode)
	{
		TryEquipItem(itemCode).Wait();

		hasItemList = GetHasItems(currentTabType).OrderByDescending(p => p.Key, sortingComparer).ToList();
		scrollContent.UpdateContents();
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
		}
	}
}
