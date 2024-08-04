using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollContent : MonoBehaviour
{
	[SerializeField] private ScrollRect scrollRect;
	[SerializeField] private RectTransform contentRect;
	[SerializeField] private GridLayoutGroup layoutGroup;

	[SerializeField] private List<Toggle> toggles = new List<Toggle>();

	[SerializeField] public int DefaultItemCount = 4;
	[SerializeField] private float tabScrollSpeed = 1.0f;

	[SerializeField] private GameObject prefab;

	private List<GameObject> contentObjList = new List<GameObject>();
	private Vector2 defaultSizeDelta = new Vector2(0, 0);

	public event Action<int> onChangedTab = null;
	public event Action<int, GameObject> onUpdateContents = null;
	public Func<int, int> onGetItemCount = null;

	private int currentTabType = -1;

	private float targetingTime = 0.0f;
	private float targetAnchoredPos = 0.0f;

	public bool IsTargeting { get; private set; }

	public void Reset()
	{
		contentRect.anchoredPosition = Vector2.zero;
	}

	private void Awake()
	{
		defaultSizeDelta = contentRect.sizeDelta;
	}

	private void Update()
	{
		float currentPos = scrollRect.horizontal ?  contentRect.anchoredPosition.x : contentRect.anchoredPosition.y;

		if (MathFloat.IsEqual(currentPos, targetAnchoredPos))
		{
			StopFocusTarget();
		}

		if (IsTargeting)
		{
			targetingTime += Time.deltaTime;

			float nextPos = Mathf.Lerp(currentPos, targetAnchoredPos, targetingTime * tabScrollSpeed);

			if (scrollRect.horizontal)
			{
				contentRect.anchoredPosition = new Vector2(nextPos, 0);
			}
			else
			{
				contentRect.anchoredPosition = new Vector2(0, nextPos);
			}
		}
	}

	public void SelectTab(int tabType)
	{
		if (tabType < 0)
		{
			currentTabType = tabType;
			return;
		}

		bool isOn = toggles[tabType].isOn;
		if (isOn == false)
		{
			toggles[tabType].isOn = true;
		}
		else
		{
			OnSelectedTab();
		}
	}

	private void OnChangedTab(int tabType)
	{
		if (currentTabType == tabType)
			return;

		currentTabType = tabType;

		onChangedTab?.Invoke(currentTabType);

		UpdateContents();
	}

	public void UpdateContents()
	{
		int itemCount = GetItemCount(currentTabType);
		UpdateContents(itemCount);
	}

	private void UpdateContents(int itemCount)
	{
		foreach (var obj in contentObjList)
		{
			Destroy(obj);
		}

		contentObjList.Clear();

		for (int i = 0; i < itemCount; i++)
		{
			var obj = Instantiate(prefab, transform);
			contentObjList.Add(obj);

			onUpdateContents?.Invoke(i, obj);
		}

		if (scrollRect.horizontal)
		{
			ExpandScrollWidth(itemCount);
		}
		else if (scrollRect.vertical)
		{
			ExpandScrollHeight(itemCount);
		}
	}

	public void OnSelectedTab()
	{                             
		int index = toggles.FindIndex(t => t.isOn);
		OnChangedTab(index);
	}

	public void StartFocusTarget(int index)
	{
		targetAnchoredPos = index <= DefaultItemCount ? 0 : GetItemPos(index);

		float maxPos = 0.0f;

		if (scrollRect.horizontal)
		{
			maxPos = Mathf.Max(0, contentRect.sizeDelta.x);
		}
		else
		{
			maxPos = contentRect.sizeDelta.y - GetItemPos(DefaultItemCount);
			maxPos = Mathf.Max(0, maxPos);
		}

		if (targetAnchoredPos >= maxPos)
		{
			targetAnchoredPos = maxPos;
		}

		IsTargeting = true;
		targetingTime = 0.0f;
	}

	public void StopFocusTarget()
	{
		IsTargeting = false;
		targetingTime = 0.0f;
	}

	private void ExpandScrollHeight(int itemCount)
	{
		contentRect.sizeDelta = defaultSizeDelta;

		int expandCount = itemCount - DefaultItemCount;
		if (expandCount > 0)
		{
			float itemCellSize = layoutGroup.cellSize.y;
			float spacingSize = layoutGroup.spacing.y;

			float expandHeight = expandCount * (itemCellSize + spacingSize);
			contentRect.sizeDelta += new Vector2(0, expandHeight);
		}
	}

	public float GetCurrentYPos()
	{
		return contentRect.anchoredPosition.y;
	}

	public float GetItemCellSizeY()
	{
		return GetItemPos(1);
	}

	private void ExpandScrollWidth(int hasItemCount)
	{
		int expandCount = hasItemCount - DefaultItemCount;
		if (expandCount > 0)
		{
			float itemCellSize = layoutGroup.cellSize.x;
			float spacingSize = layoutGroup.spacing.x;

			float expandWidth = expandCount * (itemCellSize + spacingSize);
			contentRect.sizeDelta = new Vector2(expandWidth, contentRect.sizeDelta.y);
		}
		else
		{
			contentRect.sizeDelta = new Vector2(0, contentRect.sizeDelta.y);
		}

		layoutGroup.constraintCount = hasItemCount;
	}

	public float GetItemPos(int index)
	{
		float itemCellSize = 0.0f;
		float spacingSize = 0.0f;

		if (scrollRect.horizontal)
		{
			itemCellSize = layoutGroup.cellSize.x;
			spacingSize = layoutGroup.spacing.x;
		}
		else if(scrollRect.vertical)
		{
			itemCellSize = layoutGroup.cellSize.y;
			spacingSize = layoutGroup.spacing.y;
		}
		

		return index * (itemCellSize + spacingSize);
	}

	public int GetItemCount(int tabType)
	{
		if (onGetItemCount == null)
			return 0;

		return onGetItemCount(tabType);
	}
}
