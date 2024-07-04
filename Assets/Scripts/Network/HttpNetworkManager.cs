using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

public class HttpNetworkManager : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Inventory inventory;

	private string BaseURL = "http://localhost:8001/";

	public async Task<MyPlayerPacketData> TryGetMyPlayerData()
	{
		var data = await TryGet<MyPlayerPacketData>();

		playerDataContainer.SetMyPacketData(data);
		inventory.SetMyPacketData(data);

		return data;
	}

	public async Task<PlayerPacketData[]> TryGetOtherPlayerDatas()
	{
		var otherDatas = await TryGet<PlayerPacketData[]>();

		playerDataContainer.SetOtherPacketData(otherDatas);

		return otherDatas;
	}

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
