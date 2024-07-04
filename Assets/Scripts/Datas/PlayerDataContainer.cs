using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataContainer")]
public class PlayerDataContainer : ScriptableObject
{
	public TileDataContainer tileDataContainer;

	[Header("[현재 타일 내 위치]")]
	public int currentTileOrderIndex = 0;

	[Header("[타일 당 이동 시간]")]
	public float moveTimeByOneTile = 1.0f;

	public void SaveCurrentOrderIndex(int currentTileOrderIndex)
	{
		if (tileDataContainer.tileOrders.Length > currentTileOrderIndex)
		{
			this.currentTileOrderIndex = currentTileOrderIndex;
		}
		else if(currentTileOrderIndex <= 0)
		{
			this.currentTileOrderIndex = 0;
		}
		else
		{
			this.currentTileOrderIndex = tileDataContainer.tileOrders.Length - 1;
		}
	}

	public void AddCurrentOrderIndex(int addOrderCount)
	{
		SaveCurrentOrderIndex(currentTileOrderIndex + addOrderCount);
	}
}
