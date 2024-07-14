using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParameter
{

}

public class BoardGamePopup : MonoBehaviour
{
	[SerializeField] private PopupManager.PopupType popupType;
	[SerializeField] private Canvas canvas;

	private void Reset()
	{
		canvas = GetComponent<Canvas>();
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
		PopupManager.Instance.Close(popupType);
	}

	public void Close()
	{
		gameObject.SetActive(false);
		canvas.sortingOrder = -1;

		OnClose();
	}

	public bool IsSameType(PopupManager.PopupType type)
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
