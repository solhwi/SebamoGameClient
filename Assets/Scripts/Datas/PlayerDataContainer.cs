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
	public int hasDiceCount => myPlayerPacketData.hasDiceCount;
	public int currentTileOrder => myPlayerPacketData.playerTileOrder;

	[Header("[타일 당 이동 시간]")]
	public float moveTimeByOneTile = 1.0f;

	[Header("[내 플레이어 정보]")]
	public PlayerPacketData myPlayerPacketData = null;

	[Header("[다른 플레이어들 정보]")]
	public List<PlayerPacketData> otherPlayerPacketDatas = new List<PlayerPacketData>();

	[Header("[가질 수 있는 최대 주사위 수]")]
	public int MaxDiceCount = 10;

	[Header("[다음에 나올 주사위 수]")]
	[Range(1, 6)]
	public int NextDiceCount = 3;

	[Header("[보너스 주사위 (+)]")]
	[Range(0, 3)]
	public int NextBonusAddDiceCount = 0;

	[Header("[보너스 주사위 (x)]")]
	[Range(1, 6)]
	public int NextBonusMultiplyDiceCount = 1;

#if UNITY_EDITOR
	[MenuItem("Tools/캐릭터 위치 초기화")]
	public static void ResetCharacterPosition()
	{
		var container = AssetDatabase.LoadAssetAtPath<PlayerDataContainer>("Assets/Resources/Datas/PlayerDataContainer.asset");
		if (container != null)
		{
			container.SaveCurrentOrder(0);
		}

		EditorUtility.SetDirty(container);
		AssetDatabase.SaveAssetIfDirty(container);
	}
#endif

	public bool SaveCurrentOrder(int currentTileOrder)
	{
		if (tileDataContainer.tileOrders.Length > currentTileOrder)
		{
			myPlayerPacketData.playerTileOrder = currentTileOrder;
		}
		else if (currentTileOrder <= 0)
		{
			myPlayerPacketData.playerTileOrder = 0;
		}
		else
		{
			myPlayerPacketData.playerTileOrder = tileDataContainer.tileOrders.Length - 1;
		}

		return true;
	}

	public bool AddCurrentOrder(int addOrderCount)
	{
		return SaveCurrentOrder(currentTileOrder + addOrderCount);
	}

	public void ClearBonusDiceCount()
	{
		NextBonusAddDiceCount = 0;
		NextBonusMultiplyDiceCount = 1;
	}

	public void SetMyPacketData(MyPlayerPacketData myData)
	{
		if (myData == null)
			return;

		myPlayerPacketData = myData.playerData;
	}

	public void SetOtherPacketData(PlayerPacketDataCollection playerDataCollection)
	{
		if (playerDataCollection == null || playerDataCollection.playerDatas == null)
		{
			otherPlayerPacketDatas.Clear();
		}
		else
		{
			otherPlayerPacketDatas = playerDataCollection.playerDatas.ToList();
		}
	}
}
