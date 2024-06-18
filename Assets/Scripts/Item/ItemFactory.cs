using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFactory")]
public class ItemFactory : ScriptableObject
{
	[SerializeField] private Inventory inventory = null;
	[SerializeField] private ItemTable itemTable = null;

	public ItemBase Make(string itemKeyCode)
	{
		switch(itemKeyCode)
		{
			case "UnityChanAccessory":
			case "GreatSword":
				return new DropItem(inventory, itemTable, itemKeyCode);
		}

		return null;
	}
}
