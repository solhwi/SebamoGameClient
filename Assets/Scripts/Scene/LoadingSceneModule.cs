using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadingSceneModule : SceneModuleBase
{
	[SerializeField] private bool isDownLoadAsset = false;

	private IEnumerator Start()
	{
		yield return OnPrepareEnter();
		OnEnter();
	}

	public override IEnumerator OnPrepareEnter()
	{
		PlayerConfig.Load();

		yield return UIManager.Instance.PreLoadByResources();

		UIManager.Instance.TryOpen(PopupType.PreLoading);

		PreLoadingPopup preLoadPopup = null;
		
		while (preLoadPopup == null) 
		{
			preLoadPopup = UIManager.Instance.GetPopup<PreLoadingPopup>(PopupType.PreLoading);
		}

		yield return preLoadPopup.FadeRoutine(LogoType.UnityChan);

		if (isDownLoadAsset)
		{
			float downLoadSize = 0.0f;

			yield return ResourceManager.Instance.GetDownLoadSize((size) =>
			{
				downLoadSize = size;
			});

			bool isDownLoadAsset = false;
			UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter($"{downLoadSize}MB 만큼의 추가 리소스를 다운로드합니다.", onClickConfirm: () =>
			{
				isDownLoadAsset = true;
			}));

			while (isDownLoadAsset == false)
			{
				yield return null;
			}

			StartCoroutine(preLoadPopup.FadeInRoutine(LogoType.Sebamo));

			yield return ResourceManager.Instance.DownLoadAssets((p) =>
			{
				preLoadPopup.SetDescription("Downloading Resources", p * downLoadSize);
			});
		}
		else
		{
			StartCoroutine(preLoadPopup.FadeInRoutine(LogoType.Sebamo));
		}

		yield return null;

		preLoadPopup.SetWaitDescription("Preparing Resources");

		yield return ResourceManager.Instance.PreLoadAssets();

		preLoadPopup.SetWaitDescription("Caching Resources");

		yield return SoundManager.Instance.PreLoadSound();
		yield return BoardGameManager.Instance.PreLoadCharacter();
		yield return UIManager.Instance.PreLoadPopup();
		yield return ObjectCameraManager.Instance.PreLoadObjectCamera();
		yield return ObjectManager.Instance.PreInstantiateFieldItemObject();

		preLoadPopup.SetDescription("Load Complete, Enjoy Your Game");
	}

	public override void OnEnter()
	{
		SceneManager.Instance.LoadScene(SceneType.Login);
	}
}
