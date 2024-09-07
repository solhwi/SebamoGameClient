using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardGameCanvas : MonoBehaviour, IBoardGameSubscriber, IBeginDragHandler, IDragHandler
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Inventory inventory;
	[SerializeField] private CameraController boardGameCameraController;

	[SerializeField] private Text statusText = null;

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

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
		statusText.text = $"타일 처리 후 위치 : {nextOrder}";
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder,int diceCount)
	{
		yield return null;
		statusText.text = $"일반 이동 후 위치 : {nextOrder}";
	}

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, int nextBonusMultiplyCount)
	{
		yield return null;
		statusText.text = $"나온 주사위 값 : {diceCount} x {nextBonusMultiplyCount} + {nextBonusAddCount}";
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		yield return null;
	}
	public void OnClickRollDice()
	{
		boardGameCameraController.SetFollow(true);
		BoardGameManager.Instance.OnClickRollDice();
	}

	private void Update()
	{
		if (UIManager.Instance.IsAnyOpen())
		{
			boardGameCameraController.SetFollow(true);
			boardGameCameraController.SetZoom(false);
		}
		else
		{
			boardGameCameraController.SetZoom(true);
		}
	}

	public void OnClickShop()
	{
		UIManager.Instance.TryOpen(PopupType.Shop);
	}

	public void OnClickInventory()
	{
		UIManager.Instance.TryOpen(PopupType.Inventory);
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
		boardGameCameraController.ResetZoom();
	}
}
