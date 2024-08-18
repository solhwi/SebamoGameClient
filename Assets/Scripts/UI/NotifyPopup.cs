using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum NotifyType
{
	ShopBuy = 0,
	ItemToolTip = 1,
	Random = 2,
	Sell = 3,
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
	public readonly Func<string, int, Task> onClickBuy;

	public ShopBuyUIParameter(ItemTable.ShopItemData shopItemData, Func<string, int, Task> onClickBuy)
	{
		this.shopItemData = shopItemData;
		this.onClickBuy = onClickBuy;
	}
}

public class SellUIParameter : UIParameter
{
	public readonly string itemCode;
	public readonly Func<string, int, Task> onClickSell;

	public SellUIParameter(string itemCode, Func<string, int, Task> onClickBuy)
	{
		this.itemCode = itemCode;
		this.onClickSell = onClickBuy;
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

	[SerializeField] private GameObject confirmButtonObj;
	[SerializeField] private Text confirmButtonText;

	[SerializeField] private CompareTextComponent coinCompareText;
	[SerializeField] private CompareTextComponent itemCompareText;

	[SerializeField] private NotifyStringDictionary notifyTitleDictionary = new NotifyStringDictionary();
	[SerializeField] private NotifyStringDictionary notifyConfirmTextDictionary = new NotifyStringDictionary();
	[SerializeField] private NotifyStringDictionary notifyConfirmButtonTextDictionary = new NotifyStringDictionary();

	private string currentItemCode;
	private int currentItemPrice;
	private int selectedCount;
	private int maxSelectableCount;
	private NotifyType currentNotifyType;

	private Func<string, int, Task> onClickConfirm;
	private Action onClickCancel;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Notify;
		isRecyclable = true;
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
		else if (parameter is SellUIParameter sellParameter)
		{
			currentNotifyType = NotifyType.Sell;
			currentItemCode = sellParameter.itemCode;
			currentItemPrice = itemTable.GetItemSellPrice(currentItemCode);

			maxSelectableCount = inventory.GetHasCount(currentItemCode);
			onClickConfirm = sellParameter.onClickSell;
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
		titleText.text = notifyTitleDictionary[currentNotifyType].Replace("\\n", "\n");
		buyConfirmText.text = notifyConfirmTextDictionary[currentNotifyType].Replace("\\n", "\n");
		confirmButtonText.text = notifyConfirmButtonTextDictionary[currentNotifyType].Replace("\\n", "\n");

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
				confirmButtonObj.SetActive(true);

				coinCompareText.Set(ItemTable.Coin, -1 * currentItemPrice * selectedCount);
				itemCompareText.Set(currentItemCode, selectedCount);

				break;

			case NotifyType.ItemToolTip:
				itemIconImage.gameObject.SetActive(true);
				itemNameText.gameObject.SetActive(true);
				itemDescriptionText.gameObject.SetActive(true);
				itemCountObj.SetActive(false);
				buyConfirmObj.SetActive(false);
				coinCompareText.gameObject.SetActive(false);
				itemCompareText.gameObject.SetActive(false);
				confirmButtonObj.SetActive(false);
				break;

			case NotifyType.Sell:
				itemIconImage.gameObject.SetActive(true);
				itemNameText.gameObject.SetActive(true);
				itemDescriptionText.gameObject.SetActive(true);
				itemCountObj.SetActive(true);
				buyConfirmObj.SetActive(true);
				coinCompareText.gameObject.SetActive(true);
				itemCompareText.gameObject.SetActive(true);
				confirmButtonObj.SetActive(true);

				coinCompareText.Set(ItemTable.Coin, currentItemPrice * selectedCount);
				itemCompareText.Set(currentItemCode, -selectedCount);
				break;
		}

		itemIconImage.sprite = itemTable.GetItemIconSprite(currentItemCode);
		itemNameText.text = itemTable.GetItemName(currentItemCode);
		itemDescriptionText.text = itemTable.GetItemDescription(currentItemCode);

		itemCountText.text = selectedCount.ToString("n0");
	}

	public void OnClickConfirm()
	{
		onClickConfirm?.Invoke(currentItemCode, selectedCount).Wait();
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
