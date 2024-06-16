using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardGameCanvas : BoardGameSubscriber
{
	[SerializeField] private BoardGameManager boardGameManager;
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Text statusText = null;

	public CharacterType currentCharacterType = CharacterType.UnityChan;

	private void Awake()
	{
		playerDataContainer.ResetCharacterMeshType(currentCharacterType);
	}

	public override IEnumerator OnMove(int currentOrderIndex, int diceCount)
	{
		yield return null;
		statusText.text = $"{diceCount}칸 만큼 이동 중...";
	}

	public override IEnumerator OnRollDice(int diceCount)
	{
		yield return null;
		statusText.text = $"나온 주사위 눈 : {diceCount.ToString()}";
	}

	public void OnClickRollDice()
	{
		boardGameManager.OnClickRollDice();
	}

	public void OnClickChangeAvatar()
	{
		int nextType = (int)currentCharacterType + 1 % (int)CharacterType.Max;

		currentCharacterType = (CharacterType)nextType;
		playerDataContainer.ResetCharacterMeshType(currentCharacterType);
	}
}
