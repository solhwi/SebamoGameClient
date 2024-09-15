using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginCanvas : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private AuthDataTable authDataTable;

	[SerializeField] private GameObject groupPopupObj = null;
	[SerializeField] private ScrollContent groupScrollContent = null;

	[SerializeField] private Text currentNameText = null;
	[SerializeField] private GameObject greetingTextObj = null;
	[SerializeField] private GameObject clickTextObj = null;

	[SerializeField] private GameObject loginButtonobj = null;
	[SerializeField] private GameObject startButtonObj = null;

	private List<AuthData> authDataList = new List<AuthData>();
	private AuthData currentAuthData = null;


	private void Awake()
	{
		groupPopupObj.SetActive(false);
		currentNameText.gameObject.SetActive(false);
		greetingTextObj.SetActive(false);
		clickTextObj.SetActive(false);
		startButtonObj.SetActive(false);

		loginButtonobj.SetActive(true);

		authDataList.Clear();

		groupScrollContent.onUpdateContents += OnUpdateContents;
		groupScrollContent.onGetItemCount += OnGetItemCount;
	}

	public void OnClickLogin()
	{
		AuthManager.Instance.TryLogin(OnLoginSuccess, null);
	}

	private void OnUpdateContents(int index, GameObject contentsObj)
	{
		if (index < 0 || authDataList.Count <= index)
			return;

		if (contentsObj == null)
			return;

		var groupSelectScrollItem = contentsObj.GetComponent<GroupSelectScrollItem>();
		if (groupSelectScrollItem == null)
			return;

		var authGroup = authDataList[index];
		if (authGroup == null)
			return;

		groupSelectScrollItem.SetData(authGroup);
		groupSelectScrollItem.SetItemClick(OnAuthSuccess);
	}

	private void OnAuthSuccess(AuthData authData)
	{
		currentAuthData = authData;
		currentNameText.text = $"{authData.name}({authData.group}) 님,";

		groupPopupObj.SetActive(false);
		loginButtonobj.SetActive(false);

		currentNameText.gameObject.SetActive(true);
		greetingTextObj.SetActive(true);
		clickTextObj.SetActive(true);
		startButtonObj.SetActive(true);
	}

	public void TryStartGame()
	{
		if (currentAuthData == null)
			return;

		HttpNetworkManager.Instance.TryConnect(currentAuthData.group, currentAuthData.name);
		SceneManager.Instance.LoadSceneAsync(SceneType.Game, IsConnected);
	}

	private int OnGetItemCount(int tabType)
	{
		return authDataList.Count;
	}

	private void OnLoginSuccess(string address)
	{
		groupPopupObj.SetActive(false);

		authDataList = authDataTable.GetAllAuthData(address).ToList();
		if (authDataList.Count > 1)
		{
			groupPopupObj.SetActive(true);
			groupScrollContent.UpdateContents();
		}
		else if (authDataList.Count > 0)
		{
			var authData = authDataList.FirstOrDefault();
			if (authData == null)
				return;

			OnAuthSuccess(authData);
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
