using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultDataContainer")]
public class DefaultDataContainer : DataContainer<DefaultDataContainer>
{
#if UNITY_EDITOR
	[Header("불러올 타 플레이어의 수")]
	[Space]
	[SerializeField] private int otherPlayerCount = 5;

	private const string defaultPlayerDataPath = "../SebamoScript/DefaultPlayerData.json";
	private const string defaultDataPath = "../SebamoScript/DefaultTileData.json";

	[ContextMenu("데이터 초기화 (저장은 되지 않음)")]
	public void Initialize()
	{
		var data = MakeMyFakeFacketData();

		PlayerDataContainer.Instance.SetMyPacketData(data);

		var otherData = MakeOtherFakePacketData();

		PlayerDataContainer.Instance.SetOtherPacketData(otherData.playerDatas);

		TileDataContainer.Instance.SetTileItemPacketData(null);

		EditorUtility.SetDirty(PlayerDataContainer.Instance);
		EditorUtility.SetDirty(Inventory.Instance);
		EditorUtility.SetDirty(TileDataContainer.Instance);

		AssetDatabase.SaveAssetIfDirty(PlayerDataContainer.Instance);
		AssetDatabase.SaveAssetIfDirty(Inventory.Instance);
		AssetDatabase.SaveAssetIfDirty(TileDataContainer.Instance);
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
		PlayerDataContainer.Instance.SetMyPacketData(loadData.data);

		List<PlayerPacketData> otherDatas = new List<PlayerPacketData>();
		for (int i = 0; i < otherPlayerCount; i++)
		{
			var data = loadData.data.playerData.Clone();

			data.playerName = loadData.nameList[i];
			otherDatas.Add(data);
		}
		PlayerDataContainer.Instance.SetOtherPacketData(otherDatas);

		var tileData = LoadTileData();
		TileDataContainer.Instance.SetTileItemPacketData(tileData);

		EditorUtility.SetDirty(PlayerDataContainer.Instance);
		EditorUtility.SetDirty(Inventory.Instance);
		EditorUtility.SetDirty(TileDataContainer.Instance);

		AssetDatabase.SaveAssets();
	}

	private void SavePlayerData()
	{
		var data = MyPlayerPacketData.Create();
		var nameList = PlayerDataContainer.Instance.otherPlayerPacketDatas.Select(d => d.playerName).ToList();

		var saveData = new PlayerSaveData();
		saveData.data = data;
		saveData.nameList = nameList;

		string jsonData = JsonUtility.ToJson(saveData);
		File.WriteAllText(defaultPlayerDataPath, jsonData);
	}

	private void SaveTileData()
	{
		var data = TilePacketData.Create(TileDataContainer.Instance.tileItems);

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
