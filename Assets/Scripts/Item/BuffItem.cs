using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum BuffActionType
{
	BonusDiceEffect,
	NextDiceChange,
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

	public virtual bool TryUse(PlayerDataContainer playerDataContainer)
	{
		return inventory.TryUseBuff(itemCode);
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

	public override bool TryUse(PlayerDataContainer playerDataContainer)
	{
		bool b = base.TryUse(playerDataContainer);
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

public class NextDiceBuffItem : BuffItem
{
	private NextDiceBuffType nextDiceBuffType;
	public NextDiceBuffItem(Inventory inventory, ItemTable.BuffItemData data) : base(inventory, data)
	{
		if (System.Enum.TryParse<NextDiceBuffType>(data.actionParameter, out var buffType))
		{
			nextDiceBuffType = buffType;
		}
	}

	public override bool TryUse(PlayerDataContainer playerDataContainer)
	{
		playerDataContainer.nextDiceBuffType = nextDiceBuffType;

		return base.TryUse(playerDataContainer);
	}
}
