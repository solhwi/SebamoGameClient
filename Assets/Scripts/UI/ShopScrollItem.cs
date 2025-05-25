
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ShopScrollItem : MonoBehaviour
{
	[SerializeField] private Image backGroundImage = null;
	[SerializeField] private ItemIcon itemIcon = null;
	[SerializeField] private Text itemNameText = null;
	[SerializeField] private Text itemPriceText = null;
	[SerializeField] private string priceTextFormat = "골드 : {0}";

	private ItemTable.ShopItemData myShopItemData;
	private Action<string> onClickBody;
	private Func<string, int, IEnumerator> onBuyItem;

	public string ItemCode => myShopItemData?.key ?? string.Empty;

	private Color noSelectedColor = Color.white;
	private Color selectedColor = Color.yellow;

	public void SetItemData(ItemTable.ShopItemData itemData)
	{
		myShopItemData = itemData;
		itemIcon.SetItemData(itemData);
		itemPriceText.text = string.Format(priceTextFormat, itemData.price);
		itemNameText.text = ItemTable.Instance.GetItemName(itemData.key);
	}

	public void SetItemClickCallback(Action<string> onClick)
	{
		onClickBody = onClick;
	}

	public void SetItemClickCallback(Func<string, int, IEnumerator> onBuy)
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
		UIManager.Instance.TryOpen(PopupType.ItemToolTip, new ShopBuyUIParameter(myShopItemData, onBuyItem));
	}

	public void SetSelect(bool isSelect)
	{
		if (isSelect)
		{
			backGroundImage.color = selectedColor;
		}
		else
		{
			backGroundImage.color = noSelectedColor;
		}
	}
}
