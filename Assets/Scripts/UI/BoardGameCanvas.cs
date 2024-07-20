using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardGameCanvas : BoardGameSubscriber, IBeginDragHandler, IDragHandler
{
	[SerializeField] private BoardGameManager boardGameManager;
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Inventory inventory;
	[SerializeField] private CameraController boardGameCameraController;

	[SerializeField] private Text statusText = null;
	[SerializeField] private Text coinText = null;

	[SerializeField] private RectTransform rankingBoard = null;

	private Vector2 dragStartPos = Vector2.zero;

	public bool isRankingBoardToggleOn = false;

	public override IEnumerator OnMove(int currentOrder, int diceCount)
	{
		yield return null;
		statusText.text = $"현재 위치 : {playerDataContainer.currentTileOrder}";
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
		boardGameCameraController.SetFollow(true);
		boardGameManager.OnClickRollDice();
	}

	public void OnClickShop()
	{
		PopupManager.Instance.TryOpen(PopupManager.PopupType.Shop);
	}

	public void OnClickInventory()
	{
		PopupManager.Instance.TryOpen(PopupManager.PopupType.Inventory);
	}

	public void OnClickRankingBoardToggle()
	{
		isRankingBoardToggleOn = !isRankingBoardToggleOn;
	}

	public void OnBeginDrag(PointerEventData data)
	{
		dragStartPos = data.position;
		boardGameCameraController.SetFollow(false);
	}

	public void OnDrag(PointerEventData data)
	{
		Vector3 deltaPos = dragStartPos - data.position;
		boardGameCameraController.Move(Time.deltaTime * deltaPos);

		dragStartPos = data.position;
	}

	public void OnClickResetCamera()
	{
		boardGameCameraController.SetFollow(true);
	}
}
