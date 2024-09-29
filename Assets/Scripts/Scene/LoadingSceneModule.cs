using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneModule : SceneModuleBase
{
	[SerializeField] private ItemTable itemTable;

	public override IEnumerator OnPrepareEnter()
	{
		yield return base.OnPrepareEnter();
		yield return itemTable.PreLoadTableAssets();
		yield return BoardGameManager.Instance.PreLoadCharacter();
		yield return UIManager.Instance.PreLoadPopup();
		yield return ObjectCameraManager.Instance.PreLoadObjectCamera();

		yield return ObjectManager.Instance.PreInstantiateFieldItemObject();
	}


	public override void OnEnter()
	{
		SceneManager.Instance.LoadScene(SceneType.Login);
	}

	public override void OnExit()
	{

	}
}
