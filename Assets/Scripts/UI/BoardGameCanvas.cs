using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardGameCanvas : BoardGameSubscriber
{
	[SerializeField] private BoardGameManager boardGameManager;
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Inventory inventory;

	[SerializeField] private Text statusText = null;
	[SerializeField] private Text coinText = null;

	[SerializeField] private RectTransform rankingBoard = null;
	public bool isRankingBoardToggleOn = false;

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

	private void Update()
	{
		SetCoinCount();
		SetRankingBoardPosition();
	}

	private void SetCoinCount()
	{
		int hasCoinCount = inventory.GetHasCoinCount();
		coinText.text = $"보유 코인 : {hasCoinCount}";
	}

	private void SetRankingBoardPosition()
	{
		if (isRankingBoardToggleOn)
		{
			rankingBoard.anchoredPosition = Vector3.zero;
		}
		else
		{
			rankingBoard.anchoredPosition = new Vector3(0, rankingBoard.rect.height, 0);
		}
	}

	public void OnClickRollDice()
	{
		boardGameManager.OnClickRollDice();
	}

	public void OnClickShop()
	{

	}

	public void OnClickInventory()
	{

	}

	public void OnClickRankingBoardToggle()
	{
		isRankingBoardToggleOn = !isRankingBoardToggleOn;
	}
}
