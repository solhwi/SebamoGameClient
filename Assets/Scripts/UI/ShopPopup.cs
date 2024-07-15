using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MathFloat
{
	public static bool IsEqual(float x, float y)
	{
		if (x <= y + Mathf.Epsilon &&
			x >= y - Mathf.Epsilon)
		{
			return true;
		}

		return false;
	}
}

public class ShopPopup : BoardGamePopup
{
	public enum TabType
	{
		None = -1,
		Random = 0,
		Normal = 1,
	}

	[SerializeField] private ItemTable itemTable;
	[SerializeField] private RectTransform contentRect;
	[SerializeField] private GridLayoutGroup layoutGroup;

	[SerializeField] private List<Toggle> toggles = new List<Toggle>();
	[SerializeField] private ShopItem shopItemPrefab;
	[SerializeField] private int defaultItemCount;

	[SerializeField] private float tabScrollSpeed = 1.0f;

	private List<ShopItem> currentShopItems = new List<ShopItem>();
	private TabType currentTabType = TabType.None;

	private int normalItemIndex = 0;
	private float normalItemYPos = 0;

	private float targetingTime = 0.0f;
	private float targetAnchoredPosY = 0.0f;

	private bool isTargeting = false;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupManager.PopupType.Shop;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		normalItemIndex = itemTable.sortedShopItemList.FindIndex(i => i.isRandom == 0);
		normalItemYPos = GetItemPos(normalItemIndex);

		foreach (var itemIcon in currentShopItems)
		{
			Destroy(itemIcon.gameObject);
		}

		currentShopItems.Clear();

		foreach (var shopItemData in itemTable.sortedShopItemList)
		{
			string itemCode = shopItemData.key;

			var itemIcon = Instantiate(shopItemPrefab, contentRect);
			itemIcon.SetItem(itemCode);

			currentShopItems.Add(itemIcon);
		}

		ExpandScrollSize(currentShopItems.Count);

		OnChangedTab(TabType.Random);
	}

	private float GetItemPos(int index)
	{
		float itemCellSize = layoutGroup.cellSize.y;
		float spacingSize = layoutGroup.spacing.y;

		return index * (itemCellSize + spacingSize);
	}

	private void Update()
	{
		if (normalItemIndex < defaultItemCount)
			return;

		float currentY = contentRect.anchoredPosition.y;

		if (MathFloat.IsEqual(currentY, targetAnchoredPosY))
		{
			StopTargetingAnchoredPosition();
		}

		if (isTargeting)
		{
			targetingTime += Time.deltaTime;

			float nextY = Mathf.Lerp(currentY, targetAnchoredPosY, targetingTime * tabScrollSpeed);
			contentRect.anchoredPosition = new Vector2(0, nextY);
		}
		else
		{
			float itemSize = GetItemPos(1);

			if (currentY > normalItemYPos)
			{
				SelectTab(TabType.Normal);
			}
			else if (currentY < itemSize)
			{
				SelectTab(TabType.Random);
			}
		}
	}

	protected override void OnClose()
	{
		currentTabType = TabType.None;

		foreach (var itemIcon in currentShopItems)
		{
			Destroy(itemIcon.gameObject);
		}

		currentShopItems.Clear();

		base.OnClose();
	}

	private void OnChangedTab(TabType tabType)
	{
		if (currentTabType == tabType)
			return;

		currentTabType = tabType;

		FocusTab(currentTabType);
	}

	private void SelectTab(TabType tabType)
	{
		toggles[(int)tabType].isOn = true;
	}

	private void FocusTab(TabType tabType, bool isForce = false)
	{
		switch(tabType)
		{
			case TabType.Random:

				if (isForce)
				{
					contentRect.anchoredPosition = Vector2.zero;
				}
				else
				{
					targetAnchoredPosY = 0.0f;
					StartTargetingAnchoredPosition();
				}
				
				break;

			case TabType.Normal:

				if (isForce)
				{
					contentRect.anchoredPosition = new Vector2(0, normalItemYPos);
				}
				else
				{
					targetAnchoredPosY = normalItemYPos;
					StartTargetingAnchoredPosition();
				}
				
				break;
		}
	}

	private void ExpandScrollSize(int itemCount)
	{
		contentRect.sizeDelta = new Vector2(0, 750);

		int expandCount = itemCount - defaultItemCount;
		if (expandCount > 0)
		{
			float itemCellSize = layoutGroup.cellSize.y;
			float spacingSize = layoutGroup.spacing.y;

			float expandHeight = expandCount * (itemCellSize + spacingSize);
			contentRect.sizeDelta += new Vector2(0, expandHeight);
		}
	}

	private void StartTargetingAnchoredPosition()
	{
		isTargeting = true;
		targetingTime = 0.0f;
	}

	public void StopTargetingAnchoredPosition()
	{
		isTargeting = false;
		targetingTime = 0.0f;
	}

	public void OnValueChangedIndex()
	{
		int index = toggles.FindIndex(t => t.isOn);
		OnChangedTab((TabType)index);
	}
}
