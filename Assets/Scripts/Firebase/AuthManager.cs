using FirebaseWebGL.Scripts.FirebaseBridge;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthManager : Singleton<AuthManager>
{
	[SerializeField] private string guestEmailAddress = string.Empty;

	private Action<string> onLoginSuccess;
	private Action<string> onLoginFailed;

	private void Start()
	{
#if !UNITY_EDITOR && UNITY_WEBGL
		FirebaseAuth.OnAuthStateChanged(gameObject.name, "OnAuthStateChanged", "OnAuthStateChangeFailed");
#endif
	}

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

	private void OnLoginSuccess(string data)
	{
		// js에서 data로 메일을 받아오고, 미리 매핑된 데이터와 대조하여 그룹, 이름을 가져온다.
		onLoginSuccess?.Invoke(data);
		onLoginSuccess = null;
		onLoginFailed = null;
	}

	private void OnLoginFailed(string error)
	{
		onLoginFailed?.Invoke(error);
		onLoginFailed = null;
		onLoginSuccess = null;
	}

	private void OnAuthStateChanged(string data)
	{
		Debug.Log($"{data}");
	}

	private void OnAuthStateChangeFailed(string error)
	{
		Debug.LogError($"{error}");
	}
}
