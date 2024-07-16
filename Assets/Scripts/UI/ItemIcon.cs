using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
	[SerializeField] private Image itemImage;

	public void SetItemData(ItemTable.ShopItemData itemData)
	{
		string path = itemData.GetAssetPathWithoutResources();
		var sprite = ResourceManager.Instance.Load<Sprite>(path);
		itemImage.sprite = sprite;
	}

    public void SetItemData(string itemCode, int itemCount = 1)
	{
		
	}
}
