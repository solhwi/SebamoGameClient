using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
	Login = 0,
	Game = 1,
	Loading = 2,
}

public class SceneManager : Singleton<SceneManager>
{
	[SerializeField] private float minLoadTime = 1.0f;

	private Coroutine loadCoroutine = null;

	protected override void OnAwakeInstance()
	{
		base.OnAwakeInstance();

		var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		OnLoadSceneCompleted(currentScene, LoadSceneMode.Single);
	}

	public void LoadScene(SceneType type, Func<bool> barrierFunc = null)
	{
		if (loadCoroutine != null)
			return;

		loadCoroutine = StartCoroutine(LoadSceneProcess(type, barrierFunc, OnLoading));
	}

	private void OnLoading(float progress)
	{
		// UI 처리
	}

	private void OnLoadSceneCompleted(Scene scene, LoadSceneMode sceneMode)
	{
		if (Enum.TryParse<SceneType>(scene.name.ToString(), out var sceneType) == false)
			return;

		UIManager.Instance.OpenMainCanvas(sceneType);
	}

	private IEnumerator LoadSceneProcess(SceneType type, Func<bool> barrierFunc, Action<float> onProgress)
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLoadSceneCompleted;

		UIManager.Instance.TryOpen(PopupType.Wait, new WaitingPopup.Parameter("게임 로딩 중"));

		var loadProcess = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Loading");
		while (loadProcess.isDone == false)
		{
			yield return null;
		}

		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLoadSceneCompleted;

		loadProcess = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
		loadProcess.allowSceneActivation = false;

		float currentLoadTime = 0.0f;
		while (loadProcess.isDone == false)
		{
			currentLoadTime += Time.deltaTime;
			yield return null;

			if (loadProcess.progress > 0.89f)
			{
				onProgress?.Invoke(1.0f);
				break;
			}
			else
			{
				onProgress?.Invoke(loadProcess.progress);
			}
		}

		loadProcess.allowSceneActivation = true;

		while (!loadProcess.isDone || barrierFunc != null && barrierFunc() == false)
		{
			currentLoadTime += Time.deltaTime;
			yield return null;
		}

		// 최소 로드 시간을 채움
		while (currentLoadTime < minLoadTime)
		{
			currentLoadTime += Time.deltaTime;
			yield return null;
		}

		UIManager.Instance.Close(PopupType.Wait);
		loadCoroutine = null;
	}
}
