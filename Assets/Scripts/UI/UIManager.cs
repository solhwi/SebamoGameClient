using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PopupType
{
	Shop,
	Inventory,
	Notify,
	BatchMode,
}

public class UIManager : Singleton<UIManager>
{
	[SerializeField] private BoardGameCanvas boardGameCanvas;
	[SerializeField] private Canvas popupRootCanvas;

	[System.Serializable]
	public class PopupDictionary : SerializableDictionary<PopupType, BoardGamePopup> { }
	[SerializeField] private PopupDictionary popupDictionary = new PopupDictionary();

	private Dictionary<PopupType, List<BoardGamePopup>> activePopupDictionary = new Dictionary<PopupType, List<BoardGamePopup>>();

	private Stack<BoardGamePopup> popupStack = new Stack<BoardGamePopup>();

	protected override void Awake()
	{
		activePopupDictionary.Clear();

		foreach (var iterator in popupDictionary)
		{
			var popupType = iterator.Key;
			var popupObj = iterator.Value;

			// 재활용 팝업이 아닌 경우 바로 ACTIVE 처리
			if (popupObj.IsRecyclable == false)
			{
				popupObj.gameObject.SetActive(false);
				activePopupDictionary.Add(popupType, new List<BoardGamePopup>() { popupObj });
			}
			else
			{
				activePopupDictionary.Add(popupType, new List<BoardGamePopup>());
			}
		}
	}

	public void TryOpen(PopupType popupType, UIParameter parameter = null)
	{
		if (popupDictionary.TryGetValue(popupType, out var popupObj))
		{
			BoardGamePopup newPopup = null;
			// 살아있는 팝업 중 1차로 안 열려있는 팝업을 가져옴
			if (activePopupDictionary.TryGetValue(popupType, out List<BoardGamePopup> popups))
			{
				newPopup = popups.Find(p => p.IsOpen == false);
			}

			// 살아있는 팝업이 없는 경우, 재활용 팝업이라면 새로 만듦
			if (newPopup == null && popupObj.IsRecyclable)
			{
				newPopup = Instantiate(popupObj);
				activePopupDictionary[popupType].Add(newPopup);
			}

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
		if (popupDictionary.TryGetValue(popupType, out BoardGamePopup popupObj))
		{
			return popupObj.IsOpen;
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
