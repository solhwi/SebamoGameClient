using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldItemFactory")]
public class FieldItemFactory : DataContainer<FieldItemFactory>
{
	private FieldItem Make(ItemTable.FieldItemData data)
	{
		switch (data.actionType)
		{
			case FieldActionType.Random:
				return new RandomFieldItem(data);

			case FieldActionType.Banana:
				return new BananaItem(data);

			case FieldActionType.Barricade:
				return new BarricadeItem(data);

			case FieldActionType.NextDiceOperationBuff:
				return new NextDiceOperationBuffFieldItem(data);

			case FieldActionType.NextDiceChangeBuff:
				return new NextDiceChangeBuffFieldItem(data);
		}

		return new NormalFieldItem(data);
	}

	public FieldItem Make(string itemCode)
	{
		if (itemCode == null)
			return null;

		if (ItemTable.Instance.fieldItemDataDictionary.TryGetValue(itemCode, out var data))
		{
			return Make(data);
		}

		return null;
	}

	public T Make<T>(string itemCode) where T : FieldItem
	{
		if (itemCode == null)
			return null;

		if (ItemTable.Instance.fieldItemDataDictionary.TryGetValue(itemCode, out var data))
		{
			return Make(data) as T;
		}

		return null;
	}
}
