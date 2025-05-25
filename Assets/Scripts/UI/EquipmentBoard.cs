using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentBoard : MonoBehaviour
{
	public enum BoardType
	{
		Equipment,
		Profile,
	}

	[SerializeField] private BoardType boardType;

	[SerializeField] private List<ItemIcon> equippedItemIcons = new List<ItemIcon>();

	public event Action<string> onClickItem;

	private void Update()
	{
		Refresh();
	}

	public void Refresh()
	{
		switch (boardType)
		{
			case BoardType.Equipment:

				for (int i = 0; i < equippedItemIcons.Count; i++)
				{
					string itemCode = Inventory.Instance.equippedItems[i];
					equippedItemIcons[i].SetItemData(itemCode);
					equippedItemIcons[i].SetItemClickCallback(onClickItem);
				}

				break;

			case BoardType.Profile:

				for (int i = 0; i < equippedItemIcons.Count; i++)
				{
					string itemCode = Inventory.Instance.appliedProfileItems[i];
					equippedItemIcons[i].SetItemData(itemCode);
					equippedItemIcons[i].SetItemClickCallback(onClickItem);
				}

				break;
		}
	}
}
