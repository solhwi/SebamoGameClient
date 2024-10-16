using NPOI.SS.Formula.PTG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MyPlayerPacketData : PacketData
{
	public PlayerPacketData playerData;

	public string[] hasItems; // 가진 아이템
	public int[] hasItemCounts; // 가진 아이템 개수

	public string[] appliedBuffItems; // 적용 중인 버프 아이템
}

[System.Serializable]
public class PlayerPacketDataCollection : PacketData
{
	public PlayerPacketData[] playerDatas = null;
}


[System.Serializable]
public class PlayerPacketData : PacketData, IEquatable<PlayerPacketData>
{
	public string playerGroup; // 플레이어 그룹
	public string playerName; // 플레이어 이름

	public int playerTileOrder; // 현재 위치한 타일 순서
	public int hasDiceCount; // 가진 주사위 개수

	public string profileComment; // 프로필 코멘트

	public string[] equippedItems; // 장착 중인 아이템
	public string[] appliedProfileItems; // 프로필 아이템

	public bool Equals(PlayerPacketData other)
	{
		if (other == null)
			return false;

		return playerGroup == other.playerGroup && playerName == other.playerName;
	}
}

[System.Serializable]
public class TilePacketData : PacketData
{
	public int[] tileItemIndexes = null;
	public string[] tileItemCodes = null;
}


[System.Serializable]
public class PacketData
{

}