using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinTextComponent : MonoBehaviour, IBoardGameSubscriber
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private Text coinText;

	[SerializeField] private float coinEffectTime = 1.0f;
	[SerializeField] private float coinEffectSpeedRate = 1.0f;

	private int prevCoinCount = 0;

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield break;
	}

	public void OnEndTurn()
	{
		SetCoinText();
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		yield break;
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount)
	{
		yield break;
	}

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, float nextBonusMultiplyCount)
	{
		int currentCoinCount = inventory.GetHasCoinCount();

		float t = 0.0f;
		while (t < coinEffectTime)
		{
			t += Time.deltaTime * coinEffectSpeedRate;
			int count = (int)Mathf.Lerp(prevCoinCount, currentCoinCount, t);

			yield return null;

			coinText.text = count.ToString("n0");
		}

		SetCoinText();
	}

	public void OnStartTurn()
	{
		SetCoinText();
	}

	private void Start()
    {
		BoardGameManager.Instance?.Subscribe(this);
		SetCoinText();
	}

	private void SetCoinText()
	{
		prevCoinCount = inventory.GetHasCoinCount();
		coinText.text = prevCoinCount.ToString("n0");
	}

	private void OnDestroy()
	{
		BoardGameManager.Instance?.Unsubscribe(this);
	}
}
