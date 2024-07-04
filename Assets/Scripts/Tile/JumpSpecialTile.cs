using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpSpecialTile", menuName = "2D/Tiles/Jump Special Tile")]
public class JumpSpecialTile : SpecialTileBase
{
	[SerializeField] private int count;

	public override void DoAction()
	{
		playerDataContainer.AddCurrentOrder(count);
	}
}
