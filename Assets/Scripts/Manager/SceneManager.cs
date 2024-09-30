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

	private SceneModuleBase currentSceneModule = null;
	private Coroutine loadCoroutine = null;

	protected override void OnAwakeInstance()
	{
		base.OnAwakeInstance();

		var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		OnLoadSceneCompleted(currentScene, LoadSceneMode.Single);
	}

	public void LoadScene(SceneType type, bool bUseBackground, Func<bool> barrierFunc = null)
	{
		if (loadCoroutine != null)
			return;

		loadCoroutine = StartCoroutine(LoadSceneProcess(type, bUseBackground, barrierFunc, OnLoading));
	}

	private void OnLoading(float progress)
	{
		// UI 처리
	}

	private void OnLoadSceneCompleted(Scene scene, LoadSceneMode sceneMode)
	{
		currentSceneModule = FindAnyObjectByType<SceneModuleBase>();
	}

	private IEnumerator LoadSceneProcess(SceneType type, bool bUseBackground, Func<bool> barrierFunc, Action<float> onProgress)
	{
		if (currentSceneModule != null)
		{
			yield return currentSceneModule.OnPrepareExit();
			currentSceneModule.OnExit();
		}

		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLoadSceneCompleted;
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLoadSceneCompleted;

		UIManager.Instance.TryOpen(PopupType.Wait, new WaitingPopup.Parameter("게임 로딩 중", bUseBackground));

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

		// 최종 로드 시간 보정
		while (currentLoadTime < minLoadTime)
		{
			currentLoadTime += Time.deltaTime;
			yield return null;
		}

		UIManager.Instance.Close(PopupType.Wait);
		loadCoroutine = null;
	}
}
