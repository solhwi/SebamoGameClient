using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileActionType
{
	Jump,
	RollBack,
}
public abstract class TileAction
{
    public TileAction() 
	{
	
	}

	public abstract void Invoke();
}

public class TileJumpAction : TileAction
{
	private PlayerDataContainer playerDataContainer = null;
	private int jumpTileCount = 0;

	public TileJumpAction(PlayerDataContainer playerDataContainer, string rawParameter) : base()
	{
		this.playerDataContainer = playerDataContainer;

		if (int.TryParse(rawParameter, out int value))
		{
			jumpTileCount = value;
		}
	}

	public override void Invoke()
	{
		playerDataContainer.AddCurrentOrderIndex(jumpTileCount);
	}
}

public class TileRollBackAction : TileAction
{
	private PlayerDataContainer playerDataContainer = null;
	private int rollBackTileCount = 0;

	public TileRollBackAction(PlayerDataContainer playerDataContainer, string rawParameter) : base()
	{
		this.playerDataContainer = playerDataContainer;

		if (int.TryParse(rawParameter, out int value))
		{
			rollBackTileCount = value;
		}
	}

	public override void Invoke()
	{
		playerDataContainer.AddCurrentOrderIndex(-rollBackTileCount);
	}
}