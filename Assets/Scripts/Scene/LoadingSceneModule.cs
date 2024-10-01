using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadingSceneModule : SceneModuleBase
{
	[SerializeField] private float loadingCompleteWaitTime = 3.0f;
	[SerializeField] private bool bDownLoadAsset = false;

	private bool isLoaded = false;

	private IEnumerator Start()
	{
		yield return OnPrepareEnter();
		OnEnter();
	}

	public override IEnumerator OnPrepareEnter()
	{
		isLoaded = false;

		if (boardGameCanvas is LoadingCanvas loadingCanvas == false)
			yield break;

		loadingCanvas.OnEnter();

		loadingCanvas.barrierFunc -= IsLoaded;
		loadingCanvas.barrierFunc += IsLoaded;

		if (bDownLoadAsset)
		{
			loadingCanvas.SetWaitDescription("리소스 다운로드 중");
			yield return ResourceManager.Instance.DownLoadAssets();
		}

		loadingCanvas.SetWaitDescription("리소스 준비 중");

		yield return ResourceManager.Instance.PreLoadAssets();

		loadingCanvas.SetWaitDescription("리소스 캐싱 중");

		yield return BoardGameManager.Instance.PreLoadCharacter();
		yield return UIManager.Instance.PreLoadPopup();
		yield return ObjectCameraManager.Instance.PreLoadObjectCamera();
		yield return ObjectManager.Instance.PreInstantiateFieldItemObject();

		isLoaded = true;

		loadingCanvas.SetDescription("로딩 완료, 즐거운 게임 되십시오.");

		yield return new WaitForSeconds(loadingCompleteWaitTime);
	}

	private bool IsLoaded()
	{
		return isLoaded;
	}


	public override void OnEnter()
	{
		SceneManager.Instance.LoadScene(SceneType.Login, false);
	}
}
