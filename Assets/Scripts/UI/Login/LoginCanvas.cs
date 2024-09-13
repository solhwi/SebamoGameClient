using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoginCanvas : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private AuthDataTable authDataTable;

    public void OnClickLogin()
	{
		AuthManager.Instance.TryLogin(OnLoginSuccess, null);
	}

	private void OnLoginSuccess(string address)
	{
		// 1. data를 파싱하여 그룹이 있는 지를 확인한다.
		// 2. 그룹이 있다면 UI를 띄우고, 아니라면 데이터 세팅 후 하위 로직 진행
		// 2-1. UI에서 클릭한 그룹으로 데이터 세팅 후 하위 로직 진행

		var data = GetGroupAndName(address);

		HttpNetworkManager.Instance.TryConnect(data.Key, data.Value);
		SceneManager.Instance.LoadSceneAsync(SceneType.Game, IsConnected);
	}

	private KeyValuePair<string, string> GetGroupAndName(string address)
	{
		var authData = authDataTable.GetAuthData(address);
		if (authData == null)
			return default;

		return authData.LastOrDefault();
	}

	private bool IsConnected()
	{
		return HttpNetworkManager.Instance.IsConnected;
	}
}
