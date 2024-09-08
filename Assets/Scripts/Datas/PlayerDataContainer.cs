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

	public void ClearBonusDiceCount()
	{
		NextBonusAddDiceCount = 0;
		NextBonusMultiplyDiceCount = 1;
	}

	public void SetMyPacketData(MyPlayerPacketData myData)
	{
		if (myData == null)
			return;

		playerGroup = myData.playerData.playerGroup;
		playerName = myData.playerData.playerName;
		hasDiceCount = myData.playerData.hasDiceCount;
		currentTileOrder = myData.playerData.playerTileOrder;

		inventory.SetMyPacketData(myData);
	}

	public void SetOtherPacketData(PlayerPacketData[] playerDatas)
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
		newData.playerTileOrder = currentTileOrder;
		newData.hasDiceCount = hasDiceCount;

		return newData;
	}

	public PlayerPacketData GetPlayerData(string group, string name)
	{
		if (playerGroup == group && playerName == name)
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

		int index = 5;
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
}
