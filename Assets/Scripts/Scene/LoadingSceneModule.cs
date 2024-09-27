using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneModule : SceneModuleBase
{
	public override IEnumerator OnPrepareEnter()
	{
		yield return base.OnPrepareEnter();
		yield return ResourceManager.Instance.OnPrepareInstance();
		yield return UIManager.Instance.OnPrepareInstance();
		yield return ObjectCameraManager.Instance.OnPrepareInstance();
	}


	public override void OnEnter()
	{
		SceneManager.Instance.LoadScene(SceneType.Login);
	}

	public override void OnExit()
	{

	}
}
