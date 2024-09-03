using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum BuffActionType
{
	MoreDice,
}

public class BuffItem
{
	private Inventory inventory;
	private ItemTable.BuffItemData data;

	public readonly string itemCode = string.Empty;

	public BuffItem(Inventory inventory, ItemTable.BuffItemData data)
	{
		this.inventory = inventory;
		this.data = data;

		itemCode = data.key;
	}

	public async virtual Task<bool> TryUse(PlayerDataContainer playerDataContainer)
	{
		return await inventory.TryUseBuff(itemCode);
	}
}

public class MoreDiceBuffItem : BuffItem
{
	public readonly MathType mathType;
	public readonly int count;

	public MoreDiceBuffItem(Inventory inventory, ItemTable.BuffItemData data) : base(inventory, data)
	{
		var pair = ItemTable.ParseBuffData(data.actionParameter);

		mathType = pair.Key;
		count = pair.Value;
	}

	public async override Task<bool> TryUse(PlayerDataContainer playerDataContainer)
	{
		bool b = await base.TryUse(playerDataContainer);
		if (b)
		{
			if (mathType == MathType.Add)
			{
				playerDataContainer.NextBonusAddDiceCount = count;
			}
			else if (mathType == MathType.Mul)
			{
				playerDataContainer.NextBonusMultiplyDiceCount = count;
			}
		}

		return b;
	}
}
