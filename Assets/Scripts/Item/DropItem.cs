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

	private GameObject obj;

	public string dropItemCode { get; protected set; }

	public DropItem(Inventory inventory, ItemTable.DropItemData rawData)
	{
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
		obj = new GameObject($"{rawData.key} ({worldTileData.index})");
		obj.transform.position = new Vector3(worldTileData.tileWorldPosition.x, worldTileData.tileWorldPosition.y, (int)LayerConfig.Item);
		obj.SetActive(true);

		var renderer = obj.AddComponent<SpriteRenderer>();
		renderer.sprite = itemSprite;

		return renderer;
	}

	public virtual void Destroy()
	{
		Resources.UnloadAsset(itemSprite);

		// 삭제가 안되는 중
		obj.SetActive(false);
		// MonoBehaviour.DestroyImmediate(obj);
	}

	public virtual void Use()
	{
		inventory.PushItem(dropItemCode);
		Destroy();
	}
}

public class NormalDropItem : DropItem
{
	public NormalDropItem(Inventory inventory, ItemTable.DropItemData rawData) : base(inventory, rawData)
	{
		dropItemCode = rawData.key;
	}
}

public class RandomDropItem : DropItem
{
	public RandomDropItem(Inventory inventory, ItemTable.DropItemData rawData) : base(inventory, rawData)
	{
		var dropRecipeDictionary = ItemTable.ParseDropRecipeData(rawData.actionParameter);
		dropItemCode = ItemTable.GetDropItemCode(dropRecipeDictionary);
	}
}