
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "JumpSpecialTile", menuName = "2D/Tiles/Jump Special Tile")]
public class JumpSpecialTile : SpecialTileBase
{
	[SerializeField] private int count;

	protected override void Reset()
	{
		base.Reset();

		specialTileType = SpecialTileType.Jump;
	}

	public override void DoAction()
	{
		int currentOrder = PlayerDataContainer.Instance.currentTileOrder;

		int nextOrder = TileDataManager.Instance.GetNextOrder(currentOrder, count, out var item);
		if (item != null)
		{
			item.Use(nextOrder);
		}

		PlayerDataContainer.Instance.SaveCurrentOrder(nextOrder);
	}
		
	public override IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
	}
}	
