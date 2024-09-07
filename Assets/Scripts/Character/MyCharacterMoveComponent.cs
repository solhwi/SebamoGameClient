using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterMoveComponent : CharacterMoveComponent, IBoardGameSubscriber
{
	private void Start()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Subscribe(this);
		}
	}

	private void OnDestroy()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Unsubscribe(this);
		}
	}

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, int nextBonusMultiplyCount)
	{
		yield return null;
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount)
	{
		characterView.DoRun();

		yield return ProcessMove(currentOrder, nextOrder, 1.0f);

		characterView.DoIdle();
	}

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		characterView.DoRun();

		var specialTile = TileDataManager.Instance.GetCurrentSpecialTile(currentOrder);
		if (specialTile == null)
			yield break;

		Debug.Log($"다음의 특수 타일 효과 발동 : {specialTile.specialTileType}");

		switch (specialTile.specialTileType)
		{
			case SpecialTileType.Jump:
			case SpecialTileType.RollBack:
				yield return ProcessMove(currentOrder, nextOrder, 2.5f);
				break;

			case SpecialTileType.Teleport:
				yield return ProcessTeleport(currentOrder, nextOrder, 5.0f);
				break;
		}

		characterView.DoIdle();
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		Debug.Log($"다음의 아이템 효과 발동 : {fieldItem.fieldActionType}");

		switch (fieldItem.fieldActionType)
		{
			case FieldActionType.Banana:
				characterView.DoRun();
				yield return ProcessMove(currentOrder, nextOrder, 2.5f);
				characterView.DoIdle();
				break;
		}
	}

}
