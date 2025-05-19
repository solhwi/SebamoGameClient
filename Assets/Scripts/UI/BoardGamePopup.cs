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

	[SerializeField] private RectTransform rectTransform;
	[SerializeField] private float openTime = 0.1f;

	private Coroutine openRoutine = null;

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
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		canvasScaler.matchWidthOrHeight = 1.0f;
		canvasScaler.referencePixelsPerUnit = 100;

		rectTransform = GetComponent<RectTransform>();
	}

	public void Open(Canvas rootCanvas, int sortingOrder)
	{
		gameObject.SetActive(true);
		transform.SetParent(rootCanvas.transform);

		canvas.overrideSorting = true;
		canvas.sortingOrder = rootCanvas.sortingOrder + sortingOrder * (int)UIManager.CanvasGroup.Popup;

		if (openTime > 0.0f)
		{
			rectTransform.localScale = Vector3.zero;
			openRoutine = StartCoroutine(OpenRoutine());
		}
		else
		{
			rectTransform.localScale = Vector3.one;
		}
	}

	private IEnumerator OpenRoutine()
	{
		float t = 0.0f;

		while (t < openTime)
		{
			yield return null;

			t += Time.deltaTime;

			float per = t / openTime;
			rectTransform.localScale = new Vector3(per, per, per);
		}

		rectTransform.localScale = Vector3.one;
	}

	public void OnClickClose()
	{
		UIManager.Instance.Close(popupType);
	}

	public void Close()
	{
		gameObject.SetActive(false);
		canvas.sortingOrder = -1;

		if (openRoutine != null)
		{
			StopCoroutine(openRoutine);
		}

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
