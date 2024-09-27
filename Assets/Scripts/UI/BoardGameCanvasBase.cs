using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardGameCanvasBase : MonoBehaviour
{
	[SerializeField] private Canvas canvas;
	[SerializeField] private CanvasScaler canvasScaler;

	private void Reset()
	{
		canvas = GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;

		canvasScaler = GetComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1080, 1920);
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		canvasScaler.referencePixelsPerUnit = 100;
	}

	public void OnEnter()
	{
		OnOpen();
	}

	public void OnExit()
	{
		OnClose();
	}

	protected virtual void OnOpen()
	{

	}

	protected virtual void OnClose()
	{

	}
}
