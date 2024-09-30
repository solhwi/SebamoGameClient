using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneModule : SceneModuleBase
{
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private CharacterDataContainer characterDataContainer;
	[SerializeField] private LoadingCanvas loadingCanvas;

	[SerializeField] private float loadingCompleteWaitTime = 3.0f;
	private bool isLoaded = false;

	public override IEnumerator OnPrepareEnter()
	{
		isLoaded = false;

		loadingCanvas.OnEnter();

		loadingCanvas.barrierFunc -= IsLoaded;
		loadingCanvas.barrierFunc += IsLoaded;

		yield return base.OnPrepareEnter();

		loadingCanvas.SetWaitDescription("테이블 준비 중");

		yield return itemTable.PreLoadTableAssets();

		loadingCanvas.SetWaitDescription("캐릭터 리소스 준비 중");

		yield return characterDataContainer.PreLoadCharacterParts();
		yield return BoardGameManager.Instance.PreLoadCharacter();

		loadingCanvas.SetWaitDescription("아이템 준비 중");

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
		SceneManager.Instance.LoadScene(SceneType.Login);
	}

	public override void OnExit()
	{
		loadingCanvas.OnExit();
	}
}
