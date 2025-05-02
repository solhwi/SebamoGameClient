using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DontMoveSpecialTile", menuName = "2D/Tiles/DontMove Special Tile")]
public class DontMoveSpecialTile : SpecialTileBase
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private string buffItemCode = "ZeroDiceBuff";

	protected override void Reset()
	{
		base.Reset();

#if UNITY_EDITOR
		inventory = AssetDatabase.LoadAssetAtPath<Inventory>("Assets/Bundles/Datas/Inventory.asset");
#endif
		buffItemCode = "ZeroDiceBuff";
		specialTileType = SpecialTileType.DontMove;
	}

	public override void DoAction()
	{
		inventory.ApplyBuffFirst(buffItemCode);
	}

	public override IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
	}
}
