using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataContainer")]
public class PlayerDataContainer : ScriptableObject
{
	public TileDataContainer tileDataContainer;

	[Header("[현재 플레이어 이름]")]
	public string playerName = "솔휘";

	[Header("[현재 타일 내 위치]")]
	public int currentTileOrderIndex = 0;

	[Header("[가지고 있는 주사위 수]")]
	public int hasDiceCount = 0;

	[Header("[타일 당 이동 시간]")]
	public float moveTimeByOneTile = 1.0f;

	[Header("[다른 플레이어들 정보]")]
	public PlayerPacketData[] otherPlayerPacketDatas = null;

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

	public void SetMyPacketData(MyPlayerPacketData myData)
	{
		currentTileOrderIndex = myData.playerData.playerTileIndex;
		hasDiceCount = myData.playerData.hasDiceCount;
		playerName = myData.playerData.playerName;
	}

	public void SetOtherPacketData(IEnumerable<PlayerPacketData> otherDatas)
	{
		otherPlayerPacketDatas = otherDatas.ToArray();
	}
}
