using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScrollItem : MonoBehaviour
{
	[SerializeField] private ItemIcon itemIcon;

	public void SetItemData(string itemCode, int itemCount = 1)
	{
		itemIcon.SetItemData(itemCode, itemCount);
	}

	public void SetItemClickCallback(Action<string> callback)
	{
		itemIcon.SetItemClickCallback(callback);
	}

	public void SetItemPressCallback(Action<string> callback)
	{
		itemIcon.SetItemPressCallback(callback);
	}
}
