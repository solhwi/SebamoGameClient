using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultDataContainer")]
public class DefaultDataContainer : ScriptableObject
{
#if UNITY_EDITOR

	[SerializeField] private Inventory inventory;
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileDataContainer tileDataContainer;

	[Header("불러올 타 플레이어의 수")]
	[Space]
	[SerializeField] private int otherPlayerCount = 5;

	private const string defaultPlayerDataPath = "../SebamoScript/DefaultPlayerData.json";
	private const string defaultDataPath = "../SebamoScript/DefaultTileData.json";

	[ContextMenu("데이터 초기화 (저장은 되지 않음)")]
	public void Initialize()
	{
		var data = MakeMyFakeFacketData();

		playerDataContainer.SetMyPacketData(data);

		var otherData = MakeOtherFakePacketData();

		playerDataContainer.SetOtherPacketData(otherData.playerDatas);

		tileDataContainer.SetTileItemPacketData(null);

		EditorUtility.SetDirty(playerDataContainer);
		EditorUtility.SetDirty(inventory);
		EditorUtility.SetDirty(tileDataContainer);

		AssetDatabase.SaveAssetIfDirty(playerDataContainer);
		AssetDatabase.SaveAssetIfDirty(inventory);
		AssetDatabase.SaveAssetIfDirty(tileDataContainer);
	}

	[ContextMenu("데이터 저장")]
	public void Save()
	{
		SavePlayerData();
		SaveTileData();
	}

	[ContextMenu("데이터 불러오기")]
	public void Load()
	{
		var loadData = LoadPlayerData();
		playerDataContainer.SetMyPacketData(loadData.data);

		List<PlayerPacketData> otherDatas = new List<PlayerPacketData>();
		for (int i = 0; i < otherPlayerCount; i++)
		{
			var data = loadData.data.playerData.Clone();

			data.playerName = loadData.nameList[i];
			otherDatas.Add(data);
		}
		playerDataContainer.SetOtherPacketData(otherDatas);

		var tileData = LoadTileData();
		tileDataContainer.SetTileItemPacketData(tileData);

		EditorUtility.SetDirty(playerDataContainer);
		EditorUtility.SetDirty(inventory);
		EditorUtility.SetDirty(tileDataContainer);

		AssetDatabase.SaveAssets();
	}

	private void SavePlayerData()
	{
		var data = MyPlayerPacketData.Create(playerDataContainer, inventory);
		var nameList = playerDataContainer.otherPlayerPacketDatas.Select(d => d.playerName).ToList();

		var saveData = new PlayerSaveData();
		saveData.data = data;
		saveData.nameList = nameList;

		string jsonData = JsonUtility.ToJson(saveData);
		File.WriteAllText(defaultPlayerDataPath, jsonData);
	}

	private void SaveTileData()
	{
		var data = TilePacketData.Create(playerDataContainer, tileDataContainer.tileItems);

		string jsonData = JsonUtility.ToJson(data);
		File.WriteAllText(defaultDataPath, jsonData);
	}

	private PlayerSaveData LoadPlayerData()
	{
		string jsonData = File.ReadAllText(defaultPlayerDataPath);
		return JsonUtility.FromJson<PlayerSaveData>(jsonData);
	}

	private TilePacketData LoadTileData()
	{
		string jsonData = File.ReadAllText(defaultDataPath);
		return JsonUtility.FromJson<TilePacketData>(jsonData);
	}

	private MyPlayerPacketData MakeMyFakeFacketData()
	{
		var data = new MyPlayerPacketData();
		data.playerGroup = GroupType.Exp.ToString();
		data.playerName = "솔휘"; 

		data.playerData = new PlayerPacketData();

		data.playerData.playerGroup = data.playerGroup;
		data.playerData.playerName = data.playerName;

		data.playerData.hasDiceCount = 3;
		data.playerData.playerTileOrder = 0;

		data.playerData.equippedItems = new string[6] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword" };
		data.playerData.appliedProfileItems = new string[2] { "Profile_01", "Frame_01" };

		data.playerData.profileComment = "안녕하세요.";

		data.hasItems = new string[10] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword", "TwinDagger", "Coin", "Profile_01", "Frame_01" };
		data.hasItemCounts = new int[10] { 1, 1, 1, 1, 1, 1, 1, 100000, 1, 1 };
		data.appliedBuffItems = new string[] { };

		return data;
	}

	private PlayerPacketDataCollection MakeOtherFakePacketData()
	{
		var collection = new PlayerPacketDataCollection();

		List<PlayerPacketData> otherDatas = new List<PlayerPacketData>();

		string[] otherNames = new string[5] { "지현", "지홍", "동현", "상훈", "강욱" };

		foreach (string name in otherNames)
		{
			PlayerPacketData newData = new PlayerPacketData();

			newData.playerGroup = GroupType.Exp.ToString();
			newData.playerName = name;

			newData.hasDiceCount = 3;
			newData.playerTileOrder = 0;

			newData.profileComment = "안녕하세요.";

			newData.equippedItems = new string[6] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword" };
			newData.appliedProfileItems = new string[2] { "Profile_01", "Frame_01" };

			otherDatas.Add(newData);
		}

		collection.playerDatas = otherDatas.ToArray();
		return collection;
	}
#endif
}
