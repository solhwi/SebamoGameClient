using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase
{
	protected Inventory inventory = null;
	protected ItemTable itemTable = null;
	protected ItemRawData rawData = null;
	protected Sprite itemSprite = null;

	private GameObject obj;

	public ItemBase(Inventory inventory, ItemTable itemTable, string code)
	{
		this.inventory = inventory;
		this.itemTable = itemTable;

		rawData = itemTable.GetItemRawData(code);
		if (rawData != null)
		{
			itemSprite = Resources.Load<Sprite>(rawData.assetPathWithoutResources);
		}
	}

	// 월드 타일 위에만 생성이 가능함
	public virtual void Create(WorldTileData worldTileData)
	{
		obj = new GameObject($"{rawData.keyCode}");
		obj.transform.position = new Vector3(worldTileData.tileWorldPosition.x, worldTileData.tileWorldPosition.y, (int)LayerConfig.Item);

		var renderer = obj.AddComponent<SpriteRenderer>();
		renderer.sprite = itemSprite;
	}

	public virtual void Destroy()
	{
		Resources.UnloadAsset(itemSprite);
		MonoBehaviour.Destroy(obj);
	}

	/// <summary>
	/// 아이템을 먹었을 때 행동을 재정의
	/// </summary>
	public virtual void Use()
	{
		Destroy();
	}
}

public class DropItem : ItemBase
{
	public DropItem(Inventory inventory, ItemTable itemTable, string code) : base(inventory, itemTable, code)
	{

	}
	
	/// <summary>
	/// 인벤토리에 그냥 아이템을 더함
	/// </summary>
	public override void Use()
	{
		inventory.PushItem(rawData.keyCode);

		base.Use();
	}
}
