using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFactory")]
public class ItemFactory : ScriptableObject
{
	[SerializeField] private Inventory inventory = null;

    public ItemBase Make(ItemTable.ItemData rawData)
	{
		switch(rawData.key)
		{
			case "Coin":
				return new DropItem(inventory, rawData);
		}

		return null;
	}
}
