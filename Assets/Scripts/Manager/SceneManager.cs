using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
	Login = 0,
	Game = 1,
	Loading = 2,
}

[System.Serializable]
public class SceneLoadData
{
	public bool useWaitingPopup = false;
}


public class SceneManager : Singleton<SceneManager>
{
	[SerializeField] private float minLoadTime = 1.0f;

	[System.Serializable] public class SceneLoadDataDictionary : SerializableDictionary<SceneType, SceneLoadData> { }
	[SerializeField] private SceneLoadDataDictionary sceneLoadDataDictionary = new SceneLoadDataDictionary();

	private SceneModuleBase currentSceneModule = null;
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
		currentSceneModule = FindAnyObjectByType<SceneModuleBase>();
	}

	private IEnumerator LoadSceneProcess(SceneType type, Func<bool> barrierFunc, Action<float> onProgress)
	{
		if (currentSceneModule != null)
		{
			yield return currentSceneModule.OnPrepareExit();
			currentSceneModule.OnExit();
		}

		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLoadSceneCompleted;
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLoadSceneCompleted;

		if (sceneLoadDataDictionary.TryGetValue(type, out var sceneLoadData))
		{
			if (sceneLoadData.useWaitingPopup)
			{
				UIManager.Instance.TryOpen(PopupType.Wait, new WaitingPopup.Parameter(WaitingPopup.Type.Scene));
			}
		}

		var loadProcess = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(type.ToString());
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

		// 씬 로드 요청자의 방어 조건
		while (barrierFunc != null && barrierFunc() == false)
		{
			currentLoadTime += Time.deltaTime;
			yield return null;
		}

		// 로드 대기
		loadProcess.allowSceneActivation = true;
		while (loadProcess.isDone == false)
		{
			yield return null;
		}

		// 로드 이후 준비 작업
		while (currentSceneModule == null)
		{
			yield return null;
		}

		if (currentSceneModule != null)
		{
			yield return currentSceneModule.OnPrepareEnter();
			currentSceneModule.OnEnter();
		}

		// 최소 로드 시간 보장
		while (currentLoadTime < minLoadTime)
		{
			currentLoadTime += Time.deltaTime;
			yield return null;
		}

		UIManager.Instance.Close(PopupType.Wait);
		loadCoroutine = null;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/로딩 씬에서 플레이 시작")]
	public static void ResetLoadingScene()
	{
		UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/Scenes/{SceneType.Loading}.unity");
		EditorApplication.isPlaying = true;
	}
#endif
}
