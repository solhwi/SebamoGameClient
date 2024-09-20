
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

	[SerializeField] private float updateFrequency = 60.0f;

	[SerializeField] private bool isOnNetworkMode = false;

	public override bool IsDestroyOnLoad => false;

	public bool IsConnected { get; private set; }
	private float t = 0.0f;

	private Coroutine connectCoroutine = null;
	private Coroutine updateCoroutine = null;

	public void TryConnect(string group, string name)
	{
		playerDataContainer.playerGroup = group;
		playerDataContainer.playerName = name;

		if (isOnNetworkMode)
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
		else
		{
			IsConnected = true;
		}
	}

	private IEnumerator OnUpdate()
	{
		if (isOnNetworkMode && IsConnected)
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
		if (isOnNetworkMode == false)
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
		if (isOnNetworkMode == false)
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
		if (isOnNetworkMode == false)
		{
			onSuccess?.Invoke(MakeTilePacketData());
			yield break;
		}

		yield return TryGet<TilePacketData>("Tile", (receiveData) =>
		{
			tileDataContainer.SetTileItemPacket(receiveData);
			onSuccess?.Invoke(receiveData);
		}, onFailed);
	}

	public IEnumerator TryPostTileData(Action<TilePacketData> onSuccess, Action<string> onFailed = null)
	{
		if (isOnNetworkMode == false)
		{
			onSuccess?.Invoke(MakeTilePacketData());
			yield break;
		}

		if (IsConnected == false)
			yield break;

		var sendData = MakeTilePacketData();
		yield return TryPost<TilePacketData>(sendData, (receiveData) =>
		{
			tileDataContainer.SetTileItemPacket(receiveData);
			onSuccess?.Invoke(receiveData);
		}, onFailed);
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
		if (UIManager.Instance != null)
		{
			UIManager.Instance.TryOpen(PopupType.Wait, new WaitingPopup.Parameter("서버에 접속 중"));
		}

		yield return www.SendWebRequest();

		if (www.error == null)
		{
			onGet?.Invoke(www.downloadHandler.text);
		}
		else
		{
			onGet?.Invoke(www.error);
		}

		UIManager.Instance.Close(PopupType.Wait);
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
