using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParameter
{

}

public class BoardGamePopup : MonoBehaviour
{
	[SerializeField] protected PopupType popupType;
	[SerializeField] private Canvas canvas;
	[SerializeField] private CanvasScaler canvasScaler;

	public bool IsOpen
	{
		get
		{
			return gameObject.activeSelf;
		}
	}

	protected virtual void Reset()
	{
		canvas = GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		
		canvasScaler = GetComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1080, 1920);
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		canvasScaler.referencePixelsPerUnit = 100;
	}

	public void Open(Canvas rootCanvas, int sortingOrder)
	{
		gameObject.SetActive(true);
		transform.SetParent(rootCanvas.transform);

		canvas.overrideSorting = true;
		canvas.sortingOrder = rootCanvas.sortingOrder + sortingOrder;
	}

	public void OnClickClose()
	{
		UIManager.Instance.Close(popupType);
	}

	public void Close()
	{
		gameObject.SetActive(false);
		canvas.sortingOrder = -1;

		OnClose();
	}

	public bool IsSameType(PopupType type)
	{
		return popupType == type;
	}

	public virtual void OnOpen(UIParameter parameter = null)
	{

	}

	protected virtual void OnClose()
	{

	}
}
