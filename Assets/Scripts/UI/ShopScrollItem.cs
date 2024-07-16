using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScrollItem : MonoBehaviour
{
	[SerializeField] private Text itemCodeText = null;

    public void SetItemData(ItemTable.ShopItemData itemData)
	{
		itemCodeText.text = itemData.key;
	}
}
