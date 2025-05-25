using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinTextComponent : MonoBehaviour, IBoardGameSubscriber
{
	[SerializeField] private Text coinText;

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
		int currentCoinCount = Inventory.Instance.GetHasCoinCount();

		int count = prevCoinCount;
		while (count < currentCoinCount)
		{
			count += 10;

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
		prevCoinCount = Inventory.Instance.GetHasCoinCount();
		coinText.text = prevCoinCount.ToString("n0");
	}

	private void OnDestroy()
	{
		BoardGameManager.Instance?.Unsubscribe(this);
	}
}
