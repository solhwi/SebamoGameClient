using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveComponent : BoardGameSubscriber 
{
	[SerializeField] private TileDataManager tileDataManager = null;
	[SerializeField] private CharacterView characterView = null;
	[SerializeField] private PlayerDataContainer playerDataContainer = null;

	public override IEnumerator OnRollDice(int diceCount)
	{
		yield return null;
	}

	public override IEnumerator OnMove(int currentOrder, int diceCount)
	{
		characterView.DoRun();

		yield return ProcessMove(currentOrder, diceCount, 1.0f);

		characterView.DoIdle();
	}

	private IEnumerator ProcessMove(int currentOrder, int diceCount, float speedRate)
	{
		var tiles = tileDataManager.GetTilePath(currentOrder, diceCount);
		foreach (var tile in tiles)
		{
			Vector3 startPos = transform.position;

			// 방향 전환
			ProcessFlip(startPos, tile.tilePlayerPosition);

			float t = 0.0f;
			float moveTime = playerDataContainer.moveTimeByOneTile;
			while (t < moveTime)
			{
				t += Time.deltaTime * speedRate;
				var currentPos = Vector2.Lerp(startPos, tile.tilePlayerPosition, t);

				yield return null;

				// 실제 이동
				SetPosition(currentPos);
			}
		}
	}

	private IEnumerator ProcessTeleport(int currentOrder, int nextOrder, float speedRate)
	{
		var nextTile = tileDataManager.GetTileData(nextOrder);

		Vector3 startPos = transform.position;

		// 방향 전환
		ProcessFlip(startPos, nextTile.tilePlayerPosition);

		float t = 0.0f;
		float moveTime = playerDataContainer.moveTimeByOneTile;
		while (t < moveTime)
		{
			t += Time.deltaTime * speedRate;
			var currentPos = Vector2.Lerp(startPos, nextTile.tilePlayerPosition, t);

			yield return null;

			// 실제 이동
			SetPosition(currentPos);
		}
	}

	public override IEnumerator OnDoTileAction(TileDataManager tileDataManager, int currentOrder, int nextOrder)
	{
		characterView.DoRun();

		var specialTile = tileDataManager.GetCurrentSpecialTile(currentOrder);
		if (specialTile == null)
			yield break;

		Debug.Log($"다음의 특수 타일 효과 발동 : {specialTile.specialTileType}");

		switch(specialTile.specialTileType)
		{
			case SpecialTileType.Jump:
			case SpecialTileType.RollBack:
				yield return ProcessMove(currentOrder, nextOrder - currentOrder, 2.5f);
				break;

			case SpecialTileType.Teleport:
				yield return ProcessTeleport(currentOrder, nextOrder, 5.0f);
				break;
		}

		characterView.DoIdle();
	}

	private void ProcessFlip(Vector3 startPos, Vector3 endPos)
	{
		bool isFlipX = false;
		bool isFlipY = false;

		if (startPos.x < endPos.x)
		{
			if (startPos.y < endPos.y) // 우상단
			{
				isFlipX = true;
				isFlipY = true;
			}
			else if (startPos.y > endPos.y) // 우하단
			{
				// do nothing
			}
		}
		else if (startPos.x > endPos.x)
		{
			if (startPos.y < endPos.y) // 좌상단
			{
				isFlipY = true;
			}
			else if (startPos.y > endPos.y) // 좌하단
			{
				isFlipX = true;
			}
		}

		characterView.FlipX(isFlipX);
		characterView.FlipY(isFlipY);
	}

	public void SetPosition(Vector2 pos)
	{
		transform.position = new Vector3(pos.x, pos.y, 0);
	}
}
