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
	private bool isUpdate = true;

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield break;
	}

	public void OnEndTurn()
	{
		SetCoinText();
		isUpdate = true;
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		yield break;
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount)
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

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, float nextBonusMultiplyCount)
	{
		yield break;
	}

	public void OnStartTurn()
	{
		SetCoinText();
		isUpdate = false;
	}

	private void Start()
    {
		BoardGameManager.Instance?.Subscribe(this);
		SetCoinText();
	}

	private void Update()
	{
		if (isUpdate)
		{
			SetCoinText();
		}
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
