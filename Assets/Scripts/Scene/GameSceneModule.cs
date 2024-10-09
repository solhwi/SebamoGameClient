using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneModule : SceneModuleBase
{
	[SerializeField] private Canvas backgroundCanvas = null;

	private Coroutine gameRoutine = null;

	public override IEnumerator OnPrepareEnter()
	{
		yield return base.OnPrepareEnter();

		SoundManager.Instance.PlayBGM(SoundManager.BGMType.Game, true);

		yield return TileDataManager.Instance.PrepareBoardData();
		yield return TileDataManager.Instance.PrepareTile();
		yield return BoardGameManager.Instance.PrepareCharacter();
	}

	public override void OnEnter()
	{
		base.OnEnter();

		if (HttpNetworkManager.Instance.isOfflineMode)
		{
			HttpNetworkManager.Instance.IsConnected = true;
		}

		CameraController.Instance.ResetTarget();
		backgroundCanvas.worldCamera = Camera.main;

		gameRoutine = StartCoroutine(ProcessGame());
	}

	private IEnumerator ProcessGame()
	{
		yield return BoardGameManager.Instance.ProcessBoardGame();
	}

	public override void OnExit()
	{
		base.OnExit();

		if (gameRoutine != null)
		{
			StopCoroutine(gameRoutine);
		}
	}
}
