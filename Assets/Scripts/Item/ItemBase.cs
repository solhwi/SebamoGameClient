using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase
{
	protected Inventory inventory;
	protected ItemTable.ItemData rawData = null;
	protected Sprite itemSprite = null;

	public ItemBase(Inventory inventory, ItemTable.ItemData rawData)
	{
		this.inventory = inventory;
		this.rawData = rawData;
		itemSprite = Resources.Load<Sprite>(rawData.assetPath);
	}

	// 월드 타일 위에만 생성이 가능함
	public virtual void Create(WorldTileData worldTileData)
	{
		var g = new GameObject($"{rawData.key}");
		g.transform.position = new Vector3(worldTileData.tileWorldPosition.x, worldTileData.tileWorldPosition.y, (int)LayerConfig.Item);

		var renderer = g.AddComponent<SpriteRenderer>();
		renderer.sprite = itemSprite;
	}

	public virtual void OnDestroy()
	{
		Resources.UnloadAsset(itemSprite);
	}

	/// <summary>
	/// 아이템을 먹었을 때 행동을 재정의
	/// </summary>
	public abstract void OnUse();
}

public class DropItem : ItemBase
{
	public DropItem(Inventory inventory, ItemTable.ItemData rawData) : base(inventory, rawData)
	{

	}
	
	/// <summary>
	/// 인벤토리에 그냥 아이템을 더함
	/// </summary>
	public override void OnUse()
	{
		inventory.PushItem(rawData.key);
	}
}
