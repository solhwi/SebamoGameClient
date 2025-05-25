
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
	private string BaseURL
	{
		get
		{
			if (isLocalMode)
			{
				return "http://localhost:8001";
			}
			else
			{
				return "http://3.36.213.118:8001";
			}
		}
	}

	[SerializeField] private int reconnectCount = 5;
	[SerializeField] private float updateFrequency = 60.0f;

	[HideInInspector] public bool IsConnected = false;

	[SerializeField] public bool isOfflineMode = false;
	[SerializeField] public bool isLocalMode = false;

	private float t = 0.0f;

	private Coroutine connectCoroutine = null;
	private Coroutine updateCoroutine = null;

	public void TryConnect(string group, string name)
	{
		PlayerDataContainer.Instance.playerGroup = group;
		PlayerDataContainer.Instance.playerName = name;

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
			PlayerDataContainer.Instance.SetMyPacketData(data);
			onSuccess?.Invoke(data);
		}, onFailed);
	}

	// 주기적으로 다른 플레이어 정보 가져옴
	public IEnumerator TryGetOtherPlayerDatas(Action<PlayerPacketData[]> onSuccess, Action<string> onFailed = null)
	{
		if (isOfflineMode)
		{
			onSuccess?.Invoke(PlayerDataContainer.Instance.otherPlayerPacketDatas.ToArray());
			yield break;
		}

		yield return TryGet<PlayerPacketDataCollection>("Other", (otherDatas) =>
		{
			PlayerDataContainer.Instance.SetOtherPacketData(otherDatas.playerDatas);
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
			PlayerDataContainer.Instance.SetMyPacketData(receiveData);
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
			TileDataContainer.Instance.SetTileItemPacketData(receiveData);
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
			TileDataContainer.Instance.SetTileItemPacketData(receiveData);
			onSuccess?.Invoke(receiveData);
		}, onFailed);
	}

	private MyPlayerPacketData MakePlayerPacketData()
	{
		return MyPlayerPacketData.Create();
	}

	private TilePacketData MakeTilePacketData()
	{
		return TilePacketData.Create(TileDataContainer.Instance.tileItems);
	}

	public IEnumerator TryGet<T>(string urlParameter, Action<T> onGetSuccess, Action<string> OnGetFailed)
	{
		string group = PlayerDataContainer.Instance.playerGroup;
		string name = PlayerDataContainer.Instance.playerName;

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

	private IEnumerator PostRoutine(string url, string data, Action<string> onPost)
	{
		for (int i = 0; i < reconnectCount; i++)
		{
			var www = MakeRequest(url, data, onPost);
			yield return www.SendWebRequest();

			if (www.error == null)
			{
				if (UIManager.Instance != null)
				{
					UIManager.Instance.TryCloseWaitPopup(WaitingPopup.Type.Network);
				}

				onPost?.Invoke(www.downloadHandler.text);
				break;
			}
			else if (i == reconnectCount)
			{
				onPost?.Invoke(www.error);
				break;
			}
			else
			{
				if (UIManager.Instance != null)
				{
					UIManager.Instance.TryOpenWaitPopup(new WaitingPopup.Parameter(WaitingPopup.Type.Network));
				}
			}
		}
	}

	private UnityWebRequest MakeRequest(string url, string data, Action<string> onGet)
	{
		UnityWebRequest www = UnityWebRequest.PostWwwForm(url, data);

		byte[] jsonToSend = new UTF8Encoding().GetBytes(data);
		www.uploadHandler = new UploadHandlerRaw(jsonToSend);

		//json 헤더 추가
		www.SetRequestHeader("Content-Type", "application/json");
		return www;
	}

	private IEnumerator GetRoutine(string url, Action<string> onGet)
	{
		for (int i = 0; i < reconnectCount; i++)
		{
			var www = UnityWebRequest.Get(url);
			yield return www.SendWebRequest();

			if (www.error == null)
			{
				if (UIManager.Instance != null)
				{
					UIManager.Instance.TryCloseWaitPopup(WaitingPopup.Type.Network);
				}

				onGet?.Invoke(www.downloadHandler.text);
				break;
			}
			else if (i == reconnectCount)
			{
				onGet?.Invoke(www.error);
				break;
			}
			else
			{
				if (UIManager.Instance != null)
				{
					UIManager.Instance.TryOpenWaitPopup(new WaitingPopup.Parameter(WaitingPopup.Type.Network));
				}
			}
		}

	}
}
