
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

	public override void DoAction()
	{
		playerDataContainer.AddCurrentOrder(count);
	}

	public override IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
	}
}
