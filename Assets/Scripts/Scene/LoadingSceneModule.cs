using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneModule : SceneModuleBase
{
	public override IEnumerator OnPrepareEnter()
	{
		yield return base.OnPrepareEnter();
		yield return ResourceManager.Instance.PreLoadItemData();
		yield return ResourceManager.Instance.PreLoadFieldItemObject();
		yield return ResourceManager.Instance.PreLoadCharacter();
		yield return UIManager.Instance.PreLoadPopup();
		yield return ObjectCameraManager.Instance.PreLoadObjectCamera();
	}


	public override void OnEnter()
	{
		SceneManager.Instance.LoadScene(SceneType.Login);
	}

	public override void OnExit()
	{

	}
}
