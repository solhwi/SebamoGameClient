using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "TeleportSpecialTile", menuName = "2D/Tiles/Teleport Special Tile")]
public class TeleportSpecialTile : SpecialTileBase
{
	[SerializeField] private int count;

	protected override void Reset()
	{
		base.Reset();

		specialTileType = SpecialTileType.Teleport;
	}

	public override void DoAction(TileDataManager tileDataManager)
	{
		playerDataContainer.AddCurrentOrder(count);
	}

	public async override Task OnDoTileAction(TileDataManager tileDataManager, int currentOrder, int nextOrder)
	{
		await Task.Yield();
	}
}
