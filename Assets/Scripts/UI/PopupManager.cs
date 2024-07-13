using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
	public enum PopupType
	{
		Shop,
		Inventory,
		Notify,
	}

	[SerializeField] private Canvas rootCanvas;

	[System.Serializable]
	public class PopupDictionary : SerializableDictionary<PopupType, BoardGamePopup> { }
	[SerializeField] private PopupDictionary popupDictionary = new PopupDictionary();

	private Stack<BoardGamePopup> popupStack = new Stack<BoardGamePopup>();

	public void TryOpen(PopupType popupType, UIParameter parameter = null)
	{
		if (popupDictionary.TryGetValue(popupType, out BoardGamePopup popupObj))
		{
			popupObj.Open(rootCanvas, popupStack.Count);
			popupObj.OnOpen(parameter);
			popupStack.Push(popupObj);
		}
	}

	public void Close()
	{
		var popupCanvas = popupStack.Pop();
		if (popupCanvas == null)
			return;

		popupCanvas.Close();
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

	public void CloseAll()
	{
		while (popupStack.TryPop(out var popupObj))
		{
			popupObj.Close();
		}
	}
}
