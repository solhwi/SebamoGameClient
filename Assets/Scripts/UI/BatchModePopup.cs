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

	[SerializeField] private FieldItemFactory fieldItemFactory;

	[SerializeField] private CameraController boardGameCameraController;
	[SerializeField] private TileDataManager tileDataManager;
	[SerializeField] private PlayerDataContainer playerDataContainer;

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

		boardGameCameraController.SetZoom(true);

		if (parameter is Parameter p)
		{
			currentReplaceItem = p.replaceItem;
		}
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

	public void OnClickBatch()
	{
		if (currentTileOrder == -1)
			return;

		if (currentReplaceItem == null)
			return;

		if (currentReplaceItem.IsReplaceable(tileDataManager, playerDataContainer, currentTileOrder) == false)
			return;

		currentReplaceItem?.Replace(tileDataManager, currentTileOrder);
		
		if (currentDummyReplaceItem != null)
		{
			currentDummyReplaceItem.Destroy();
			currentDummyReplaceItem = null;
		}

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

			if (currentReplaceItem != null)
			{
				if (currentDummyReplaceItem != null)
				{
					currentDummyReplaceItem.Destroy();
					currentDummyReplaceItem = null;
				}

				currentDummyReplaceItem = fieldItemFactory.Make<ReplaceFieldItem>(currentReplaceItem.fieldItemCode);

				if (currentDummyReplaceItem != null)
				{
					var worldTileData = tileDataManager.GetTileData(currentTileOrder);
					currentDummyReplaceItem.CreateDummy(worldTileData);
				}
			}
		}
	}
}
