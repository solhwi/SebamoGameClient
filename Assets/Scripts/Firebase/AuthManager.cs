using FirebaseWebGL.Scripts.FirebaseBridge;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : Singleton<AuthManager>
{
	[SerializeField] private string guestEmailAddress = string.Empty;

	private Action<string> onLoginSuccess;
	private Action<string> onLoginFailed;

	public void TryLogin(Action<string> onSuccess, Action<string> onFailed)
	{
		onLoginSuccess = onSuccess;
		onLoginFailed = onFailed;

#if UNITY_EDITOR
		OnLoginSuccess(guestEmailAddress);
#elif UNITY_WEBGL
		FirebaseAuth.SignInWithGoogle(gameObject.name, "OnLoginSuccess", "OnLoginFailed");
#endif
	}

	public void OnLoginSuccess(string data)
	{
		onLoginSuccess?.Invoke(data);
		onLoginSuccess = null;
		onLoginFailed = null;
	}

	public void OnLoginFailed(string error)
	{
		onLoginFailed?.Invoke(error);
		onLoginFailed = null;
		onLoginSuccess = null;
	}

	public void OnAuthStateChanged(string data)
	{
		Debug.Log($"{data}");
	}

	public void OnAuthStateChangeFailed(string error)
	{
		Debug.LogError($"{error}");
	}
}
