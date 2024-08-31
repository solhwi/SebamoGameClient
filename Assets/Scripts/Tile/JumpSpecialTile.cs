using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

	public override void DoAction(TileDataManager tileDataManager)
	{
		int currentOrder = playerDataContainer.currentTileOrder;

		int nextOrder = tileDataManager.GetNextOrder(currentOrder, count, out var item);
		if (item != null)
		{
			item.Use(tileDataManager, playerDataContainer, nextOrder);
		}

		playerDataContainer.SaveCurrentOrder(nextOrder);
	}
		
	public async override Task OnDoTileAction(TileDataManager tileDataManager, int currentOrder, int nextOrder)
	{
		await Task.Yield();
	}
}	
