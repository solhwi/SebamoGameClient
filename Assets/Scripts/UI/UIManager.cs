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
	Wait,
}

public class UIManager : Singleton<UIManager>
{
	[SerializeField] private Canvas backgroundCanvas;

	[SerializeField] private LoginCanvas loginCanvas;
	[SerializeField] private BoardGameCanvas boardGameCanvas;
	[SerializeField] private Canvas popupRootCanvas;

	public override bool IsDestroyOnLoad => false;

	[System.Serializable]
	public class PopupDictionary : SerializableDictionary<PopupType, BoardGamePopup> { }
	[SerializeField] private PopupDictionary popupDictionary = new PopupDictionary();

	private Stack<BoardGamePopup> popupStack = new Stack<BoardGamePopup>();

	protected override void Awake()
	{
		base.Awake();

	}

	public void TryOpen(PopupType popupType, UIParameter parameter = null)
	{
		if (popupDictionary.TryGetValue(popupType, out var newPopup))
		{
			if (newPopup != null && newPopup.IsOpen == false)
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

	private void SetActiveBackgroundCanvas(bool isActive)
	{
		backgroundCanvas.gameObject.SetActive(isActive);
		backgroundCanvas.worldCamera = isActive ? Camera.main : null;
	}

	public void OpenMainCanvas(SceneType sceneType)
	{
		if (sceneType == SceneType.Login)
		{
			loginCanvas.gameObject.SetActive(true);
			boardGameCanvas.gameObject.SetActive(false);
		}
		else if(sceneType == SceneType.Game)
		{
			loginCanvas.gameObject.SetActive(false);
			boardGameCanvas.gameObject.SetActive(true);
		}

		SetActiveBackgroundCanvas(sceneType == SceneType.Game);
	}

	public void CloseMainCanvas()
	{
		loginCanvas.gameObject.SetActive(false);
		boardGameCanvas.gameObject.SetActive(false);

		SetActiveBackgroundCanvas(false);
	}

	public bool IsOpenMainCanvas()
	{
		return boardGameCanvas.gameObject.activeSelf;
	}

	public bool IsOpenBackgroundCanvas()
	{
		return backgroundCanvas.gameObject.activeSelf;
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

	public bool IsAnyOpen(IEnumerable<PopupType> popupTypes)
	{
		foreach (var popupType in popupTypes)
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
