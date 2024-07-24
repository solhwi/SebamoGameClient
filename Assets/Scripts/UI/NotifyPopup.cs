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
	public readonly Action onClickBuy;

	public ShopBuyUIParameter(ItemTable.ShopItemData shopItemData, Action onClickBuy)
	{
		this.shopItemData = shopItemData;
		this.onClickBuy = onClickBuy;
	}
}

[System.Serializable]
public class NotifyTitleDictionary : SerializableDictionary<NotifyType, string> { }

public class NotifyPopup : BoardGamePopup
{
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Text titleText;
	[SerializeField] private Image itemIconImage;
	[SerializeField] private Text itemNameText;
	[SerializeField] private Text itemDescriptionText;

	[SerializeField] private NotifyTitleDictionary notifyTitleDictionary = new NotifyTitleDictionary();

	private Action onClickConfirm;
	private Action onClickCancel;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupManager.PopupType.Notify;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		if (parameter is ShopBuyUIParameter shopBuyParameter)
		{
			titleText.text = notifyTitleDictionary[NotifyType.ShopBuy];
			var data = shopBuyParameter.shopItemData;
			if (data != null)
			{
				itemIconImage.sprite = itemTable.GetItemIconSprite(data.key);
				itemNameText.text = itemTable.GetItemName(data.key);
				itemDescriptionText.text = itemTable.GetItemDescription(data.key);
				onClickConfirm = shopBuyParameter.onClickBuy;
			}
		}
		else if(parameter is ItemToolTipParameter itemToolTipParameter)
		{
			titleText.text = notifyTitleDictionary[NotifyType.ItemToolTip];
			string itemCode = itemToolTipParameter.itemCode;
			if (itemCode != string.Empty)
			{
				itemIconImage.sprite = itemTable.GetItemIconSprite(itemCode);
				itemNameText.text = itemTable.GetItemName(itemCode);
				itemDescriptionText.text = itemTable.GetItemDescription(itemCode);
			}
		}
		else
		{
			titleText.text = string.Empty;
			itemIconImage.sprite = null;
			itemNameText.text = string.Empty;
			itemDescriptionText.text = string.Empty;
		}
	}

	protected override void OnClose()
	{
		onClickConfirm = null;
		onClickCancel = null;

		base.OnClose();
	}

	public void OnClickConfirm()
	{
		onClickConfirm?.Invoke();
		OnClickClose();
	}

	public void OnClickCancel()
	{
		onClickCancel?.Invoke();
		OnClickClose();
	}

}
