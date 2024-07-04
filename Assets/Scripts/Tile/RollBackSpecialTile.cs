using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RollBackSpecialTile", menuName = "2D/Tiles/RollBack Special Tile")]
public class RollBackSpecialTile : SpecialTileBase
{
	[SerializeField] private int count;

	public override void DoAction()
	{
		playerDataContainer.AddCurrentOrder(-count);
	}
}
