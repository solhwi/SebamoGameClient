using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

public class HttpNetworkManager : MonoBehaviour
{
	[SerializeField] private string BaseURL = "http://localhost:8001/";

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Inventory inventory;

	[SerializeField] private float updateFrequency = 60.0f;

	private float t = 0.0f;

	//private async void Start()
	//{
	//	await TryGetMyPlayerData();
	//}
	
	//private async void Update()
	//{
	//	t += Time.deltaTime;
	//	if (t > updateFrequency)
	//	{
	//		await TryGetOtherPlayerDatas();
	//		t = 0.0f;
	//	}
	//}

	// 최초 1회
	private async Task TryGetMyPlayerData()
	{
		var data = await TryGet<MyPlayerPacketData>();

		playerDataContainer.SetMyPacketData(data);
		inventory.SetMyPacketData(data);
	}

	// 주기적으로 다른 플레이어 정보 가져옴
	private async Task TryGetOtherPlayerDatas()
	{
		var otherDatas = await TryGet<PlayerPacketData[]>();

		playerDataContainer.SetOtherPacketData(otherDatas);
	}

	// 자신의 정보가 변경될 때마다 업데이트쳐줌
	public async Task<MyPlayerPacketData> TryPostMyPlayerData()
	{
		var sendData = MakePlayerPacketData();
		var receiveData = await TryPost<MyPlayerPacketData>(sendData);

		playerDataContainer.SetMyPacketData(receiveData);
		inventory.SetMyPacketData(receiveData);

		return receiveData;
	}

	private MyPlayerPacketData MakePlayerPacketData()
	{
		return new MyPlayerPacketData();
	}

	public async Task<T> TryGet<T>()
	{
		string responseData = await GetRoutine(BaseURL);
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

}
