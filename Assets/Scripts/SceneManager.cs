using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
	Login = 0,
	Game = 1,
}

public class SceneManager : Singleton<SceneManager>
{
	public override bool IsDestroyOnLoad => false;

	private Coroutine loadCoroutine = null;

    public void LoadSceneAsync(SceneType type)
	{
		if (loadCoroutine != null)
		{
			StopCoroutine(loadCoroutine);
		}

		loadCoroutine = StartCoroutine(LoadSceneProcess(type, OnLoading));
	}

	private void OnLoading(float progress)
	{
		// UI 처리
	}

	private IEnumerator LoadSceneProcess(SceneType type, Action<float> onProgress)
	{
		var loadProcess = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(type.ToString());
		if (loadProcess == null)
			yield break;

		loadProcess.allowSceneActivation = false;

		while (loadProcess.isDone == false)
		{
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
		loadCoroutine = null;
	}
}
