using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffItemFactory")]
public class BuffItemFactory : ScriptableObject
{
	[SerializeField] private Inventory inventory = null;
	[SerializeField] private ItemTable itemTable = null;

	public BuffItem Make(ItemTable.BuffItemData data)
	{
		switch (data.actionType)
		{
			case BuffActionType.BonusDiceEffect:
				return new MoreDiceBuffItem(inventory, data);

			case BuffActionType.NextDiceChange:
				return new NextDiceBuffItem(inventory, data);
		}

		return null;
	}

	public BuffItem Make(string itemCode)
	{
		if (itemTable.buffItemDataDictionary.TryGetValue(itemCode, out var buffItemData))
		{
			return Make(buffItemData);
		}

		return null;
	}
}
