using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardGameCanvas : BoardGameSubscriber
{
	[SerializeField] private BoardGameManager boardGameManager;
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Text statusText = null;

	public override IEnumerator OnMove(int currentOrderIndex, int diceCount)
	{
		yield return null;
		statusText.text = $"현재 위치 : {playerDataContainer.currentTileOrderIndex}";
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
}
