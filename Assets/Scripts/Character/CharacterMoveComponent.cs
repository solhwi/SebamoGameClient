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

		var tiles = tileDataManager.GetTilePath(currentOrder, diceCount);
		foreach (var tile in tiles)
		{
			Vector3 startPos = transform.position;

			// 방향 전환
			ProcessFlip(startPos, tile.tilePlayerPosition);

			Debug.Log($"start move {startPos} > {tile.tilePlayerPosition} [{Time.time}]");

			float t = 0.0f;
			float moveTime = playerDataContainer.moveTimeByOneTile;
			while (t < moveTime)
			{
				t += Time.deltaTime;
				var currentPos = Vector2.Lerp(startPos, tile.tilePlayerPosition, t);

				yield return null;

				// 실제 이동
				SetPosition(currentPos);
			}

			Debug.Log($"end move {startPos} > {tile.tilePlayerPosition} [{Time.time}]");
		}

		characterView.DoIdle();
	}

	public override IEnumerator OnDoTileAction(WorldTileData tileData, int currentOrder, int nextOrder)
	{
		characterView.DoRun();

		var tiles = tileDataManager.GetTilePath(currentOrder, nextOrder - currentOrder);
		foreach (var tile in tiles)
		{
			Vector3 startPos = transform.position;

			// 방향 전환
			ProcessFlip(startPos, tile.tilePlayerPosition);

			Debug.Log($"start move {startPos} > {tile.tilePlayerPosition} [{Time.time}]");

			float t = 0.0f;
			float moveTime = playerDataContainer.moveTimeByOneTile;
			while (t < moveTime)
			{
				t += Time.deltaTime * 2;
				var currentPos = Vector2.Lerp(startPos, tile.tilePlayerPosition, t);

				yield return null;

				// 실제 이동
				SetPosition(currentPos);
			}

			Debug.Log($"end move {startPos} > {tile.tilePlayerPosition} [{Time.time}]");
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
