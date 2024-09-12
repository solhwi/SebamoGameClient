using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginCanvas : MonoBehaviour
{
    public void OnClickLogin()
	{
		HttpNetworkManager.Instance.TryConnect();
		SceneManager.Instance.LoadSceneAsync(SceneType.Game, IsConnected);
	}

	private bool IsConnected()
	{
		return HttpNetworkManager.Instance.IsConnected;
	}
}
