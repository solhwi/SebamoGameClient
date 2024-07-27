using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScrollItem : MonoBehaviour
{
	[SerializeField] private ItemIcon itemIcon;
	[SerializeField] private Image backgroundImage;

	private Action<string> onClickItem;

	private Color noSelectedColor = Color.white;
	private Color selectedColor = Color.yellow;

	public void SetItemData(string itemCode, int itemCount = 1)
	{
		itemIcon.SetItemData(itemCode, itemCount);
		itemIcon.SetItemClickCallback(OnClickItem);
	}

	public void SetItemClickCallback(Action<string> callback)
	{
		onClickItem = callback;
	}

	private void OnClickItem(string itemCode)
	{
		onClickItem?.Invoke(itemCode);
	}

	public void SetSelect(bool isSelect)
	{
		if (isSelect)
		{
			backgroundImage.color = selectedColor;
		}
		else
		{
			backgroundImage.color = noSelectedColor;
		}
	}
}
