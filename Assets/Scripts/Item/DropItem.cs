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
	protected DropRecipeTable dropRecipeTable = null;
	protected ItemTable.DropItemData rawData = null;
	protected Sprite itemSprite = null;

	private GameObject obj;

	public string dropItemCode { get; protected set; }

	public DropItem(DropRecipeTable dropRecipeTable, Inventory inventory, ItemTable.DropItemData rawData)
	{
		this.dropRecipeTable = dropRecipeTable;
		this.inventory = inventory;
		this.rawData = rawData;

		if (rawData != null)
		{
			string path = rawData.GetAssetPathWithoutResources();
			itemSprite = Resources.Load<Sprite>(path);
		}
	}

	// 월드 타일 위에만 생성이 가능함
	public virtual SpriteRenderer Create(WorldTileData worldTileData)
	{
		obj = new GameObject($"{rawData.key}");
		obj.transform.position = new Vector3(worldTileData.tileWorldPosition.x, worldTileData.tileWorldPosition.y, (int)LayerConfig.Item);

		var renderer = obj.AddComponent<SpriteRenderer>();
		renderer.sprite = itemSprite;

		return renderer;
	}

	public virtual void Destroy()
	{
		Resources.UnloadAsset(itemSprite);
		MonoBehaviour.Destroy(obj);
	}

	public virtual void Use()
	{
		inventory.PushItem(dropItemCode);
		Destroy();
	}
}

public class NormalDropItem : DropItem
{
	public NormalDropItem(DropRecipeTable dropRecipeTable, Inventory inventory, ItemTable.DropItemData rawData) : base(dropRecipeTable, inventory, rawData)
	{
		dropItemCode = rawData.key;
	}
}

public class RandomDropItem : DropItem
{
	public RandomDropItem(DropRecipeTable dropRecipeTable, Inventory inventory, ItemTable.DropItemData rawData) : base(dropRecipeTable, inventory, rawData)
	{
		if (dropRecipeTable.recipeDataDictionary.TryGetValue(rawData.recipeCode, out var recipeData))
		{
			var dropRecipeDictionary = DropRecipeTable.ParseDropRecipeData(recipeData.recipe);
			dropItemCode = DropRecipeTable.GetDropItemCode(dropRecipeDictionary);
		}	
	}
}