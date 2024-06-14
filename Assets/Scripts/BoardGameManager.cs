using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameManager : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileDataManager tileDataManager;

	[SerializeField] private PlayerCharacterView playerCharacterView;
	[SerializeField] private PlayerCharacterController characterController;

    private IEnumerator Start()
	{
		yield return null;

		// 데이터 기반 3D 모델 세팅

		yield return null;

		// 플레이어 캐릭터 뷰 타일 위로 배치

		Vector2 playerPos = GetPlayerPos();
		playerCharacterView.SetPosition(playerPos);

		yield return null;
	}

	private Vector2 GetPlayerPos()
	{
		if (playerDataContainer == null)
		{
			Debug.Log("플레이어 데이터 없음");
			return default;
		}

		// 내 타일이 몇 번째 순서인 지
		int currentPlayerTileOrderIndex = playerDataContainer.currentTileOrderIndex;

		if (tileDataManager == null)
		{
			Debug.Log("타일 데이터 매니저 없음");
			return default;
		}

		var currentPlayerTileData = tileDataManager.GetTileDataByOrder(currentPlayerTileOrderIndex);
		return currentPlayerTileData.tileWorldPosition;
	}
}
