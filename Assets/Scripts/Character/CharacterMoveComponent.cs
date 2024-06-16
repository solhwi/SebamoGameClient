using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveComponent : BoardGameSubscriber 
{
	[SerializeField] private TileDataManager tileDataManager = null;
	[SerializeField] private PlayerCharacterView characterView = null;
	[SerializeField] private float moveTime = 1.0f;

	public override IEnumerator OnRollDice(int diceCount)
	{
		yield return null;
	}

	public override IEnumerator OnMove(int currentOrderIndex, int diceCount)
	{
		var tiles = tileDataManager.GetTilePath(currentOrderIndex, diceCount);
		foreach (var tile in tiles)
		{
			Vector3 startPos = transform.position;

			// 방향 전환
			characterView.FlipX(tile.tileWorldPosition.x - startPos.x < 0);

			Debug.Log($"start move {startPos} > {tile.tileWorldPosition} [{Time.time}]");

			float t = 0.0f;
			while (t < moveTime)
			{
				t += Time.deltaTime;
				var currentPos = Vector2.Lerp(startPos, tile.tileWorldPosition, t);

				yield return null;
				// 실제 이동
				SetPosition(currentPos);
			}

			Debug.Log($"end move {startPos} > {tile.tileWorldPosition} [{Time.time}]");
		}
	}

	public void SetPosition(Vector2 pos)
	{
		transform.position = new Vector3(pos.x, pos.y, transform.position.z);
	}
}
