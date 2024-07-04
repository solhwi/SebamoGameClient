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

	[Header("[현재 위치한 타일 순서]")]
	public int currentTileOrder = 0;

	[Header("[현재 위치한 타일 인덱스]")]
	public int currentTileIndex = 0;

	[Header("[가지고 있는 주사위 수]")]
	public int hasDiceCount = 0;

	[Header("[타일 당 이동 시간]")]
	public float moveTimeByOneTile = 1.0f;

	[Header("[다른 플레이어들 정보]")]
	public PlayerPacketData[] otherPlayerPacketDatas = null;

	public void SaveCurrentTile(int currentTileOrder)
	{
		if (tileDataContainer.tileOrders.Length > currentTileOrder)
		{
			this.currentTileOrder = currentTileOrder;
		}
		else if (currentTileOrder <= 0)
		{
			this.currentTileOrder = 0;
		}
		else
		{
			this.currentTileOrder = tileDataContainer.tileOrders.Length - 1;
		}

		currentTileIndex = tileDataContainer.GetTileIndexByOrder(currentTileOrder);
	}

	public void AddCurrentOrder(int addOrderCount)
	{
		SaveCurrentTile(currentTileOrder + addOrderCount);
	}

	public void SetMyPacketData(MyPlayerPacketData myData)
	{
		currentTileIndex = myData.playerData.playerTileIndex;
		currentTileOrder = tileDataContainer.tileOrders[currentTileIndex];

		hasDiceCount = myData.playerData.hasDiceCount;
		playerName = myData.playerData.playerName;
	}

	public void SetOtherPacketData(IEnumerable<PlayerPacketData> otherDatas)
	{
		otherPlayerPacketDatas = otherDatas.ToArray();
	}
}
