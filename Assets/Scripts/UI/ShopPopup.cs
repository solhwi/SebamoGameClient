using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MathFloat
{
	public static bool IsEqual(float x, float y)
	{
		if (x <= y + Mathf.Epsilon &&
			x >= y - Mathf.Epsilon)
		{
			return true;
		}

		return false;
	}
}

public class ShopPopup : BoardGamePopup
{
	public enum TabType
	{
		None = -1,
		Random = 0,
		Normal = 1,
	}

	[SerializeField] private Inventory inventory;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private ScrollContent scrollContent;
	[SerializeField] private Text npcText;
	[SerializeField] private Text coinText;

	private List<ShopScrollItem> shopItems = new List<ShopScrollItem>();
	private int normalItemIndex = 0;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupManager.PopupType.Shop;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		normalItemIndex = itemTable.sortedShopItemList.FindIndex(i => i.isRandom == 0);

		scrollContent.onChangedTab += OnChangedTab;
		scrollContent.onUpdateContents += OnUpdateContents;
		scrollContent.onGetItemCount += GetItemCount;

		scrollContent.SelectTab((int)TabType.Random);
	}

	protected override void OnClose()
	{
		scrollContent.onChangedTab -= OnChangedTab;
		scrollContent.onUpdateContents -= OnUpdateContents;
		scrollContent.onGetItemCount -= GetItemCount;

		base.OnClose();
	}

	public void OnChangedTab(int tabType)
	{
		FocusItemByTab((TabType)tabType);

		shopItems.Clear();
	}

	public void OnUpdateContents(int index, GameObject contentsObj)
	{
		if (index < 0 || itemTable.sortedShopItemList.Count <= index)
			return;

		if (contentsObj == null)
			return;

		var shopScrollItem = contentsObj.GetComponent<ShopScrollItem>();
		if (shopScrollItem == null)
			return;

		var shopItemData = itemTable.sortedShopItemList[index];
		shopScrollItem.SetItemData(shopItemData);
		shopScrollItem.SetItemClickCallback(OnClickItem);
		shopScrollItem.SetItemClickCallback(OnBuyItem);
		shopScrollItem.SetSelect(false);

		shopItems.Add(shopScrollItem);
	}

	private void OnClickItem(string itemCode)
	{
		npcText.text = itemTable.GetItemNPCDescription(itemCode);

		foreach (var shopItem in shopItems)
		{
			shopItem.SetSelect(shopItem.ItemCode == itemCode);
		}
	}

	public int GetItemCount(int tabType)
	{
		return itemTable.sortedShopItemList.Count;
	}

	private void Update()
	{
		if (normalItemIndex >= scrollContent.DefaultItemCount)
		{
			if (scrollContent.IsTargeting == false)
			{
				float itemSize = scrollContent.GetItemCellSizeY();
				float normalItemPos = scrollContent.GetItemPos(normalItemIndex);
				float currentYPos = scrollContent.GetCurrentYPos();

				if (currentYPos > normalItemPos)
				{
					scrollContent.SelectTab((int)TabType.Normal);
				}
				else if (currentYPos < itemSize)
				{
					scrollContent.SelectTab((int)TabType.Random);
				}
			}
		}
	}

	private void FocusItemByTab(TabType tabType)
	{
		switch(tabType)
		{
			case TabType.Random:
				scrollContent.StartFocusTarget(0);
				break;

			case TabType.Normal:
				scrollContent.StartFocusTarget(normalItemIndex);
				break;
		}
	}

	private async Task OnBuyItem(string itemCode, int buyCount)
	{
		int price = itemTable.GetItemBuyPrice(itemCode);

		bool isSuccess = await inventory.TryRemoveItem(ItemTable.Coin, price * buyCount);
		if (isSuccess)
		{
			inventory.TryAddItem(itemCode, buyCount).Wait();
		}
	}
}
