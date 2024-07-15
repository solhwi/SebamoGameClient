using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
	[SerializeField] private Text itemCodeText = null;

    public void SetItem(ItemTable.ShopItemData itemData)
	{
		itemCodeText.text = itemData.key;
	}
}
