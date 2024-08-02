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

	public void OnBeginDrag(PointerEventData data)
	{
		boardGameCameraController.SetFollow(false);
	}

	public void OnDrag(PointerEventData data)
	{
		
	}

	public void OnClickResetCamera()
	{
		boardGameCameraController.SetFollow(true);
	}
}
