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
		Props,
		Parts,
		Decorate,
		Replace,
	}

	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Inventory inventory;

	[SerializeField] private ScrollContent scrollContent;

	private List<KeyValuePair<string, int>> hasItemList = new List<KeyValuePair<string, int>>();

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
		scrollContent.onGetItemCount += GetHasItemCount;

		scrollContent.SelectTab((int)TabType.Props);
	}

	protected override void OnClose()
	{
		scrollContent.onChangedTab -= OnChangedTab;
		scrollContent.onUpdateContents -= OnUpdateContents;
		scrollContent.onGetItemCount -= GetHasItemCount;

		base.OnClose();
	}

	private void OnChangedTab(int tabType)
	{
		hasItemList = GetHasItems((TabType)tabType).ToList();

		scrollContent.Reset();
	}

	private void OnUpdateContents(int index, GameObject contentObj)
	{
		if (index < 0 || hasItemList.Count <= index)
			return;

		if (contentObj == null)
			return;

		var itemIcon = contentObj.GetComponent<ItemIcon>();
		if (itemIcon == null)
			return;

		var hasItemData = hasItemList[index];
		itemIcon.SetItemData(hasItemData.Key, hasItemData.Value);
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
