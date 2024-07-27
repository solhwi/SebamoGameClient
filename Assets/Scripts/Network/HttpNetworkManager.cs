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

	[SerializeField] private float updateFrequency = 60.0f;

	[SerializeField] private bool isOnNetworkMode = false;

	[SerializeField] private GroupType playerGroup;
	[SerializeField] private string playerName;

	public bool IsLoaded { get; private set; }
	private float t = 0.0f;

	private async void Start()
	{
		if (isOnNetworkMode)
		{
			IsLoaded = false;
			await TryGetMyPlayerData();
			await TryGetOtherPlayerDatas();
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
			}
		}
	}

	// 최초 1회
	private async Task TryGetMyPlayerData()
	{
		var data = await TryGet<MyPlayerPacketData>("My");

		playerDataContainer.SetMyPacketData(data);
		inventory.SetMyPacketData(data);
	}

	// 주기적으로 다른 플레이어 정보 가져옴
	private async Task TryGetOtherPlayerDatas()
	{
		var otherDatas = await TryGet<PlayerPacketDataCollection>("Other");

		playerDataContainer.SetOtherPacketData(otherDatas);
	}

	// 자신의 정보가 변경될 때마다 업데이트쳐줌
	public async Task TryPostMyPlayerData()
	{
		if (isOnNetworkMode == false)
			return;

		var sendData = MakePlayerPacketData();
		var receiveData = await TryPost<MyPlayerPacketData>(sendData);

		playerDataContainer.SetMyPacketData(receiveData);
		inventory.SetMyPacketData(receiveData);
	}

	private MyPlayerPacketData MakePlayerPacketData()
	{
		var data = new MyPlayerPacketData();
		data.playerData = new PlayerPacketData();

		data.playerData.playerName = playerDataContainer.playerName;
		data.playerData.playerGroup = playerDataContainer.playerGroup;
		data.playerData.hasDiceCount = playerDataContainer.hasDiceCount;
		data.playerData.playerTileIndex = playerDataContainer.currentTileIndex;

		data.playerData.equippedItems = inventory.equippedItems;

		data.hasItems = inventory.hasItems.Keys.ToArray();
		data.hasItemCounts = inventory.hasItems.Values.ToArray();
		data.appliedBuffItems = inventory.appliedBuffItems.ToArray();

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
		var data = new MyPlayerPacketData();
		data.playerData = new PlayerPacketData();

		data.playerData.playerGroup = playerGroup.ToString();
		data.playerData.playerName = playerName;

		data.playerData.hasDiceCount = 3;
		data.playerData.playerTileIndex = 0;

		data.playerData.equippedItems = new string[6] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword"};
		data.hasItems = new string[8] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword", "TwinDagger", "Coin" };
		data.hasItemCounts = new int[8] { 1, 1, 1, 1, 1, 1, 1, 100000 };
		data.appliedBuffItems = new string[] { };
		
		playerDataContainer.SetMyPacketData(data);
		inventory.SetMyPacketData(data);
		playerDataContainer.SetOtherPacketData(null);
		
		EditorUtility.SetDirty(playerDataContainer);
		EditorUtility.SetDirty(inventory);

		AssetDatabase.SaveAssetIfDirty(playerDataContainer);
		AssetDatabase.SaveAssetIfDirty(inventory);
	}
#endif
}
