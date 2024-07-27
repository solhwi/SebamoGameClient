using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NotifyType
{
	ShopBuy = 0,
	ItemToolTip = 1,
	Random = 2,
}

public class ItemToolTipParameter : UIParameter
{
	public readonly string itemCode;

	public ItemToolTipParameter(string itemCode)
	{
		this.itemCode = itemCode;
	}
}

public class ShopBuyUIParameter : UIParameter
{
	public readonly ItemTable.ShopItemData shopItemData = null;
	public readonly Action<int> onClickBuy;

	public ShopBuyUIParameter(ItemTable.ShopItemData shopItemData, Action<int> onClickBuy)
	{
		this.shopItemData = shopItemData;
		this.onClickBuy = onClickBuy;
	}
}

[System.Serializable]
public class NotifyStringDictionary : SerializableDictionary<NotifyType, string> { }

public class NotifyPopup : BoardGamePopup
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private ItemTable itemTable;

	[SerializeField] private Text titleText;
	[SerializeField] private Image itemIconImage;
	[SerializeField] private Text itemNameText;
	[SerializeField] private Text itemDescriptionText;

	[SerializeField] private GameObject itemCountObj;
	[SerializeField] private Text itemCountText;
	[SerializeField] private GameObject buyConfirmObj;
	[SerializeField] private Text buyConfirmText;

	[SerializeField] private CompareTextComponent coinCompareText;
	[SerializeField] private CompareTextComponent itemCompareText;

	[SerializeField] private NotifyStringDictionary notifyTitleDictionary = new NotifyStringDictionary();
	[SerializeField] private NotifyStringDictionary notifyConfirmTextDictionary = new NotifyStringDictionary();

	private string currentItemCode;
	private int currentItemPrice;
	private int selectedCount;
	private int maxSelectableCount;
	private NotifyType currentNotifyType;

	private Action<int> onClickConfirm;
	private Action onClickCancel;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupManager.PopupType.Notify;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		selectedCount = 1;

		if (parameter is ShopBuyUIParameter shopBuyParameter)
		{
			var data = shopBuyParameter.shopItemData;
			if (data != null)
			{
				currentNotifyType = NotifyType.ShopBuy;
				currentItemCode = data.key;
				currentItemPrice = data.price;

				int hasCoinCount = inventory.GetHasCoinCount();
				maxSelectableCount = hasCoinCount / data.price;

				onClickConfirm = shopBuyParameter.onClickBuy;
			}
		}
		else if (parameter is ItemToolTipParameter itemToolTipParameter)
		{
			currentNotifyType = NotifyType.ItemToolTip;
			currentItemCode = itemToolTipParameter.itemCode;
		}
		else
		{
			titleText.text = string.Empty;
		}

		Refresh();
	}

	protected override void OnClose()
	{
		onClickConfirm = null;
		onClickCancel = null;

		base.OnClose();
	}

	private void Refresh()
	{
		titleText.text = notifyTitleDictionary[currentNotifyType];
		buyConfirmText.text = notifyConfirmTextDictionary[currentNotifyType];

		switch (currentNotifyType)
		{
			case NotifyType.ShopBuy:
				itemIconImage.gameObject.SetActive(true);
				itemNameText.gameObject.SetActive(true);
				itemDescriptionText.gameObject.SetActive(true);
				itemCountObj.SetActive(true);
				buyConfirmObj.SetActive(true);
				coinCompareText.gameObject.SetActive(true);
				itemCompareText.gameObject.SetActive(true);
				break;

			case NotifyType.ItemToolTip:
				itemIconImage.gameObject.SetActive(true);
				itemNameText.gameObject.SetActive(true);
				itemDescriptionText.gameObject.SetActive(true);
				itemCountObj.SetActive(false);
				buyConfirmObj.SetActive(false);
				coinCompareText.gameObject.SetActive(false);
				itemCompareText.gameObject.SetActive(false);
				break;
		}

		itemIconImage.sprite = itemTable.GetItemIconSprite(currentItemCode);
		itemNameText.text = itemTable.GetItemName(currentItemCode);
		itemDescriptionText.text = itemTable.GetItemDescription(currentItemCode);

		itemCountText.text = selectedCount.ToString("n0");

		coinCompareText.Set("Coin", -1 * currentItemPrice * selectedCount);
		itemCompareText.Set(currentItemCode, selectedCount);
	}

	public void OnClickConfirm()
	{
		onClickConfirm?.Invoke(selectedCount);
		OnClickClose();
	}

	public void OnClickCancel()
	{
		onClickCancel?.Invoke();
		OnClickClose();
	}

	public void OnClickPlus()
	{
		if (maxSelectableCount > selectedCount)
		{
			selectedCount++;
		}

		Refresh();
	}

	public void OnClickMinus()
	{
		if (1 < selectedCount)
		{
			selectedCount--;
		}

		Refresh();
	}
}
