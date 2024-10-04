using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
	[System.Serializable]
	public class PopupRefDictionary : SerializableDictionary<PopupType, AssetReferenceGameObject> { }
	[SerializeField] private PopupRefDictionary popupRefDictionary = new PopupRefDictionary();

	[SerializeField] private NotifyPopup notifyPopupPrefab = null;

	private Dictionary<PopupType, BoardGamePopup> popupDictionary = new Dictionary<PopupType, BoardGamePopup>();
	private readonly Stack<BoardGamePopup> popupStack = new Stack<BoardGamePopup>();

	private BoardGameCanvasBase boardGameMainCanvas;
	private Canvas popupRootCanvas;

	protected override void OnAwakeInstance()
	{
		base.OnAwakeInstance();

		popupRootCanvas = GetComponentInChildren<Canvas>();
	}

	public IEnumerator PreLoadPopup()
	{
		foreach (var p in popupRefDictionary.Values)
		{
			yield return ResourceManager.Instance.LoadAsync<BoardGamePopup>(p);
		}
	}

	public IEnumerator PreLoadByResources()
	{
		var notifyPopup = Instantiate(notifyPopupPrefab, popupRootCanvas.transform);
		notifyPopup.gameObject.SetActive(false);

		popupDictionary[PopupType.Notify] = notifyPopup;

		yield return null;
	}

	public void TryOpen(PopupType popupType, UIParameter parameter = null)
	{
		if (popupDictionary.TryGetValue(popupType, out var newPopup) == false)
		{
			if (popupRefDictionary.TryGetValue(popupType, out var popupPrefabRef))
			{
				ResourceManager.Instance.TryInstantiateAsync<BoardGamePopup>(popupPrefabRef, popupRootCanvas.transform, (newPopup) =>
				{
					if (newPopup != null)
					{
						newPopup.Open(popupRootCanvas, popupStack.Count);
						newPopup.OnOpen(parameter);
						popupStack.Push(newPopup);

						popupDictionary[popupType] = newPopup;
					}
				});
			}
		}
		else
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

	public void SetMainCanvas(BoardGameCanvasBase canvas)
	{
		boardGameMainCanvas = canvas;
	}

	public void OpenMainCanvas()
	{
		if (boardGameMainCanvas != null)
		{
			boardGameMainCanvas.gameObject.SetActive(true);
		}
	}

	public void CloseMainCanvas()
	{
		if (boardGameMainCanvas != null)
		{
			boardGameMainCanvas.gameObject.SetActive(false);
		}
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
