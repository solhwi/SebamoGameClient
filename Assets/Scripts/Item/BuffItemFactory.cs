using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffItemFactory")]
public class BuffItemFactory : DataContainer<BuffItemFactory>
{
	public BuffItem Make(ItemTable.BuffItemData data)
	{
		switch (data.actionType)
		{
			case BuffActionType.NextDiceOperationBuff:
				return new NextDiceOperationBuffItem(data);

			case BuffActionType.NextDiceChangeBuff:
				return new NextDiceChangeBuffItem(data);
		}

		return null;
	}

	public BuffItem Make(string itemCode)
	{
		if (ItemTable.Instance.buffItemDataDictionary.TryGetValue(itemCode, out var buffItemData))
		{
			return Make(buffItemData);
		}

		return null;
	}
}
