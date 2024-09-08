using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PopupType
{
	Shop,
	Inventory,
	ItemToolTip,
	BatchMode,
	Notify,
	Profile,
}

public class UIManager : Singleton<UIManager>
{
	[SerializeField] private BoardGameCanvas boardGameCanvas;
	[SerializeField] private Canvas popupRootCanvas;

	[System.Serializable]
	public class PopupDictionary : SerializableDictionary<PopupType, BoardGamePopup> { }
	[SerializeField] private PopupDictionary popupDictionary = new PopupDictionary();

	private Stack<BoardGamePopup> popupStack = new Stack<BoardGamePopup>();

	public void TryOpen(PopupType popupType, UIParameter parameter = null)
	{
		if (popupDictionary.TryGetValue(popupType, out var newPopup))
		{
			if (newPopup != null)
			{
				newPopup.Open(popupRootCanvas, popupStack.Count);
				newPopup.OnOpen(parameter);
				popupStack.Push(newPopup);
			}
		}
	}

	public void Close()
	{
		var popupCanvas = popupStack.Pop();
		if (popupCanvas == null)
			return;

		popupCanvas.Close();
	}

	public void OpenMainCanvas()
	{
		boardGameCanvas.gameObject.SetActive(true);
	}

	public void CloseMainCanvas()
	{
		boardGameCanvas.gameObject.SetActive(false);
	}

	public bool IsOpenMainCanvas()
	{
		return boardGameCanvas.gameObject.activeSelf;
	}

	public void CloseAll(PopupType popupType)
	{
		Close(popupType, int.MaxValue);
	}

	public void Close(PopupType popupType, int closeCount = 1)
	{
		Stack<BoardGamePopup> tempStack = new Stack<BoardGamePopup>();

		int count = 0;

		while (popupStack.TryPop(out var popupObj))
		{
			if (popupObj.IsSameType(popupType) == false || count >= closeCount)
			{
				tempStack.Push(popupObj);
			}
			else
			{
				count++;
				popupObj.Close();
			}
		}

		while(tempStack.TryPop(out var newPopupObj))
		{
			popupStack.Push(newPopupObj);
		}
	}

	public bool IsAnyOpen()
	{
		foreach (var popupType in popupDictionary.Keys)
		{
			if (IsOpen(popupType))
			{
				return true;
			}
		}

		return false;
	}

	public bool IsOpen(PopupType popupType)
	{
		if (popupDictionary.TryGetValue(popupType, out var popup))
		{
			return popup.IsOpen;
		}

		return false;
	}

	public void CloseAll()
	{
		while (popupStack.TryPop(out var popupObj))
		{
			popupObj.Close();
		}
	}
}
