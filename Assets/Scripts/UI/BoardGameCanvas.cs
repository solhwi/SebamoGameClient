using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardGameCanvas : BoardGameCanvasBase, IBoardGameSubscriber, IBeginDragHandler, IDragHandler
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Inventory inventory;

	[SerializeField] private DiceView diceView = null; 
	[SerializeField] private ProfileSetter profileSetter;
	[SerializeField] private Text statusText = null;

	[SerializeField] private List<PopupType> cameraResetPopupTypes = new List<PopupType>();

	protected override void OnOpen()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Subscribe(this);
		}

		diceView.Initialize();
	}

	protected override void OnClose()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Unsubscribe(this);
		}
	}
	public void OnStartTurn()
	{

	}
	public void OnEndTurn()
	{

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

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, float nextBonusMultiplyCount)
	{
		statusText.text = $"나온 주사위 값 : {diceCount} x {nextBonusMultiplyCount} + {nextBonusAddCount}";
		yield return diceView.DoDiceAction(diceCount);
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		yield return null;
	}

	public void OnClickRollDice()
	{
		CameraController.Instance.ResetTarget();
		CameraController.Instance.SetFollow(true);
		BoardGameManager.Instance.OnClickRollDice();
	}

	private void Update()
	{
		if (UIManager.Instance.IsAnyOpen(cameraResetPopupTypes))
		{
			CameraController.Instance.ResetTarget();
			CameraController.Instance.SetFollow(true);
			CameraController.Instance.SetZoom(false);
		}
		else
		{
			CameraController.Instance.SetZoom(true);
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

	public void OnClickSetting()
	{
		UIManager.Instance.TryOpen(PopupType.Setting);
	}

	public void OnBeginDrag(PointerEventData data)
	{
		CameraController.Instance.SetFollow(false);
	}

	public void OnDrag(PointerEventData data)
	{
		
	}

	public void OnClickResetCamera()
	{
		CameraController.Instance.ResetTarget();
		CameraController.Instance.SetFollow(true);
		CameraController.Instance.ResetZoom();
	}
}
