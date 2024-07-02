using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerPacketData : PacketData
{
	public PlayerSyncPacketData syncData;

	public string[] hasItems; // 가진 아이템
	public string[] appliedBuffItems; // 적용 중인 버프 아이템
}

[System.Serializable]
public class PlayerSyncPacketData : PacketData
{
	public int playerTileIndex; // 현재 타일 인덱스
	public int hasDiceCount; // 가진 주사위 개수

	public string[] equippedItems; // 장착 중인 아이템

}

[System.Serializable]
public class PacketData
{ 

}

public class HttpNetworkManager : MonoBehaviour
{
	private string BaseURL = "http://localhost:8000/";

	public async Task<PlayerPacketData> TryGetMyPlayerData()
	{
		return await TryGet<PlayerPacketData>();
	}

	public async Task<PlayerSyncPacketData[]> TryGetOtherPlayerDatas()
	{
		return await TryGet<PlayerSyncPacketData[]>();
	}

	public async Task<PlayerPacketData> TryPostMyPlayerData(PlayerPacketData data)
	{
		return await TryPost<PlayerPacketData>(data);
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
