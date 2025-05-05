using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;

public enum BuffActionType
{
	NextDiceOperationBuff,
	NextDiceChangeBuff,
}

public class BuffItem
{
	private Inventory inventory;
	private ItemTable.BuffItemData data;

	public readonly string itemCode = string.Empty;

	private GameObject effectObj;

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

	public virtual void CreateEffect(Transform owner)
	{
		ResourceManager.Instance.TryInstantiateAsync<GameObject>(data.effectPath, owner, true, (obj) =>
		{
			effectObj = obj;
		});
	}

	public virtual void DestroyEffect()
	{
		if (effectObj != null)
		{
			ObjectManager.Instance?.Destroy(effectObj);
		}
	}
}

public class NextDiceOperationBuffItem : BuffItem
{
	public readonly MathType mathType;
	public readonly float count;

	public NextDiceOperationBuffItem(Inventory inventory, ItemTable.BuffItemData data) : base(inventory, data)
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
				playerDataContainer.NextBonusAddDiceCount = (int)count;
			}
			else if (mathType == MathType.Mul)
			{
				playerDataContainer.NextBonusMultiplyDiceCount = count;
			}
		}

		return b;
	}
}

public class NextDiceChangeBuffItem : BuffItem
{
	private NextDiceChangeBuffType nextDiceBuffType;
	public NextDiceChangeBuffItem(Inventory inventory, ItemTable.BuffItemData data) : base(inventory, data)
	{
		if (System.Enum.TryParse<NextDiceChangeBuffType>(data.actionParameter, out var buffType))
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
