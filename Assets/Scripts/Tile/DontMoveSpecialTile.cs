using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DontMoveSpecialTile", menuName = "2D/Tiles/DontMove Special Tile")]
public class DontMoveSpecialTile : SpecialTileBase
{
	[SerializeField] private string buffItemCode = "ZeroDiceBuff";

	protected override void Reset()
	{
		base.Reset();

		buffItemCode = "ZeroDiceBuff";
		specialTileType = SpecialTileType.DontMove;
	}

	public override void DoAction()
	{
		Inventory.Instance.ApplyBuffFirst(buffItemCode);
	}

	public override IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
	}
}
