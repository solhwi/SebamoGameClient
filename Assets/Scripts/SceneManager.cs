using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
	Login = 0,
	Game = 1,
}

public class SceneManager : Singleton<SceneManager>
{
	[SerializeField] private float minLoadTime = 1.0f;

	public override bool IsDestroyOnLoad => false;

	private Coroutine loadCoroutine = null;

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
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLoadSceneCompleted;

		var loadProcess = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(type.ToString());
		if (loadProcess == null)
			yield break;

		loadProcess.allowSceneActivation = false;

		UIManager.Instance.TryOpen(PopupType.Wait, new WaitingPopup.Parameter("게임 로딩 중"));

		float t = 0.0f;
		while (loadProcess.isDone == false)
		{
			t += Time.deltaTime;
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

		while (barrierFunc != null && barrierFunc() == false)
		{
			t += Time.deltaTime;
			yield return null;
		}

		while (t < minLoadTime)
		{
			t += Time.deltaTime;
			yield return null;
		}

		UIManager.Instance.Close(PopupType.Wait);

		loadProcess.allowSceneActivation = true;

		loadCoroutine = null;
	}
}
