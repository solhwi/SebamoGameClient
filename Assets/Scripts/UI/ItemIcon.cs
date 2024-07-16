using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Image itemImage;

	public void SetItemData(ItemTable.ShopItemData itemData)
	{
		string itemCode = itemData.key;

		if (itemTable.itemIconDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			var sprite = ResourceManager.Instance.Load<Sprite>(iconData.GetAssetPathWithoutResources());
			itemImage.sprite = sprite;
		}
	}

    public void SetItemData(string itemCode, int itemCount = 1)
	{
		
	}
}
