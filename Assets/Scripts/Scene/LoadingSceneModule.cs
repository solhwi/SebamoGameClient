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

		SoundManager.Instance.PlaySFX(SoundManager.SFXType.Start);
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

			yield return preLoadPopup.FadeInRoutine(LogoType.Sebamo);

			yield return ResourceManager.Instance.DownLoadAssets((p) =>
			{
				preLoadPopup.SetDescription("리소스 다운로드 중", p * downLoadSize);
			});
		}
		else
		{
			yield return preLoadPopup.FadeInRoutine(LogoType.Sebamo);
		}

		preLoadPopup.SetWaitDescription("리소스 준비 중");

		yield return ResourceManager.Instance.PreLoadAssets();

		preLoadPopup.SetWaitDescription("리소스 캐싱 중");

		yield return BoardGameManager.Instance.PreLoadCharacter();
		yield return UIManager.Instance.PreLoadPopup();
		yield return ObjectCameraManager.Instance.PreLoadObjectCamera();
		yield return ObjectManager.Instance.PreInstantiateFieldItemObject();

		preLoadPopup.SetDescription("로딩 완료, 즐거운 게임 되십시오.");
	}

	public override void OnEnter()
	{
		SceneManager.Instance.LoadScene(SceneType.Login);
	}
}
