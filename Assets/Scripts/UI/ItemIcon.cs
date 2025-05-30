using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
	[SerializeField] private PressEventTrigger eventTrigger;
	[SerializeField] private Image itemImage;

	private Action<string> onClickItem;
	private Action<string> onPressItem;

	public string itemCode { get; private set; }
	public int itemCount { get; private set; }

	private void Awake()
	{
		eventTrigger.onEndPress += OnPressIcon;
	}

	private void OnDestroy()
	{
		eventTrigger.onEndPress -= OnPressIcon;
	}

	private void Update()
	{
		bool isValid = itemCode != null && itemCode != string.Empty;

		if (itemImage.raycastTarget != isValid)
		{
			itemImage.raycastTarget = isValid;
		}
	}

	public void SetActiveImage(bool isActive)
	{
		float alpha = isActive ? 1.0f : 0.0f;
		itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, alpha);
	}

	public void SetItemData(ItemTable.ShopItemData itemData)
	{
		itemCode = itemData.key;
		itemImage.sprite = ItemTable.Instance.GetItemToolTipIconSprite(itemCode);
	}

	public void SetBuffItemData(string itemCode)
	{
		if (this.itemCode == itemCode && this.itemCount == itemCount)
			return;

		this.itemCode = itemCode;
		itemImage.sprite = ItemTable.Instance.GetBuffItemIconSprite(itemCode);
	}

	public void SetItemData(string itemCode, int itemCount = 1)
	{
		if (this.itemCode == itemCode && this.itemCount == itemCount)
			return;

		this.itemCode = itemCode;
		itemImage.sprite = ItemTable.Instance.GetItemToolTipIconSprite(itemCode);
	}

	public void SetItemClickCallback(Action<string> callback)
	{
		onClickItem = callback;
	}

	public void SetItemPressCallback(Action<string> callback)
	{
		onPressItem = callback;
	}

	public void OnPressIcon(float time)
	{
		if (itemCode == null || itemCode == string.Empty)
			return;

		if (onPressItem != null)
		{
			onPressItem?.Invoke(itemCode);
		}
		else
		{
			UIManager.Instance.TryOpen(PopupType.ItemToolTip, new ItemToolTipParameter(itemCode));
		}
	}

	public void OnClickIcon()
	{
		onClickItem?.Invoke(itemCode);
	}
}
