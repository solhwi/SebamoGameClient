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
			case BuffActionType.NextDiceOperationBuff:
				return new NextDiceOperationBuffItem(inventory, data);

			case BuffActionType.NextDiceChangeBuff:
				return new NextDiceChangeBuffItem(inventory, data);
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
