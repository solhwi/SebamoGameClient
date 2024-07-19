using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
	[SerializeField] private PressEventTrigger eventTrigger;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Image itemImage;

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
		string itemCode = itemData.key;

		if (itemTable.itemIconDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			var sprite = ResourceManager.Instance.Load<Sprite>(iconData.GetAssetPathWithoutResources());
			itemImage.sprite = sprite;
		}
	}

    public void SetItemData(string itemCode, int itemCount = 1)
	{
		
	}

	public void OnPressIcon(float time)
	{
		PopupManager.Instance.TryOpen(PopupManager.PopupType.Notify);
	}
}
