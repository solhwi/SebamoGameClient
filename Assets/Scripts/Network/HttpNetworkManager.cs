using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

public enum GroupType
{
	None = -1,
	Kahlua = 0,
	Exp = 1,
}

public class HttpNetworkManager : Singleton<HttpNetworkManager>
{
	[SerializeField] private string BaseURL = "http://localhost:8001";

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Inventory inventory;
	[SerializeField] private TileDataContainer tileDataContainer;

	[SerializeField] private float updateFrequency = 60.0f;

	[SerializeField] private bool isOnNetworkMode = false;

	public override bool IsDestroyOnLoad => false;

	public bool IsLoaded { get; private set; }
	private float t = 0.0f;

	private async void Start()
	{
		if (isOnNetworkMode)
		{
			IsLoaded = false;
			await TryGetAll();
		}

		IsLoaded = true;
	}

	private async void Update()
	{
		if (isOnNetworkMode)
		{
			t += Time.deltaTime;
			if (t > updateFrequency)
			{
				t = 0.0f;
				await TryGetOtherPlayerDatas();
				await TryGetTileData();
			}
		}
	}

	private async Task<bool> TryGetAll()
	{
		bool b = await TryGetMyPlayerData();
		b &= await TryGetOtherPlayerDatas();
		b &= await TryGetTileData();

		return b;
	}

	// 최초 1회
	private async Task<bool> TryGetMyPlayerData()
	{
		var data = await TryGet<MyPlayerPacketData>("My");
		if (data == null)
			return false;

		playerDataContainer.SetMyPacketData(data);
		return true;
	}

	// 주기적으로 다른 플레이어 정보 가져옴
	public async Task<bool> TryGetOtherPlayerDatas()
	{
		var otherDatas = await TryGet<PlayerPacketDataCollection>("Other");
		if (otherDatas == null) 
			return false;

		playerDataContainer.SetOtherPacketData(otherDatas.playerDatas);
		return true;
	}

	// 자신의 정보가 변경될 때마다 업데이트쳐줌
	public async Task<bool> TryPostMyPlayerData()
	{
		if (isOnNetworkMode == false)
			return true;

		var sendData = MakePlayerPacketData();
		var receiveData = await TryPost<MyPlayerPacketData>(sendData);
		if (receiveData == null)
			return false;

		playerDataContainer.SetMyPacketData(receiveData);
		return true;
	}

	public async Task<bool> TryGetTileData()
	{
		if (isOnNetworkMode == false)
			return true;

		var receiveData = await TryGet<TilePacketData>("Tile");
		if (receiveData == null)
			return false;

		tileDataContainer.SetTileItemPacket(receiveData);
		return true;
	}

	public async Task<bool> TryPostTileData()
	{
		if (isOnNetworkMode == false)
			return true;

		var sendData = MakeTilePacketData();
		var receiveData = await TryPost<TilePacketData>(sendData);
		if (receiveData == null)
			return false;

		tileDataContainer.SetTileItemPacket(receiveData);
		return true;
	}

	private MyPlayerPacketData MakePlayerPacketData()
	{
		var data = new MyPlayerPacketData();
		data.playerData = new PlayerPacketData();

		data.playerData.playerName = playerDataContainer.playerName;
		data.playerData.playerGroup = playerDataContainer.playerGroup;
		data.playerData.hasDiceCount = playerDataContainer.hasDiceCount;
		data.playerData.playerTileOrder = playerDataContainer.currentTileOrder;

		data.playerData.equippedItems = playerDataContainer.equippedItems;
		data.playerData.appliedProfileItems = playerDataContainer.appliedProfileItems;

		data.hasItems = inventory.hasItems.Keys.ToArray();
		data.hasItemCounts = inventory.hasItems.Values.ToArray();
		data.appliedBuffItems = inventory.appliedBuffItems.ToArray();

		return data;
	}

	private TilePacketData MakeTilePacketData()
	{
		var data = new TilePacketData();

		List<int> indexes = new List<int>();
		List<string> itemCodes = new List<string>();

		for (int i = 0; i < tileDataContainer.tileItems.Length; i++)
		{
			string itemCode = tileDataContainer.tileItems[i];
			if (itemCode == null || itemCode == string.Empty)
				continue;

			indexes.Add(i);
			itemCodes.Add(itemCode);
		}

		data.tileItemIndexes = indexes.ToArray();
		data.tileItemCodes = itemCodes.ToArray();

		return data;
	}

	public async Task<T> TryGet<T>(string urlParameter)
	{
		string group = playerDataContainer.playerGroup;
		string name = playerDataContainer.playerName;

		string responseData = await GetRoutine($"{BaseURL}/{group}?p1={name}&p2={urlParameter}");
		if (responseData == null)
			return default;

		return JsonUtility.FromJson<T>(responseData);
	}

	public async Task<T> TryPost<T>(PacketData requestData)
	{
		string requestJsonData = JsonUtility.ToJson(requestData);

		string responseJsonData = await PostRoutine(BaseURL, requestJsonData);

		return JsonUtility.FromJson<T>(responseJsonData);
	}

	private Task<string> PostRoutine(string url, string data)
	{
		UnityWebRequest www = UnityWebRequest.PostWwwForm(url, data);

		byte[] jsonToSend = new UTF8Encoding().GetBytes(data);
		www.uploadHandler = new UploadHandlerRaw(jsonToSend);

		//json 헤더 추가
		www.SetRequestHeader("Content-Type", "application/json");

		return OnRequest(www);
	}

	private Task<string> GetRoutine(string url)
	{
		UnityWebRequest www = UnityWebRequest.Get(url);
		return OnRequest(www);
	}

	private async Task<string> OnRequest(UnityWebRequest request)
	{
		await request.SendWebRequest();

		if (request.error == null)
		{
			return request.downloadHandler.text;
		}
		else
		{
			return request.error;
		}
	}

#if UNITY_EDITOR

	[ContextMenu("페이크 패킷 데이터로 세팅")]
	public void SetFakePacketData()
	{
		var data = MakeMyFakeFacketData();

		playerDataContainer.SetMyPacketData(data);

		var otherData = MakeOtherFakePacketData();

		playerDataContainer.SetOtherPacketData(otherData.playerDatas);
		
		EditorUtility.SetDirty(playerDataContainer);
		EditorUtility.SetDirty(inventory);

		AssetDatabase.SaveAssetIfDirty(playerDataContainer);
		AssetDatabase.SaveAssetIfDirty(inventory);
	}

	private MyPlayerPacketData MakeMyFakeFacketData()
	{
		var data = new MyPlayerPacketData();
		data.playerData = new PlayerPacketData();

		data.playerData.playerGroup = GroupType.Exp.ToString();
		data.playerData.playerName = "솔휘";

		data.playerData.hasDiceCount = 3;
		data.playerData.playerTileOrder = 0;

		data.playerData.equippedItems = new string[6] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword" };
		data.playerData.appliedProfileItems = new string[2] { "", "" };

		data.playerData.profileComment = "안녕하세요.";

		data.hasItems = new string[8] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword", "TwinDagger", "Coin" };
		data.hasItemCounts = new int[8] { 1, 1, 1, 1, 1, 1, 1, 100000 };
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
			newData.appliedProfileItems = new string[2] { "", "" };

			otherDatas.Add(newData);
		}

		collection.playerDatas = otherDatas.ToArray();
		return collection;
	}
#endif
}
