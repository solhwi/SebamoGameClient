
using System.Collections;
using System.Collections.Generic;

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

	public override void DoAction()
	{
		PlayerDataContainer.Instance.AddCurrentOrder(-count);
	}

	public  override IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
	}
}
