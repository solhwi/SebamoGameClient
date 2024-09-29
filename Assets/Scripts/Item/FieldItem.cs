
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FieldActionType
{
	Normal, // 그 아이템 자체를 획득함
	Random, // 내부에 등록된 아이템을 획득함
	Banana, // 바나나
	Barricade, // 바리케이트
}

public abstract class FieldItem
{
	protected Inventory inventory = null;
	protected ItemTable.FieldItemData rawData = null;

	private GameObject obj;

	public string fieldItemCode { get; protected set; }
	public FieldActionType fieldActionType => rawData.actionType;

	public FieldItem(Inventory inventory, ItemTable.FieldItemData rawData)
	{
		this.inventory = inventory;
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
	public virtual void Use(PlayerDataContainer playerDataContainer, int tileOrder)
	{
		TileDataManager.Instance.TrySetTileItem(tileOrder, null);
	}
}

public abstract class DropFieldItem : FieldItem
{
	protected int dropCount;

	protected DropFieldItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{

	}

	public override void Use(PlayerDataContainer playerDataContainer, int tileOrder)
	{
		for (int i = 0; i < dropCount; i++)
		{
			inventory.TryAddItem(fieldItemCode);
		}

		base.Use(playerDataContainer,tileOrder);
	}
}

public abstract class ReplaceFieldItem : FieldItem
{
	public readonly int[] ranges = new int[2];

	protected ReplaceFieldItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
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

	public bool IsReplaceable( PlayerDataContainer playerDataContainer, int tileOrder)
	{
		int min = playerDataContainer.currentTileOrder + ranges[0];
		int max = playerDataContainer.currentTileOrder + ranges[1];

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

		bool bResult = inventory.TryRemoveItem(fieldItemCode);
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
	public NormalFieldItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{
		dropCount = int.Parse(rawData.actionParameter);
	}
}

public class RandomFieldItem : DropFieldItem
{
	public RandomFieldItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{
		var dropRecipeDictionary = ItemTable.ParseDropRecipeData(rawData.actionParameter);
		fieldItemCode = ItemTable.GetFieldItemCode(dropRecipeDictionary);
		dropCount = 1;
	}
}

public class BananaItem : ReplaceFieldItem
{
	public readonly int count = 0;

	public BananaItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{
		count = ItemTable.ParseCountData(rawData.actionParameter);
	}

	public override void Use(PlayerDataContainer playerDataContainer, int tileOrder)
	{
		int nextOrder = TileDataManager.Instance.GetNextOrder(tileOrder, count, out var item);
		if (item != null)
		{
			item.Use(playerDataContainer, nextOrder);
		}

		bool isSuccess = playerDataContainer.SaveCurrentOrder(nextOrder);
		if (isSuccess)
		{
			base.Use(playerDataContainer, tileOrder);
		}
	}
}

public class BarricadeItem : ReplaceFieldItem
{
	public BarricadeItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{

	}
}