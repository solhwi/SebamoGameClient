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

	[SerializeField] private CharacterView uiCharacterView;
	[SerializeField] private List<ItemIcon> equippedItemIcons = new List<ItemIcon>();

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
					equippedItemIcons[i].SetItemClickCallback(OnClickEquipment);
				}

				break;

			case BoardType.Profile:
				break;
		}
	}

	public void OnClickEquipment(string itemCode)
	{
		if (itemTable.IsEnableEquipOffItem(itemCode) == false)
		{
			Debug.Log($"해당 아이템 [{itemCode}]는 벗을 수 없는 아이템입니다.");
		}
		else
		{
			inventory.TryEquipOff(itemCode).Wait();
			uiCharacterView.RefreshCharacter();
		}
	}
}
