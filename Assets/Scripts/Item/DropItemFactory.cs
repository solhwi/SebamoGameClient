using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropItemFactory")]
public class DropItemFactory : ScriptableObject
{
	[SerializeField] private Inventory inventory = null;
	[SerializeField] private ItemTable itemTable = null;

	[SerializeField] private GameObject dropItemPrefab = null;

	public DropItem Make(ItemTable.DropItemData data)
	{
		switch (data.dropActionType)
		{
			case DropActionType.Random:
				return new RandomDropItem(inventory, data);
		}

		return new NormalDropItem(inventory, data);
	}

	public DropItem Make(string itemCode)
	{
		if (itemTable.dropItemDataDictionary.TryGetValue(itemCode, out var data))
		{
			return Make(data);
		}

		return null;
	}
}
