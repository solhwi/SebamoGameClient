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

	private PopupDictionary popupObjectDictionary = new PopupDictionary();

	private Stack<BoardGamePopup> popupStack = new Stack<BoardGamePopup>();

	public void TryOpen(PopupType popupType)
	{
		if (popupObjectDictionary.TryGetValue(popupType, out BoardGamePopup popupObj))
		{
			popupObj.OnOpen(popupStack.Count + rootCanvas.sortingOrder);
			popupStack.Push(popupObj);
		}
		else
		{
			if (popupDictionary.TryGetValue(popupType, out var popupPrefab))
			{
				var obj = Instantiate(popupPrefab);
				popupObjectDictionary[popupType] = obj;

				obj.OnOpen(popupStack.Count + rootCanvas.sortingOrder);
				popupStack.Push(obj);
			}
		}
	}

	public void Close()
	{
		var popupCanvas = popupStack.Pop();
		if (popupCanvas == null)
			return;

		popupCanvas.OnClose();
	}

	public void Close(PopupType popupType, int closeCount = 1)
	{
		Stack<BoardGamePopup> tempStack = new Stack<BoardGamePopup>();

		int count = 0;

		while (popupStack.TryPop(out var popupObj))
		{
			if (popupObj.popupType != popupType || count >= closeCount)
			{
				tempStack.Push(popupObj);
			}
			else
			{
				count++;
				popupObj.OnClose();
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
			popupObj.OnClose();
		}
	}
}
