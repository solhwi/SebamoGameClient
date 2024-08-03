using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopScrollItem : MonoBehaviour
{
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private ItemIcon itemIcon = null;
	[SerializeField] private Text itemNameText = null;
	[SerializeField] private Text itemPriceText = null;
	[SerializeField] private string priceTextFormat = "골드 : {0}";

	private ItemTable.ShopItemData myShopItemData;
	private Action<string> onClickBody;
	private Func<string, int, Task> onBuyItem;

	public void SetItemData(ItemTable.ShopItemData itemData)
	{
		myShopItemData = itemData;
		itemIcon.SetItemData(itemData);
		itemPriceText.text = string.Format(priceTextFormat, itemData.price);
		itemNameText.text = itemTable.GetItemName(itemData.key);
	}

	public void SetItemClickCallback(Action<string> onClick)
	{
		onClickBody = onClick;
	}

	public void SetItemClickCallback(Func<string, int, Task> onBuy)
	{
		onBuyItem = onBuy;
	}

	public void OnClickBody()
	{
		if (myShopItemData == null)
			return;

		onClickBody?.Invoke(myShopItemData.key);
	}

	public void OnClickBuy()
	{
		PopupManager.Instance.TryOpen(PopupManager.PopupType.Notify, new ShopBuyUIParameter(myShopItemData, onBuyItem));
	}
}
