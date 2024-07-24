using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScrollItem : MonoBehaviour
{
	public class Parameter : UIParameter
	{
		public readonly ItemTable.ShopItemData shopItemData = null;
		
		public Parameter(ItemTable.ShopItemData shopItemData)
		{
			this.shopItemData = shopItemData;
		}
	}

	[SerializeField] private ItemTable itemTable;
	[SerializeField] private ItemIcon itemIcon = null;
	[SerializeField] private Text itemNameText = null;
	[SerializeField] private Text itemPriceText = null;
	[SerializeField] private string priceTextFormat = "골드 : {0}";

	private ItemTable.ShopItemData myShopItemData;

	public void SetItemData(ItemTable.ShopItemData itemData)
	{
		myShopItemData = itemData;

		itemIcon.SetItemData(itemData);

		itemPriceText.text = string.Format(priceTextFormat, itemData.price);
		 
		if (itemTable.itemIconDataDictionary.TryGetValue(itemData.key, out var iconData))
		{
			itemNameText.text = iconData.itemName;
		}
	}

	public void OnClickBuy()
	{
		PopupManager.Instance.TryOpen(PopupManager.PopupType.Notify, new Parameter(myShopItemData));
	}
}
