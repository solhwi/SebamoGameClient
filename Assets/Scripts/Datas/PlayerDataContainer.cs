using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataContainer")]
public class PlayerDataContainer : ScriptableObject
{
	public TileDataContainer tileDataContainer;
	public string playerGroup => myPlayerPacketData.playerGroup;
	public string playerName => myPlayerPacketData.playerName;
	public int currentTileIndex => myPlayerPacketData.playerTileIndex;
	public int hasDiceCount => myPlayerPacketData.hasDiceCount;

	[Header("[현재 위치한 타일 순서]")]
	public int currentTileOrder = 0;

	[Header("[가지고 있는 주사위 수]")]

	[Header("[타일 당 이동 시간]")]
	public float moveTimeByOneTile = 1.0f;

	[Header("[내 플레이어 정보]")]
	public PlayerPacketData myPlayerPacketData = null;

	[Header("[다른 플레이어들 정보]")]
	public PlayerPacketData[] otherPlayerPacketDatas = null;

	private const int MaxDiceCount = 10;

#if UNITY_EDITOR
	[MenuItem("Tools/캐릭터 위치 초기화")]
	public static void ResetCharacterPosition()
	{
		var container = AssetDatabase.LoadAssetAtPath<PlayerDataContainer>("Assets/Resources/Datas/PlayerDataContainer.asset");
		if (container != null)
		{
			container.currentTileOrder = 0;
		}

		EditorUtility.SetDirty(container);
		AssetDatabase.SaveAssetIfDirty(container);
	}
#endif

	public async Task<bool> SaveCurrentOrder(int currentTileOrder)
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

		myPlayerPacketData.playerTileIndex = tileDataContainer.GetTileIndexByOrder(currentTileOrder);

		return await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task<bool> AddCurrentOrder(int addOrderCount)
	{
		return await SaveCurrentOrder(currentTileOrder + addOrderCount);
	}

	public async Task<bool> UseDiceCount()
	{
		if (hasDiceCount <= 0)
			return false;

		myPlayerPacketData.hasDiceCount--;

		return await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task<bool> AddDiceCount()
	{
		if (hasDiceCount >= MaxDiceCount)
			return false;

		myPlayerPacketData.hasDiceCount++;

		return await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public void SetMyPacketData(MyPlayerPacketData myData)
	{
		if (myData == null)
			return;

		myPlayerPacketData = myData.playerData;
		currentTileOrder = tileDataContainer.tileOrders[currentTileIndex];
	}

	public void SetOtherPacketData(PlayerPacketDataCollection playerDataCollection)
	{
		if (playerDataCollection == null || playerDataCollection.playerDatas == null)
		{
			otherPlayerPacketDatas = null;
		}
		else
		{
			otherPlayerPacketDatas = playerDataCollection.playerDatas.ToArray();
		}
	}
}
