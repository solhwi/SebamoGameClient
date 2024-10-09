using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneModule : SceneModuleBase
{
	public override IEnumerator OnPrepareEnter()
	{
		yield return base.OnPrepareEnter();

		SoundManager.Instance.PlayBGM(SoundManager.BGMType.Login, false);

		var preLoadPopup = UIManager.Instance.GetPopup<PreLoadingPopup>(PopupType.PreLoading);
		if (preLoadPopup != null)
		{
			yield return preLoadPopup.FadeOutRoutine(LogoType.All);
			UIManager.Instance.Close(PopupType.PreLoading);
		}

	}
}
