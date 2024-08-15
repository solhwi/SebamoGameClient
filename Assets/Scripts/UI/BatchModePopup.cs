using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BatchModePopup : BoardGamePopup, IBeginDragHandler, IDragHandler
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
	private ReplaceFieldItem currentReplaceItem;

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

	public void OnClickTile(int tileIndex)
	{
		currentReplaceItem?.Replace(tileIndex).Wait();
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
}
