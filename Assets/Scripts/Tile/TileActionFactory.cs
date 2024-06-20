using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileActionFactory")]
public class TileActionFactory : ScriptableObject
{
	[SerializeField] private PlayerDataContainer playerDataContainer;

	public TileAction Make(TileTable.TileData rawData)
	{
		switch (rawData.tileActionType)
		{
			case TileActionType.Jump:
				return new TileJumpAction(playerDataContainer, rawData.tileActionParameter);

			case TileActionType.RollBack:
				return new TileRollBackAction(playerDataContainer, rawData.tileActionParameter);
		}

		return null;
	}
}
