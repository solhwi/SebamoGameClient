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
}

public abstract class FieldItem
{
	protected Inventory inventory = null;
	protected ItemTable.FieldItemData rawData = null;

	private GameObject obj;

	public string fieldItemCode { get; protected set; }

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

	public async virtual Task Use()
	{
		await Task.Yield();
	}
}

public abstract class DropFieldItem : FieldItem
{
	protected int dropCount;

	protected DropFieldItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{

	}

	public async override Task Use()
	{
		for (int i = 0; i < dropCount; i++)
		{
			await inventory.TryAddItem(fieldItemCode);
		}

		Destroy();
	}
}

public abstract class ReplaceFieldItem : FieldItem
{
	public readonly int[] ranges = new int[2];

	protected ReplaceFieldItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{
		ranges = ItemTable.ParseRangeData(rawData.actionParameter);
	}

	public async virtual Task<bool> Replace(int tileIndex)
	{
		bool bResult = await inventory.TryRemoveItem(fieldItemCode);
		if (bResult)
		{
			Destroy();
		}

		return bResult;
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
	public BananaItem(Inventory inventory, ItemTable.FieldItemData rawData) : base(inventory, rawData)
	{
		
	}
}