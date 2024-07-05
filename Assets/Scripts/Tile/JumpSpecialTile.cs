using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpSpecialTile", menuName = "2D/Tiles/Jump Special Tile")]
public class JumpSpecialTile : SpecialTileBase
{
	[SerializeField] private int count;

	public async override Task DoAction()
	{
		await playerDataContainer.AddCurrentOrder(count);
	}
}
