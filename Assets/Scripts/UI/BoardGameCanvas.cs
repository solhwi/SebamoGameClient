using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardGameCanvas : BoardGameSubscriber
{
	[SerializeField] private BoardGameManager boardGameManager;
	[SerializeField] private Text diceText = null;

	public override IEnumerator OnMove(int currentOrderIndex, int diceCount)
	{
		yield return null;
	}

	public override IEnumerator OnRollDice(int diceCount)
	{
		yield return null;
		diceText.text = $"나온 주사위 눈 : {diceCount.ToString()}";
	}

	public void OnClickRollDice()
	{
		boardGameManager.OnClickRollDice();
	}
}
