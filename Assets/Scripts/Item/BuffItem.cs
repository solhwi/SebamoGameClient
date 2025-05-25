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
	private ItemTable.BuffItemData data;

	public readonly string itemCode = string.Empty;

	private GameObject effectObj;

	public BuffItem(ItemTable.BuffItemData data)
	{
		this.data = data;

		itemCode = data.key;
	}

	public virtual bool TryUse()
	{
		return Inventory.Instance.TryUseBuff(itemCode);
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

	public NextDiceOperationBuffItem(ItemTable.BuffItemData data) : base(data)
	{
		var pair = ItemTable.ParseBuffData(data.actionParameter);

		mathType = pair.Key;
		count = pair.Value;
	}

	public override bool TryUse()
	{
		bool b = base.TryUse();
		if (b)
		{
			if (mathType == MathType.Add)
			{
				PlayerDataContainer.Instance.NextBonusAddDiceCount = (int)count;
			}
			else if (mathType == MathType.Mul)
			{
				PlayerDataContainer.Instance.NextBonusMultiplyDiceCount = count;
			}
		}

		return b;
	}
}

public class NextDiceChangeBuffItem : BuffItem
{
	private NextDiceChangeBuffType nextDiceBuffType;
	public NextDiceChangeBuffItem(ItemTable.BuffItemData data) : base(data)
	{
		if (System.Enum.TryParse<NextDiceChangeBuffType>(data.actionParameter, out var buffType))
		{
			nextDiceBuffType = buffType;
		}
	}

	public override bool TryUse()
	{
		PlayerDataContainer.Instance.nextDiceBuffType = nextDiceBuffType;

		return base.TryUse();
	}
}
