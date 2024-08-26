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

	public async override Task DoAction(TileDataManager tileDataManager)
	{
		int currentOrder = playerDataContainer.currentTileOrder;
		int nextOrder = currentOrder;

		for (; nextOrder < currentOrder + count; nextOrder++)
		{
			var fieldItem = tileDataManager.GetCurrentTileItem(nextOrder);
			if (fieldItem is BarricadeItem barricadeItem)
			{
				await barricadeItem.Use(tileDataManager, nextOrder);
				break;
			}
		}

		await playerDataContainer.SaveCurrentOrder(nextOrder);
	}

	public async override Task OnDoTileAction(TileDataManager tileDataManager, int currentOrder, int nextOrder)
	{
		await Task.Yield();
	}
}
