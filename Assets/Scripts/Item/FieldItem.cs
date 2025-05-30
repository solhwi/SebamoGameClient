using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum FieldActionType
{
	Normal, // 그 아이템 자체를 획득함
	Random, // 내부에 등록된 아이템을 획득함
	Banana, // 바나나
	Barricade, // 바리케이트
	NextDiceOperationBuff, // 다음 주사위에 적용되는 추가 계산 버프
	NextDiceChangeBuff,	// 다음 주사위에 적용되는 버프
}

public enum NextDiceChangeBuffType
{
	None = -1,

	OneOrSix, // 1회 1 or 6
	Odd,	// 홀수
	Even,	// 짝수
}

public abstract class FieldItem
{
	protected ItemTable.FieldItemData rawData = null;

	private GameObject obj;
	private GameObject effectObj;

	public string fieldItemCode { get; protected set; }
	public FieldActionType fieldActionType => rawData.actionType;

	public FieldItem(ItemTable.FieldItemData rawData)
	{
		this.rawData = rawData;

		fieldItemCode = rawData.key;
	}

	// 월드 타일 위에만 생성이 가능함
	public virtual GameObject Create(WorldTileData worldTileData)
	{
		obj = ObjectManager.Instance.GetFieldItemObject(rawData, worldTileData);
		return obj;
	}

	public virtual GameObject Create(Transform parent, Vector3 pos, Vector3 rot)
	{
		obj = ObjectManager.Instance.GetFieldItemObject(rawData);
		
		if(obj != null)
		{
			obj.transform.SetParent(parent);
			obj.transform.localPosition = pos;
			obj.transform.localEulerAngles = rot;
		}

		return obj;
	}

	public virtual void Destroy()
	{
		if (obj != null)
		{
			ObjectManager.Instance.Destroy(obj);
		}
	}

	// 아이템 사용 후 반드시 서버와 동기화를 해야 하나, 이 곳에서 하면 통신을 너무 자주 하게 되어 밖으로 뺌
	// 이 경우 Use Process에서 실패한다하여 되감거나 다른 처리룰 해줄 방법은 없음
	// 그냥 실행한다.
	public virtual bool Use(int tileOrder)
	{
		return TileDataManager.Instance.TrySetTileItem(tileOrder, null);
	}

	public virtual void CreateEffect(Transform owner)
	{
		ResourceManager.Instance.TryInstantiateAsync<GameObject>(rawData.effectPath, owner, true, (obj) =>
		{
			effectObj = obj;
		});
	}

	public virtual void DestroyEffect()
	{
		if (obj != null)
		{
			ObjectManager.Instance.Destroy(effectObj);
		}
	}

	public virtual IEnumerator ChangeState(CharacterAnimationController controller)
	{
		yield return null;
	}
}

public abstract class DropFieldItem : FieldItem
{
	protected int dropCount;

	protected DropFieldItem(ItemTable.FieldItemData rawData) : base(rawData)
	{

	}

	public override bool Use(int tileOrder)
	{
		for (int i = 0; i < dropCount; i++)
		{
			Inventory.Instance.TryAddItem(fieldItemCode);
		}

		base.Use(tileOrder);

		return true;
	}
}

public abstract class ReplaceFieldItem : FieldItem
{
	public readonly int[] ranges = new int[2];

	protected ReplaceFieldItem(ItemTable.FieldItemData rawData) : base(rawData)
	{
		ranges = ItemTable.ParseRangeData(rawData.actionParameter);
	}

	public GameObject CreateDummy(WorldTileData worldTileData)
	{
		var obj = Create(worldTileData);
		if (obj == null)
			return null;

		var sprite = obj.GetComponentInChildren<SpriteRenderer>();
		if (sprite == null)
			return null;

		sprite.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		return obj;
	}

	public bool IsReplaceable(int tileOrder)
	{
		int min = PlayerDataContainer.Instance.currentTileOrder + ranges[0];
		int max = PlayerDataContainer.Instance.currentTileOrder + ranges[1];

		min = Math.Max(0, min);
		max = Math.Min(max, TileDataManager.Instance.tileBoardDatas.Length - 1);

		bool bResult = min <= tileOrder && tileOrder <= max;

		bResult &= TileDataManager.Instance.IsAlreadyReplaced(tileOrder) == false;
		bResult &= TileDataManager.Instance.IsSpecialTile(tileOrder) == false;

		return bResult;
	}

	public virtual IEnumerator Replace(int tileOrder, Action<TilePacketData> onSuccess)
	{
		if (TileDataManager.Instance.IsAlreadyReplaced(tileOrder))
			yield break;

		if (TileDataManager.Instance.IsSpecialTile(tileOrder))
			yield break;

		bool bResult = Inventory.Instance.TryRemoveItem(fieldItemCode);
		if (bResult == false)
			yield break;

		bResult = TileDataManager.Instance.TrySetTileItem(tileOrder, this);
		if (bResult == false)
			yield break;

		yield return HttpNetworkManager.Instance.TryPostTileData(onSuccess);
	}
}

public class NormalFieldItem : DropFieldItem
{
	public NormalFieldItem(ItemTable.FieldItemData rawData) : base(rawData)
	{
		string[] columns = rawData.actionParameter.Split(':');

		fieldItemCode = columns[0];
		dropCount = int.Parse(columns[1]);
	}
}

public class RandomFieldItem : DropFieldItem
{
	public RandomFieldItem(ItemTable.FieldItemData rawData) : base(rawData)
	{
		var dropRecipeDictionary = ItemTable.ParseDropRecipeData(rawData.actionParameter);
		fieldItemCode = ItemTable.GetFieldItemCode(dropRecipeDictionary);
		dropCount = 1;
	}
}

public class BananaItem : ReplaceFieldItem
{
	public readonly int count = 0;

	public BananaItem(ItemTable.FieldItemData rawData) : base(rawData)
	{
		count = ItemTable.ParseCountData(rawData.actionParameter);
	}

	public override bool Use(int tileOrder)
	{
		int nextOrder = TileDataManager.Instance.GetNextOrder(tileOrder, count, out var item);
		if (item != null)
		{
			item.Use(nextOrder);
		}

		bool isSuccess = PlayerDataContainer.Instance.SaveCurrentOrder(nextOrder);
		if (isSuccess)
		{
			base.Use(tileOrder);
		}

		return true;
	}
}

public class BarricadeItem : ReplaceFieldItem
{
	public BarricadeItem(ItemTable.FieldItemData rawData) : base(rawData)
	{

	}

	public override IEnumerator ChangeState(CharacterAnimationController controller)
	{
		yield return base.ChangeState(controller);

		controller.ChangeState(CharacterStateType.DropItem, CharacterState.BarricadeDown);
		yield return controller.WaitForEnd(CharacterState.BarricadeDown.ToString());
	}
}

public class NextDiceOperationBuffFieldItem : ReplaceFieldItem
{
	MathType mathType; 
	float count = 0;

	public NextDiceOperationBuffFieldItem(ItemTable.FieldItemData rawData) : base(rawData)
	{
		string[] columns = rawData.actionParameter.Split('/');

		var pair = ItemTable.ParseBuffData(columns[2]);

		mathType = pair.Key;
		count = pair.Value;
	}

	public override bool Use(int tileOrder)
	{
		base.Use(tileOrder);

		return Inventory.Instance.ApplyBuffFirst(fieldItemCode);
	}

	public override IEnumerator ChangeState(CharacterAnimationController controller)
	{
		yield return base.ChangeState(controller);

		CharacterState nextState = CharacterState.Idle;

		if (mathType == MathType.Mul && count > 1)
		{
			nextState = CharacterState.DoubleDiceBuff;
		}
		else if (mathType == MathType.Mul && count > 0)
		{
			nextState = CharacterState.HalfDiceBuff;
		}
		else if (mathType == MathType.Mul && count < 0)
		{
			nextState = CharacterState.MinusDiceBuff;
		}

		controller.ChangeState(CharacterStateType.DropItem, nextState);
		yield return controller.WaitForEnd(nextState.ToString());
	}
}

public class NextDiceChangeBuffFieldItem : ReplaceFieldItem
{
	NextDiceChangeBuffType nextDiceBuffType = NextDiceChangeBuffType.None;

	public NextDiceChangeBuffFieldItem(ItemTable.FieldItemData rawData) : base(rawData)
	{
		string[] columns = rawData.actionParameter.Split('/');

		if (System.Enum.TryParse<NextDiceChangeBuffType>(columns[2], out var buffType))
		{
			nextDiceBuffType = buffType;
		}
	}

	public override bool Use(int tileOrder)
	{
		base.Use(tileOrder);

		return Inventory.Instance.ApplyBuffFirst(fieldItemCode);
	}

	public override IEnumerator ChangeState(CharacterAnimationController controller)
	{
		yield return base.ChangeState(controller);

		CharacterState nextState = CharacterState.Idle;

		if (nextDiceBuffType == NextDiceChangeBuffType.OneOrSix)
		{
			nextState = CharacterState.DrunkBuff;
		}
		else if (nextDiceBuffType == NextDiceChangeBuffType.Odd)
		{
			nextState = CharacterState.OddBuff;
		}
		else if (nextDiceBuffType == NextDiceChangeBuffType.Even)
		{
			nextState = CharacterState.EvenBuff;
		}

		controller.ChangeState(CharacterStateType.DropItem, nextState);
		yield return controller.WaitForEnd(nextState.ToString());
	}
}