using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFactory")]
public class DropItemFactory : ScriptableObject
{
	[SerializeField] private Inventory inventory = null;
	[SerializeField] private DropRecipeTable dropRecipeTable = null;

	public DropItem Make(ItemTable.DropItemData data)
	{
		switch (data.dropActionType)
		{
			case DropActionType.Random:
				return new RandomDropItem(dropRecipeTable, inventory, data);
		}

		return new NormalDropItem(dropRecipeTable, inventory, data);
	}
}
