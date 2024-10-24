
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
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

	[SerializeField] private int reconnectCount = 5;
	[SerializeField] private float updateFrequency = 60.0f;

	[HideInInspector] public bool IsConnected = false;

	[SerializeField] public bool isOfflineMode = false;

	private float t = 0.0f;

	private Coroutine connectCoroutine = null;
	private Coroutine updateCoroutine = null;

	public void TryConnect(string group, string name)
	{
		playerDataContainer.playerGroup = group;
		playerDataContainer.playerName = name;

		if (isOfflineMode)
		{
			IsConnected = true;
		}
		else
		{
			if (connectCoroutine != null)
			{
				StopCoroutine(connectCoroutine);
			}

			if (updateCoroutine != null)
			{
				StopCoroutine(updateCoroutine);
			}

			connectCoroutine = StartCoroutine(TryGetAll(() =>
			{
				updateCoroutine = StartCoroutine(OnUpdate());
			}));
		}
	}

	private IEnumerator OnUpdate()
	{
		if (!isOfflineMode && IsConnected)
		{
			t += Time.deltaTime;
			if (t > updateFrequency)
			{
				t = 0.0f;

				yield return TryGetOtherPlayerDatas(null, null);
				yield return TryGetTileData(null, null);
			}
		}
	}

	private IEnumerator TryGetAll(Action onGetAll)
	{
		bool isSuccess = false;

		yield return TryGetMyPlayerData((d) => { isSuccess = true; }, (s) => { isSuccess = false; });
		if (isSuccess == false)
			yield break;

		yield return TryGetOtherPlayerDatas((d) => { isSuccess = true; }, (s) => { isSuccess = false; });
		if (isSuccess == false)
			yield break;

		yield return TryGetTileData((d) => { isSuccess = true; }, (s) => { isSuccess = false; });
		if (isSuccess == false)
			yield break;

		IsConnected = true;
		onGetAll?.Invoke();
	}

	private void OnDestroy()
	{
		if (connectCoroutine != null)
		{
			StopCoroutine(connectCoroutine);
		}

		if (updateCoroutine != null)
		{
			StopCoroutine(updateCoroutine);
		}
	}

	// 최초 1회
	private IEnumerator TryGetMyPlayerData(Action<MyPlayerPacketData> onSuccess, Action<string> onFailed = null)
	{
		yield return TryGet<MyPlayerPacketData>("My", (data) =>
		{
			playerDataContainer.SetMyPacketData(data);
			onSuccess?.Invoke(data);
		}, onFailed);
	}

	// 주기적으로 다른 플레이어 정보 가져옴
	public IEnumerator TryGetOtherPlayerDatas(Action<PlayerPacketData[]> onSuccess, Action<string> onFailed = null)
	{
		if (isOfflineMode)
		{
			onSuccess?.Invoke(playerDataContainer.otherPlayerPacketDatas.ToArray());
			yield break;
		}

		yield return TryGet<PlayerPacketDataCollection>("Other", (otherDatas) =>
		{
			playerDataContainer.SetOtherPacketData(otherDatas.playerDatas);
			onSuccess?.Invoke(otherDatas.playerDatas);
		}, onFailed);
	}

	// 자신의 정보가 변경될 때마다 업데이트쳐줌
	public IEnumerator TryPostMyPlayerData(Action<MyPlayerPacketData> onSuccess, Action<string> onFailed = null)
	{
		if (isOfflineMode)
		{
			onSuccess?.Invoke(MakePlayerPacketData());
			yield break;
		}

		if (IsConnected == false)
			yield break;

		var sendData = MakePlayerPacketData();
		yield return TryPost<MyPlayerPacketData>(sendData, (receiveData) =>
		{
			playerDataContainer.SetMyPacketData(receiveData);
			onSuccess?.Invoke(receiveData);
		}, onFailed);
	}

	public IEnumerator TryGetTileData(Action<TilePacketData> onSuccess, Action<string> onFailed = null)
	{
		if (isOfflineMode)
		{
			onSuccess?.Invoke(MakeTilePacketData());
			yield break;
		}

		yield return TryGet<TilePacketData>("Tile", (receiveData) =>
		{
			tileDataContainer.SetTileItemPacketData(receiveData);
			onSuccess?.Invoke(receiveData);
		}, onFailed);
	}

	public IEnumerator TryPostTileData(Action<TilePacketData> onSuccess, Action<string> onFailed = null)
	{
		if (isOfflineMode)
		{
			onSuccess?.Invoke(MakeTilePacketData());
			yield break;
		}

		if (IsConnected == false)
			yield break;

		var sendData = MakeTilePacketData();
		yield return TryPost<TilePacketData>(sendData, (receiveData) =>
		{
			tileDataContainer.SetTileItemPacketData(receiveData);
			onSuccess?.Invoke(receiveData);
		}, onFailed);
	}

	private MyPlayerPacketData MakePlayerPacketData()
	{
		return MyPlayerPacketData.Create(playerDataContainer, inventory);
	}

	private TilePacketData MakeTilePacketData()
	{
		return TilePacketData.Create(tileDataContainer.tileItems);
	}

	public IEnumerator TryGet<T>(string urlParameter, Action<T> onGetSuccess, Action<string> OnGetFailed)
	{
		string group = playerDataContainer.playerGroup;
		string name = playerDataContainer.playerName;

		yield return GetRoutine($"{BaseURL}/{group}?p1={name}&p2={urlParameter}", (responseJsonData) =>
		{
			try
			{
				if (responseJsonData != null)
				{
					T responseData = JsonUtility.FromJson<T>(responseJsonData);

					IsConnected = true;

					onGetSuccess?.Invoke(responseData);
				}
			}
			catch (Exception e)
			{
				IsConnected = false;

				Debug.LogError(e.ToString());
				OnGetFailed?.Invoke(e.ToString());
			}
		});
	}

	public IEnumerator TryPost<T>(PacketData requestData, Action<T> onGetSuccess, Action<string> OnGetFailed)
	{
		if (IsConnected == false)
			yield break;

		string requestJsonData = JsonUtility.ToJson(requestData);

		yield return PostRoutine(BaseURL, requestJsonData, (responseJsonData) =>
		{
			try
			{
				if (responseJsonData != null)
				{
					T responseData = JsonUtility.FromJson<T>(responseJsonData);

					IsConnected = true;

					onGetSuccess?.Invoke(responseData);
				}
			}
			catch (Exception e)
			{
				IsConnected = false;

				Debug.LogError(e.ToString());
				OnGetFailed?.Invoke(e.ToString());
			}
		});
	}

	private IEnumerator PostRoutine(string url, string data, Action<string> onGet)
	{
		UnityWebRequest www = UnityWebRequest.PostWwwForm(url, data);

		byte[] jsonToSend = new UTF8Encoding().GetBytes(data);
		www.uploadHandler = new UploadHandlerRaw(jsonToSend);

		//json 헤더 추가
		www.SetRequestHeader("Content-Type", "application/json");
		yield return OnRequest(www, onGet);
	}

	private IEnumerator GetRoutine(string url, Action<string> onGet)
	{
		UnityWebRequest www = UnityWebRequest.Get(url);
		yield return OnRequest(www, onGet);
	}

	private IEnumerator OnRequest(UnityWebRequest www, Action<string> onGet)
	{
		for (int i = 0; i < reconnectCount; i++)
		{
			yield return www.SendWebRequest();

			if (www.error == null)
			{
				if (UIManager.Instance != null)
				{
					UIManager.Instance.Close(PopupType.Wait);
				}

				onGet?.Invoke(www.downloadHandler.text);
				break;
			}
			else
			{
				if (UIManager.Instance != null)
				{
					UIManager.Instance.TryOpen(PopupType.Wait, new WaitingPopup.Parameter(WaitingPopup.Type.Network));
				}
			}
		}

		onGet?.Invoke(www.error);
	}
}
