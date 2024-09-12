using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginCanvas : MonoBehaviour
{
    public void OnClickLogin()
	{
		SceneManager.Instance.LoadSceneAsync(SceneType.Game);
	}
}
