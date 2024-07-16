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

	public event Action<int> onChangedTab = null;
	public event Action<int, GameObject> onUpdateContents = null;
	public Func<int, int> onGetItemCount = null;

	private int currentTabType = -1;

	private float targetingTime = 0.0f;
	private float targetAnchoredPosY = 0.0f;

	public bool IsTargeting { get; private set; }

	public void Reset()
	{
		contentRect.anchoredPosition = Vector2.zero;
	}

	private void Update()
	{
		float currentY = contentRect.anchoredPosition.y;

		if (MathFloat.IsEqual(currentY, targetAnchoredPosY))
		{
			StopFocusTarget();
		}

		if (IsTargeting)
		{
			targetingTime += Time.deltaTime;

			float nextY = Mathf.Lerp(currentY, targetAnchoredPosY, targetingTime * tabScrollSpeed);

			contentRect.anchoredPosition = new Vector2(0, nextY);
		}
	}

	public void SelectTab(int tabType)
	{
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

		int itemCount = GetItemCount(currentTabType);
		UpdateContents(itemCount);
	}

	public void UpdateContents(int itemCount)
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
		targetAnchoredPosY = GetItemPosY(index);
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
		contentRect.sizeDelta = new Vector2(0, 750);

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
		return GetItemPosY(1);
	}

	private void ExpandScrollWidth(int hasItemCount)
	{
		int expandCount = hasItemCount - DefaultItemCount;
		if (expandCount > 0)
		{
			float itemCellSize = layoutGroup.cellSize.x;
			float spacingSize = layoutGroup.spacing.x;

			float expandWidth = expandCount * (itemCellSize + spacingSize);
			contentRect.offsetMax = new Vector2(expandWidth, 0);
		}

		layoutGroup.constraintCount = hasItemCount;
	}

	public float GetItemPosY(int index)
	{
		float itemCellSize = layoutGroup.cellSize.y;
		float spacingSize = layoutGroup.spacing.y;

		return index * (itemCellSize + spacingSize);
	}

	public int GetItemCount(int tabType)
	{
		if (onGetItemCount == null)
			return 0;

		return onGetItemCount(tabType);
	}
}
