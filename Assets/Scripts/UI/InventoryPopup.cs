using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPopup : BoardGamePopup
{
	public enum TabType
	{
		None = -1,
		Equipment,
		Replace,
		Decorate,
	}

	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Inventory inventory;
	[SerializeField] private RectTransform contentRect;
	[SerializeField] private GridLayoutGroup layoutGroup;
	[SerializeField] private ItemIcon itemIconPrefab;
	[SerializeField] private List<Toggle> toggles = new List<Toggle>();
	[SerializeField] private int defaultItemCount;

	private List<ItemIcon> currentItemIcons = new List<ItemIcon>();
	private TabType currentTabType = TabType.None;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupManager.PopupType.Inventory;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		OnChangedTab(TabType.Equipment);
	}

	protected override void OnClose()
	{
		currentTabType = TabType.None;
	
		base.OnClose();
	}

	private void OnChangedTab(TabType tabType)
	{
		if (currentTabType == tabType)
			return;

		currentTabType = tabType;

		foreach (var itemIcon in currentItemIcons)
		{
			Destroy(itemIcon.gameObject);
		}

		currentItemIcons.Clear();

		var hasItems = GetHasItems(tabType);

		foreach (var itemCode in hasItems)
		{
			var itemIcon = Instantiate(itemIconPrefab, contentRect);
			itemIcon.SetItem(itemCode);

			currentItemIcons.Add(itemIcon);
		}

		ExpandScrollSize(currentItemIcons.Count);
	}

	private void ExpandScrollSize(int hasItemCount)
	{
		int expandCount = hasItemCount - defaultItemCount;
		
		float itemCellSize = layoutGroup.cellSize.x;
		float spacingSize = layoutGroup.spacing.x;

		float expandWidth = expandCount * (itemCellSize + spacingSize);
		contentRect.offsetMax = new Vector2(expandWidth, 0);

		layoutGroup.constraintCount = hasItemCount;
	}

	private IEnumerable<string> GetHasItems(TabType tabType)
	{
		switch(tabType)
		{
			case TabType.Equipment:

				foreach (var iter in inventory.hasItems)
				{
					var itemCode = iter.Key;
					if (itemTable.IsEquipmentItem(itemCode))
					{
						yield return itemCode;
					}
				}
				break;
		}
	}

	public void OnValueChangedIndex()
	{
		int index = toggles.FindIndex(t => t.isOn);
		OnChangedTab((TabType)index);
	}
}
