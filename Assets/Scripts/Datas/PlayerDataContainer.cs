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
	[SerializeField] private Inventory inventory;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private TileDataContainer tileDataContainer;

	[Header("[현재 플레이어 그룹]")]
	public string playerGroup;

	[Header("[현재 플레이어 이름]")]
	public string playerName;

	[Header("[현재 보유한 주사위 수]")]
	public int hasDiceCount;

	[Header("[현재 타일 위치]")]
	public int currentTileOrder;

	[Header("[프로필 코멘트]")]
	public string profileComment;

	public string[] equippedItems
	{
		get
		{
			return inventory.equippedItems;
		}
	}

	public string[] appliedProfileItems
	{
		get
		{
			return inventory.appliedProfileItems;
		}
	}

	public bool IsMeEnded
	{
		get
		{
			return currentTileOrder == LastTileOrder;
		}
	}

	public int LastTileOrder => tileDataContainer.tileOrders.Length - 1;

	[Header("[타일 당 이동 시간]")]
	public float moveTimeByOneTile = 1.0f;

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
	[Range(-6, 6)]
	public float NextBonusMultiplyDiceCount = 1.0f;

	public NextDiceChangeBuffType nextDiceBuffType = NextDiceChangeBuffType.None;

#if UNITY_EDITOR
	[MenuItem("Tools/캐릭터 위치 초기화")]
	public static void ResetCharacterPosition()
	{
		var container = AssetDatabase.LoadAssetAtPath<PlayerDataContainer>("Assets/Bundles/Datas/PlayerDataContainer.asset");
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

		return true;
	}

	public bool AddCurrentOrder(int addOrderCount)
	{
		return SaveCurrentOrder(currentTileOrder + addOrderCount);
	}

	public void ClearBonusDiceData()
	{
		NextBonusAddDiceCount = 0;
		NextBonusMultiplyDiceCount = 1.0f;
		nextDiceBuffType = NextDiceChangeBuffType.None;
	}

	public void UseDice()
	{
		hasDiceCount = hasDiceCount > 0 ? hasDiceCount - 1 : 0;
	}

	public bool IsMine(string group, string name)
	{
		return playerGroup == group && playerName == name;
	}

	public void SetMyPacketData(MyPlayerPacketData myData)
	{
		if (myData == null)
			return;

		playerGroup = myData.playerData.playerGroup;
		playerName = myData.playerData.playerName;
		hasDiceCount = myData.playerData.hasDiceCount;
		currentTileOrder = myData.playerData.playerTileOrder;
		profileComment = myData.playerData.profileComment;

		inventory.SetMyPacketData(myData);
	}

	public void SetOtherPacketData(IEnumerable<PlayerPacketData> playerDatas)
	{
		if (playerDatas == null)
		{
			otherPlayerPacketDatas.Clear();
		}
		else
		{
			otherPlayerPacketDatas = playerDatas.ToList();
		}
	}

	public IEnumerable<PlayerPacketData> GetAllPlayerData()
	{
		yield return GetMyPlayerData();

		foreach (var otherPlayerData in otherPlayerPacketDatas)
		{
			yield return otherPlayerData;
		}
	}

	public PlayerPacketData GetMyPlayerData()
	{
		var newData = new PlayerPacketData();

		newData.equippedItems = equippedItems.ToArray();
		newData.appliedProfileItems = appliedProfileItems.ToArray();
		newData.playerGroup = playerGroup;
		newData.playerName = playerName;
		newData.profileComment = profileComment;
		newData.playerTileOrder = currentTileOrder;
		newData.hasDiceCount = hasDiceCount;

		return newData;
	}

	public PlayerPacketData GetPlayerData(string group, string name)
	{
		if (IsMine(group, name))
		{
			return GetMyPlayerData();
		}

		return otherPlayerPacketDatas.Find(p => p.playerGroup == group && p.playerName == name);
	}

	public CharacterType GetEquippedBodyType(string group, string name, int index)
	{
		var data = GetPlayerData(group, name);
		if (data == null)
			return CharacterType.Max;

		if (index < 0 || index >= data.equippedItems.Length)
			return CharacterType.Max;

		string itemCode = data.equippedItems[index];
		return itemTable.GetPartsCharacterType(itemCode);
	}

	public IEnumerable<PropType> GetEquippedPropType(string group, string name)
	{
		var data = GetPlayerData(group, name);
		if (data == null)
			yield break;

		if (data.equippedItems == null)
			yield break;

		int index = 5;

		if (data.equippedItems.Length <= index)
			yield break;

		string itemCode = data.equippedItems[index];

		var propType = itemTable.GetItemPropType(itemCode);
		if (propType == PropType.TwinDagger_L)
		{
			yield return PropType.TwinDagger_L;
			yield return PropType.TwinDagger_R;
		}
		else
		{
			yield return propType;
		}
	}

	public CharacterType GetCurrentPartsCharacterType(string group, string name, CharacterPartsType partsType)
	{
		int index = -1;

		if (partsType == CharacterPartsType.Accessory)
			index = 4;

		if (partsType == CharacterPartsType.Face)
			index = 3;

		if (partsType == CharacterPartsType.RightEye || partsType == CharacterPartsType.Eye)
			index = 2;

		if (partsType == CharacterPartsType.Hair || partsType == CharacterPartsType.FrontHair)
			index = 1;

		if (partsType == CharacterPartsType.Body)
			index = 0;

		return GetEquippedBodyType(group, name, index);
	}

	public bool IsEnded(string group, string name)
	{
		if (IsMine(group, name))
		{
			return IsMeEnded;
		}

		foreach (var data in otherPlayerPacketDatas)
		{
			if (data.Equals(group, name))
			{
				if (data.playerTileOrder == LastTileOrder)
				{
					return true;
				}
			}	
		}

		return false;
	}
}
