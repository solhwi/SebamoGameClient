
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : BoardGamePopup
{
	public enum TabType
	{
		None = -1,
		Random = 0,
		Normal = 1,
	}

	[SerializeField] private Inventory inventory;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private UINpcView npcView;
	[SerializeField] private ScrollContent scrollContent;
	[SerializeField] private Text npcText;
	[SerializeField] private Text coinText;
	[SerializeField] private string npcDefaultText = "안녕하세요\r\n어서오세요.";

	private List<ShopScrollItem> shopItems = new List<ShopScrollItem>();
	private int normalItemIndex = 0;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Shop;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		var myCharacter = BoardGameManager.Instance.GetMyPlayerCharacter();
		if (myCharacter != null)
		{
			myCharacter.gameObject.SetActive(false);
		}

		normalItemIndex = itemTable.sortedShopItemList.FindIndex(i => i.isRandom == 0);

		npcView.Initialize();
		npcText.text = npcDefaultText;

		scrollContent.onChangedTab += OnChangedTab;
		scrollContent.onUpdateContents += OnUpdateContents;
		scrollContent.onGetItemCount += GetItemCount;

		scrollContent.SelectTab((int)TabType.Random);
	}

	protected override void OnClose()
	{
		scrollContent.onChangedTab -= OnChangedTab;
		scrollContent.onUpdateContents -= OnUpdateContents;
		scrollContent.onGetItemCount -= GetItemCount;

		scrollContent.SelectTab((int)TabType.None);

		var myCharacter = BoardGameManager.Instance.GetMyPlayerCharacter();
		if (myCharacter != null)
		{
			myCharacter.gameObject.SetActive(true);
			myCharacter.Refresh();
		}

		base.OnClose();
	}

	public void OnChangedTab(int tabType)
	{
		FocusItemByTab((TabType)tabType);

		shopItems.Clear();
	}

	public void OnUpdateContents(int index, GameObject contentsObj)
	{
		if (index < 0 || itemTable.sortedShopItemList.Count <= index)
			return;

		if (contentsObj == null)
			return;

		var shopScrollItem = contentsObj.GetComponent<ShopScrollItem>();
		if (shopScrollItem == null)
			return;

		var shopItemData = itemTable.sortedShopItemList[index];
		shopScrollItem.SetItemData(shopItemData);
		shopScrollItem.SetItemClickCallback(OnClickItem);
		shopScrollItem.SetItemClickCallback(OnBuyItem);
		shopScrollItem.SetSelect(false);

		shopItems.Add(shopScrollItem);
	}

	private void OnClickItem(string itemCode)
	{
		npcText.text = itemTable.GetItemNPCDescription(itemCode);

		foreach (var shopItem in shopItems)
		{
			shopItem.SetSelect(shopItem.ItemCode == itemCode);
		}
	}

	public int GetItemCount(int tabType)
	{
		return itemTable.sortedShopItemList.Count;
	}

	private void Update()
	{
		if (normalItemIndex >= scrollContent.DefaultItemCount)
		{
			if (scrollContent.IsTargeting == false)
			{
				float itemSize = scrollContent.GetItemCellSizeY();
				float normalItemPos = scrollContent.GetItemPos(normalItemIndex);
				float currentYPos = scrollContent.GetCurrentYPos();

				if (currentYPos > normalItemPos)
				{
					scrollContent.SelectTab((int)TabType.Normal);
				}
				else if (currentYPos < itemSize)
				{
					scrollContent.SelectTab((int)TabType.Random);
				}
			}
		}
	}

	private void FocusItemByTab(TabType tabType)
	{
		switch(tabType)
		{
			case TabType.Random:
				scrollContent.StartFocusTarget(0);
				break;

			case TabType.Normal:
				scrollContent.StartFocusTarget(normalItemIndex);
				break;
		}
	}

	private IEnumerator OnBuyItem(string itemCode, int buyCount)
	{
		int price = itemTable.GetItemBuyPrice(itemCode);

		bool isSuccess = inventory.TryRemoveItem(ItemTable.Coin, price * buyCount);
		if (isSuccess)
		{
			isSuccess = inventory.TryAddItem(itemCode, buyCount);
			if (isSuccess)
			{
				yield return HttpNetworkManager.Instance.TryPostMyPlayerData((d) =>
				{
					isSuccess = true;
				}, (s) =>
				{
					isSuccess = false;
				});
			}
		}

		string notifyTextStr = string.Empty;
		if (isSuccess)
		{
			notifyTextStr = "구매가 완료되었습니다.";
		}
		else
		{
			notifyTextStr = "구매에 실패하였습니다.";
		}

		UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter(notifyTextStr));
	}
}
