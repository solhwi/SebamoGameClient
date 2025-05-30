
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

	

	private ReplaceFieldItem currentReplaceItem;
	private ReplaceFieldItem currentDummyReplaceItem;

	private int currentTileOrder;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.BatchMode;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		CameraController.Instance.SetZoom(true);

		if (parameter is Parameter p)
		{
			currentReplaceItem = p.replaceItem;
		}

		currentTileOrder = -1;
	}

	protected override void OnClose()
	{
		if (currentDummyReplaceItem != null)
		{
			currentDummyReplaceItem.Destroy();
			currentDummyReplaceItem = null;
		}

		base.OnClose();
	}

	public  void OnClickBatch()
	{
		if (currentTileOrder == -1)
			return;

		if (currentReplaceItem == null)
			return;

		if (currentReplaceItem.IsReplaceable(currentTileOrder) == false)
			return;

		StartCoroutine(currentReplaceItem.Replace(currentTileOrder, (d) =>
		{
			if (currentDummyReplaceItem != null)
			{
				currentDummyReplaceItem.Destroy();
				currentDummyReplaceItem = null;
			}

			BoardGameManager.Instance.EndReplaceMode(true);
		}));
	}

	public void OnClickStop()
	{
		BoardGameManager.Instance.EndReplaceMode(false);
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

	public void OnPointerClick(PointerEventData eventData)
	{
		Vector2 clickPos = Camera.main.ScreenToWorldPoint(eventData.position);

		int tileIndex = TileDataManager.Instance.GetTileIndexFromPos(clickPos);
		if (tileIndex > -1)
		{
			int tileOrder = TileDataManager.Instance.GetTileOrder(tileIndex);

			// 설치 가능한 위치에 클릭을 했다면
			if (currentReplaceItem != null && currentReplaceItem.IsReplaceable(tileOrder))
			{
				currentTileOrder = tileOrder;

				// 기존 더미 파괴
				if (currentDummyReplaceItem != null)
				{
					currentDummyReplaceItem.Destroy();
					currentDummyReplaceItem = null;
				}

				// 새 더미 생성
				currentDummyReplaceItem = FieldItemFactory.Instance.Make<ReplaceFieldItem>(currentReplaceItem.fieldItemCode);

				if (currentDummyReplaceItem != null)
				{
					var worldTileData = TileDataManager.Instance.GetTileData(currentTileOrder);
					currentDummyReplaceItem.CreateDummy(worldTileData);
				}
			}
		}
	}
}
