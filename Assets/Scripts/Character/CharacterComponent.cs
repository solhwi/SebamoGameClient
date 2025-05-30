using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterComponent : MonoBehaviour 
{
	[SerializeField] protected CharacterView characterView = null;

	public void SetPlayerData(string playerGroup, string playerName)
	{
		characterView.Initialize();
		characterView.SetPlayerData(playerGroup, playerName);
	}

	public void Refresh()
	{
		characterView.RefreshCharacter();
		characterView.Replay();
	}

	public IEnumerator ProcessMove(int currentOrder, int nextOrder, float speedRate)
	{
		var tiles = TileDataManager.Instance.GetTilePath(currentOrder, nextOrder - currentOrder);
		foreach (var tile in tiles)
		{
			Vector3 startPos = transform.position;

			// 방향 전환
			ProcessFlip(startPos, tile.tilePlayerPosition);

			float t = 0.0f;
			float moveTime = PlayerDataContainer.Instance.moveTimeByOneTile;
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

	public IEnumerator ProcessTeleport(int currentOrder, int nextOrder, float speedRate)
	{
		var currentTile = TileDataManager.Instance.GetTileData(currentOrder);
		if (currentTile.tileBase is TeleportSpecialTile specialTile == false)
			yield break;

		var nextTile = TileDataManager.Instance.GetTileData(nextOrder);

		Vector3 startPos = transform.position;

		// 방향 전환
		ProcessFlip(startPos, nextTile.tilePlayerPosition);

		yield return specialTile.DepartEffect(characterView.originCharacterTransform);
		specialTile.DestroyEffect();

		SetPosition(nextTile.tilePlayerPosition);

		yield return specialTile.ArriveEffect(characterView.originCharacterTransform);
		specialTile.DestroyEffect();

		characterView.Replay();
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
		transform.position = new Vector3(pos.x, pos.y, -(int)LayerConfig.Character);
	}
}
