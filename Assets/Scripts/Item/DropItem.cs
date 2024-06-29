using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DropActionType
{
	Normal, // 그 아이템 자체를 획득함
	Random, // 내부에 등록된 아이템을 획득함
}

public abstract class DropItem
{
	protected Inventory inventory = null;
	protected ItemTable.DropItemData rawData = null;
	protected Sprite itemSprite = null;
	protected int dropCount;

	private GameObject obj;

	public string dropItemCode { get; protected set; }

	public DropItem(Inventory inventory, ItemTable.DropItemData rawData)
	{
		this.inventory = inventory;
		this.rawData = rawData;

		if (rawData != null)
		{
			string path = rawData.GetAssetPathWithoutResources();
			itemSprite = ResourceManager.Instance.Load<Sprite>(path);
		}
	}

	// 월드 타일 위에만 생성이 가능함
	public virtual GameObject Create(WorldTileData worldTileData)
	{
		obj = ResourceManager.Instance.GetDropItemObject(rawData, worldTileData);
		return obj;
	}

	public virtual void Destroy()
	{
		ResourceManager.Instance.Destroy(obj);
	}

	public virtual void Use()
	{
		for(int i = 0; i < dropCount; i++)
		{
			inventory.AddItem(dropItemCode);
		}

		Destroy();
	}
}

public class NormalDropItem : DropItem
{
	public NormalDropItem(Inventory inventory, ItemTable.DropItemData rawData) : base(inventory, rawData)
	{
		dropItemCode = rawData.key;
		dropCount = int.Parse(rawData.actionParameter);
	}
}

public class RandomDropItem : DropItem
{
	public RandomDropItem(Inventory inventory, ItemTable.DropItemData rawData) : base(inventory, rawData)
	{
		var dropRecipeDictionary = ItemTable.ParseDropRecipeData(rawData.actionParameter);
		dropItemCode = ItemTable.GetDropItemCode(dropRecipeDictionary);
		dropCount = 1;
	}
}