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

	[SerializeField] private Inventory inventory;
	[SerializeField] private ItemTable itemTable;

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
					string itemCode = inventory.equippedItems[i];
					equippedItemIcons[i].SetItemData(itemCode);
					equippedItemIcons[i].SetItemClickCallback(onClickItem);
				}

				break;

			case BoardType.Profile:
				break;
		}
	}
}
