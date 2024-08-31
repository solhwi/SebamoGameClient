using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RollBackSpecialTile", menuName = "2D/Tiles/RollBack Special Tile")]
public class RollBackSpecialTile : SpecialTileBase
{
	[SerializeField] private int count;

	protected override void Reset()
	{
		base.Reset();

		specialTileType = SpecialTileType.RollBack;
	}

	public override void DoAction(TileDataManager tileDataManager)
	{
		playerDataContainer.AddCurrentOrder(-count);
	}

	public async override Task OnDoTileAction(TileDataManager tileDataManager, int currentOrder, int nextOrder)
	{
		await Task.Yield();
	}
}
