using NPOI.SS.Formula.PTG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PacketType
{
	My,
	Other,
	Tile
}

[System.Serializable]
public class PlayerSaveData
{
	public MyPlayerPacketData data;
	public List<string> nameList;
}

[System.Serializable]
public class MyPlayerPacketData : PacketData
{
	public PlayerPacketData playerData;

	public string[] hasItems; // 가진 아이템
	public int[] hasItemCounts; // 가진 아이템 개수

	public string[] appliedBuffItems; // 적용 중인 버프 아이템

	public static MyPlayerPacketData Create(PlayerDataContainer playerDataContainer, Inventory inventory)
	{
		var data = new MyPlayerPacketData();

		data.type = (int)PacketType.My;
		data.playerGroup = playerDataContainer.playerGroup;
		data.playerName = playerDataContainer.playerName;

		data.playerData = new PlayerPacketData();

		data.playerData.playerName = playerDataContainer.playerName;
		data.playerData.playerGroup = playerDataContainer.playerGroup;
		data.playerData.hasDiceCount = playerDataContainer.hasDiceCount;
		data.playerData.playerTileOrder = playerDataContainer.currentTileOrder;

		data.playerData.equippedItems = playerDataContainer.equippedItems;
		data.playerData.appliedProfileItems = playerDataContainer.appliedProfileItems;

		data.hasItems = inventory.hasItems.Keys.ToArray();
		data.hasItemCounts = inventory.hasItems.Values.ToArray();
		data.appliedBuffItems = inventory.appliedBuffItemList.ToArray();

		return data;
	}
}

[System.Serializable]
public class PlayerPacketDataCollection : PacketData
{
	public PlayerPacketData[] playerDatas = null;
}


[System.Serializable]
public class PlayerPacketData : PacketData, IEquatable<PlayerPacketData>
{
	public int playerTileOrder; // 현재 위치한 타일 순서
	public int hasDiceCount; // 가진 주사위 개수

	public string profileComment; // 프로필 코멘트

	public string[] equippedItems; // 장착 중인 아이템
	public string[] appliedProfileItems; // 프로필 아이템

	public bool Equals(PlayerPacketData other)
	{
		if (other == null)
			return false;

		return Equals(other.playerGroup, other.playerName);
	}

	public bool Equals(string group, string name)
	{
		return playerGroup == group && playerName == name;
	}

	public PlayerPacketData Clone()
	{
		var data = new PlayerPacketData();
		data.appliedProfileItems = appliedProfileItems;
		data.playerTileOrder = playerTileOrder;
		data.playerGroup = playerGroup;
		data.equippedItems = equippedItems;
		data.profileComment = profileComment;
		data.hasDiceCount = hasDiceCount;
		data.playerName = playerName;
		data.type = type;
		return data;
	}
}

[System.Serializable]
public class TilePacketData : PacketData
{
	public int[] tileItemIndexes = null;
	public string[] tileItemCodes = null;

	public static TilePacketData Create(PlayerDataContainer playerDataContainer, string[] tileItems)
	{
		var data = new TilePacketData();
		data.playerGroup = playerDataContainer.playerGroup;
		data.playerName = playerDataContainer.playerName;
		data.type = (int)PacketType.Tile;

		List<int> indexes = new List<int>();
		List<string> itemCodes = new List<string>();

		for (int i = 0; i < tileItems.Length; i++)
		{
			string itemCode = tileItems[i];
			if (itemCode == null || itemCode == string.Empty)
				continue;

			indexes.Add(i);
			itemCodes.Add(itemCode);
		}

		data.tileItemIndexes = indexes.ToArray();
		data.tileItemCodes = itemCodes.ToArray();

		return data;
	}
}


[System.Serializable]
public class PacketData
{
	public int type; // 패킷 타입

	public string playerGroup; // 플레이어 그룹
	public string playerName; // 플레이어 이름

	public bool IsTile()
	{
		return this.type == 2;
	}
}