using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileActionFactory")]
public class TileActionFactory : ScriptableObject
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileTable tileTable;

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

	public TileAction Make(string tileCode)
	{
		if(tileTable.tileDataDictionary.TryGetValue(tileCode, out var tileData))
		{
			return Make(tileData);
		}

		return null;
	}
}
