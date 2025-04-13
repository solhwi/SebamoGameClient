using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldItemFactory")]
public class FieldItemFactory : ScriptableObject
{
	[SerializeField] private Inventory inventory = null;
	[SerializeField] private ItemTable itemTable = null;

	private FieldItem Make(ItemTable.FieldItemData data)
	{
		switch (data.actionType)
		{
			case FieldActionType.Random:
				return new RandomFieldItem(inventory, data);

			case FieldActionType.Banana:
				return new BananaItem(inventory, data);

			case FieldActionType.Barricade:
				return new BarricadeItem(inventory, data);

			case FieldActionType.NextDiceBuff:
				return new NextDiceBuffFieldItem(inventory, data);
		}

		return new NormalFieldItem(inventory, data);
	}

	public FieldItem Make(string itemCode)
	{
		if (itemCode == null)
			return null;

		if (itemTable.fieldItemDataDictionary.TryGetValue(itemCode, out var data))
		{
			return Make(data);
		}

		return null;
	}

	public T Make<T>(string itemCode) where T : FieldItem
	{
		if (itemCode == null)
			return null;

		if (itemTable.fieldItemDataDictionary.TryGetValue(itemCode, out var data))
		{
			return Make(data) as T;
		}

		return null;
	}
}
