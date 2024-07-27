using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
	[SerializeField] private PressEventTrigger eventTrigger;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Image itemImage;

	private Action<string> onClickItem;
	private string itemCode;

	private void Awake()
	{
		eventTrigger.onEndPress += OnPressIcon;
	}

	private void OnDestroy()
	{
		eventTrigger.onEndPress -= OnPressIcon;
	}

	public void SetItemData(ItemTable.ShopItemData itemData)
	{
		itemCode = itemData.key;
		itemImage.sprite = itemTable.GetItemIconSprite(itemCode);
	}

	public void SetItemData(string itemCode, int itemCount = 1)
	{
		this.itemCode = itemCode;
		itemImage.sprite = itemTable.GetItemIconSprite(itemCode);
	}

	public void SetItemClickCallback(Action<string> callback)
	{
		onClickItem = callback;
	}

	public void OnPressIcon(float time)
	{
		PopupManager.Instance.TryOpen(PopupManager.PopupType.Notify, new ItemToolTipParameter(itemCode));
	}

	public void OnClickIcon()
	{
		onClickItem?.Invoke(itemCode);
	}
}
