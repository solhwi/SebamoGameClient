using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BatchModePopup : BoardGamePopup, IBeginDragHandler, IDragHandler, IPointerClickHandler
{
	public class Parameter : UIParameter
	{
		public readonly ReplaceFieldItem replaceItem;

		public Parameter(ReplaceFieldItem replaceItem)
		{
			this.replaceItem = replaceItem;
		}
	}

	[SerializeField] private CameraController boardGameCameraController;
	[SerializeField] private TileDataManager tileDataManager;
	[SerializeField] private PlayerDataContainer playerDataContainer;

	private ReplaceFieldItem currentReplaceItem;

	private int currentTileOrder;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.BatchMode;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		boardGameCameraController.SetZoom(true);

		if (parameter is Parameter p)
		{
			currentReplaceItem = p.replaceItem;
		}
	}

	public void OnClickBatch()
	{
		if (currentTileOrder == -1)
			return;

		if (currentReplaceItem == null)
			return;

		if (currentReplaceItem.IsReplaceable(playerDataContainer, currentTileOrder) == false)
			return;

		currentReplaceItem?.Replace(tileDataManager, currentTileOrder).Wait();
		BoardGameManager.Instance.EndReplaceMode(true);
	}

	public void OnClickStop()
	{
		BoardGameManager.Instance.EndReplaceMode(false);
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

	public void OnPointerClick(PointerEventData eventData)
	{
		Vector2 clickPos = Camera.main.ScreenToWorldPoint(eventData.position);

		int index = tileDataManager.GetTileIndexFromPos(clickPos);
		if (index > -1)
		{
			currentTileOrder = tileDataManager.GetTileOrder(index);
			Debug.Log($"{currentTileOrder}번 타일이 선택됨");
		}
	}
}
