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

	public override void DoAction()
	{
		int currentOrder = playerDataContainer.currentTileOrder;

		int nextOrder = TileDataManager.Instance.GetNextOrder(currentOrder, count, out var item);
		if (item != null)
		{
			item.Use(playerDataContainer, nextOrder);
		}

		playerDataContainer.SaveCurrentOrder(nextOrder);
	}
		
	public async override Task OnDoTileAction(int currentOrder, int nextOrder)
	{
		await Task.Yield();
	}
}	
