using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
		obj = ResourceManager.Instance.GetFieldItemObject(rawData, worldTileData);
		return obj;
	}

	public virtual GameObject Create(Transform parent, Vector3 pos, Vector3 rot)
	{
		obj = ResourceManager.Instance.GetFieldItemObject(rawData);
		
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
			ResourceManager.Instance.Destroy(obj);
		}
	}

	public virtual void Use(TileDataManager tileDataManager, PlayerDataContainer playerDataContainer, int tileOrder)
	{
		tileDataManager.TrySetTileItem(tileOrder, null);
	}
}

public abstract class DropFieldItem : FieldItem
{
	protected int dropCount;

	protected DropFieldItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{

	}

	public override void Use(TileDataManager tileDataManager, PlayerDataContainer playerDataContainer, int tileOrder)
	{
		for (int i = 0; i < dropCount; i++)
		{
			inventory.TryAddItem(fieldItemCode);
		}

		base.Use(tileDataManager, playerDataContainer,tileOrder);
	}
}

public abstract class ReplaceFieldItem : FieldItem
{
	public readonly int[] ranges = new int[2];

	protected ReplaceFieldItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{
		ranges = ItemTable.ParseRangeData(rawData.actionParameter);
	}

	public bool IsReplaceable(PlayerDataContainer playerDataContainer, int tileOrder)
	{
		int min = playerDataContainer.currentTileOrder + ranges[0];
		int max = playerDataContainer.currentTileOrder + ranges[1];

		bool bResult = min <= tileOrder && tileOrder <= max;
		return bResult;
	}

	public virtual bool Replace(TileDataManager tileDataManager, int tileOrder)
	{
		if (tileDataManager.IsAlreadyReplaced(tileOrder))
			return false;

		if (tileDataManager.IsSpecialTile(tileOrder))
			return false;

		bool bResult = inventory.TryRemoveItem(fieldItemCode);
		if (bResult == false)
			return false;

		return tileDataManager.TrySetTileItem(tileOrder, this);
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

	public override void Use(TileDataManager tileDataManager, PlayerDataContainer playerDataContainer, int tileOrder)
	{
		int nextOrder = tileDataManager.GetNextOrder(tileOrder, count, out var item);
		if (item != null)
		{
			item.Use(tileDataManager, playerDataContainer, nextOrder);
		}

		bool isSuccess = playerDataContainer.SaveCurrentOrder(nextOrder);
		if (isSuccess)
		{
			base.Use(tileDataManager, playerDataContainer, tileOrder);
		}
	}
}

public class BarricadeItem : ReplaceFieldItem
{
	public BarricadeItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{

	}
}