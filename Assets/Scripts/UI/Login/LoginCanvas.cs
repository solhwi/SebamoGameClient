using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LoginCanvas : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private AuthDataTable authDataTable;

	[SerializeField] private GameObject groupPopupObj = null;
	[SerializeField] private ScrollContent groupScrollContent = null;

	private List<AuthData> authGroupList = new List<AuthData>();

	private void Awake()
	{
		groupPopupObj.SetActive(false);
		authGroupList.Clear();

		groupScrollContent.onUpdateContents += OnUpdateContents;
		groupScrollContent.onGetItemCount += OnGetItemCount;
	}

	public void OnClickLogin()
	{
		AuthManager.Instance.TryLogin(OnLoginSuccess, null);
	}

	private void OnUpdateContents(int index, GameObject contentsObj)
	{
		if (index < 0 || authGroupList.Count <= index)
			return;

		if (contentsObj == null)
			return;

		var groupSelectScrollItem = contentsObj.GetComponent<GroupSelectScrollItem>();
		if (groupSelectScrollItem == null)
			return;

		var authGroup = authGroupList[index];
		if (authGroup == null)
			return;

		groupSelectScrollItem.SetData(authGroup);
		groupSelectScrollItem.SetItemClick(TryConnect);
	}

	private void TryConnect(AuthData authData)
	{
		HttpNetworkManager.Instance.TryConnect(authData.group, authData.name);
		SceneManager.Instance.LoadSceneAsync(SceneType.Game, IsConnected);
	}

	private int OnGetItemCount(int tabType)
	{
		return authGroupList.Count;
	}

	private void OnLoginSuccess(string address)
	{
		string group = string.Empty;
		string name = string.Empty;

		authGroupList = authDataTable.GetAllAuthData(address).ToList();
		if (authGroupList.Count > 1)
		{
			groupPopupObj.SetActive(true);
			groupScrollContent.UpdateContents();
		}
		else if (authGroupList.Count > 0)
		{
			var authData = authGroupList.FirstOrDefault();
			if (authData == null)
				return;

			TryConnect(authData);
		}
		else
		{
			Debug.LogError($"다음 메일 주소 {address}는 등록되지 않은 계정입니다.");
		}
	}

	private KeyValuePair<string, string> GetGroupAndName(string address)
	{
		var authDatas = authDataTable.GetAllAuthData(address);
		if (authDatas == null)
			return default;

		var authData = authDatas.LastOrDefault();
		if (authData == null)
			return default;

		return new KeyValuePair<string, string>(authData.group, authData.name);
	}

	private bool IsConnected()
	{
		return HttpNetworkManager.Instance.IsConnected;
	}
}
