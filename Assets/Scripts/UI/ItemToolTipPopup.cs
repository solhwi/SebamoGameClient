
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum ToolTipType
{
	ItemToolTip = 0,
	ShopBuy = 1,
	Sell = 2,
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
	public readonly Func<string, int, IEnumerator> onClickBuy;

	public ShopBuyUIParameter(ItemTable.ShopItemData shopItemData, Func<string, int, IEnumerator> onClickBuy)
	{
		this.shopItemData = shopItemData;
		this.onClickBuy = onClickBuy;
	}
}

public class SellUIParameter : UIParameter
{
	public readonly string itemCode;
	public readonly Func<string, int, IEnumerator> onClickSell;

	public SellUIParameter(string itemCode, Func<string, int, IEnumerator> onClickSell)
	{
		this.itemCode = itemCode;
		this.onClickSell = onClickSell;
	}
}

[System.Serializable]
public class ToolTipStringDictionary : SerializableDictionary<ToolTipType, string> { }

public class ItemToolTipPopup : BoardGamePopup
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

	[SerializeField] private ToolTipStringDictionary notifyTitleDictionary = new ToolTipStringDictionary();
	[SerializeField] private ToolTipStringDictionary notifyConfirmTextDictionary = new ToolTipStringDictionary();
	[SerializeField] private ToolTipStringDictionary notifyConfirmButtonTextDictionary = new ToolTipStringDictionary();

	private string currentItemCode;
	private int currentItemPrice;
	private int selectedCount;
	private int maxSelectableCount;

	private ToolTipType currentToolTipType;

	private Func<string, int, IEnumerator> onClickConfirm;
	private Action onClickCancel;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.ItemToolTip;
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
				currentToolTipType = ToolTipType.ShopBuy;
				currentItemCode = data.key;
				currentItemPrice = data.price;

				int hasCoinCount = inventory.GetHasCoinCount();
				maxSelectableCount = hasCoinCount / data.price;

				onClickConfirm = shopBuyParameter.onClickBuy;
			}
		}
		else if (parameter is ItemToolTipParameter itemToolTipParameter)
		{
			currentToolTipType = ToolTipType.ItemToolTip;
			currentItemCode = itemToolTipParameter.itemCode;
		}
		else if (parameter is SellUIParameter sellParameter)
		{
			currentToolTipType = ToolTipType.Sell;
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
		titleText.text = notifyTitleDictionary[currentToolTipType].Replace("\\n", "\n");
		buyConfirmText.text = notifyConfirmTextDictionary[currentToolTipType].Replace("\\n", "\n");
		confirmButtonText.text = notifyConfirmButtonTextDictionary[currentToolTipType].Replace("\\n", "\n");

		switch (currentToolTipType)
		{
			case ToolTipType.ShopBuy:
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

			case ToolTipType.ItemToolTip:
				itemIconImage.gameObject.SetActive(true);
				itemNameText.gameObject.SetActive(true);
				itemDescriptionText.gameObject.SetActive(true);
				itemCountObj.SetActive(false);
				buyConfirmObj.SetActive(false);
				coinCompareText.gameObject.SetActive(false);
				itemCompareText.gameObject.SetActive(false);
				confirmButtonObj.SetActive(false);
				break;

			case ToolTipType.Sell:
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
		StartCoroutine(ProcessConfirm());
	}

	private IEnumerator ProcessConfirm()
	{
		yield return onClickConfirm.Invoke(currentItemCode, selectedCount);
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
