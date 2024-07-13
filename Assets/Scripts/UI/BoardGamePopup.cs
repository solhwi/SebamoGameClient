using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGamePopup : MonoBehaviour
{
	[SerializeField] public PopupManager.PopupType popupType;
	[SerializeField] private Canvas canvas;
	
	public void OnOpen(int sortingOrder)
	{
		gameObject.SetActive(true);
		canvas.overrideSorting = true;
		canvas.sortingOrder = sortingOrder;
	}

	public void OnClickClose()
	{
		PopupManager.Instance.Close(popupType);
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
		canvas.sortingOrder = -1;
	}
}
