using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginCanvas : BoardGameCanvasBase
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private AuthDataTable authDataTable;

	[SerializeField] private GameObject currentGroupTextObj = null;
	[SerializeField] private TextMeshProUGUI currentGroupText = null;

	[SerializeField] private GameObject loginButtonobj = null;
	[SerializeField] private GameObject startButtonObj = null;

	private List<AuthData> authDataList = new List<AuthData>();
	private AuthData currentAuthData = null;

	protected override void OnOpen()
	{
		currentGroupTextObj.SetActive(false);
		startButtonObj.SetActive(false);
		loginButtonobj.SetActive(true);

		authDataList.Clear();
	}

	public void OnClickLogin()
	{
		AuthManager.Instance.TryLogin(OnLoginSuccess, null);
	}

	public void OnClickSelectGroup()
	{
		UIManager.Instance.TryOpen(PopupType.GroupSelect, new GroupSelectPopup.Param(authDataList, OnAuthSuccess));
	}


	private void OnAuthSuccess(AuthData authData)
	{
		currentAuthData = authData;

		currentGroupText.text = $"{authData.name}({authData.group})";

		loginButtonobj.SetActive(false);

		currentGroupTextObj.SetActive(true);
		startButtonObj.SetActive(true);
	}

	public void TryStartGame()
	{
		if (currentAuthData == null)
			return;

		HttpNetworkManager.Instance.TryConnect(currentAuthData.group, currentAuthData.name);
		SceneManager.Instance.LoadScene(SceneType.Game, IsConnected);
	}

	private void OnLoginSuccess(string address)
	{
		authDataList = authDataTable.GetAllAuthData(address).ToList();
		if (authDataList.Count > 1)
		{
			UIManager.Instance.TryOpen(PopupType.GroupSelect, new GroupSelectPopup.Param(authDataList, OnAuthSuccess));
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

	private bool IsConnected()
	{
		return HttpNetworkManager.Instance.IsConnected;
	}
}
