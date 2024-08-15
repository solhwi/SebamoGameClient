using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopScrollItem : MonoBehaviour
{
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Image backGroundImage = null;
	[SerializeField] private ItemIcon itemIcon = null;
	[SerializeField] private Text itemNameText = null;
	[SerializeField] private Text itemPriceText = null;
	[SerializeField] private string priceTextFormat = "골드 : {0}";

	private ItemTable.ShopItemData myShopItemData;
	private Action<string> onClickBody;
	private Func<string, int, Task> onBuyItem;

	public string ItemCode => myShopItemData?.key ?? string.Empty;

	private Color noSelectedColor = Color.white;
	private Color selectedColor = Color.yellow;

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
		UIManager.Instance.TryOpen(PopupType.Notify, new ShopBuyUIParameter(myShopItemData, onBuyItem));
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
