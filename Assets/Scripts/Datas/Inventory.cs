using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory")]
public class Inventory : ScriptableObject
{
	[SerializeField] private ItemTable table;

	[System.Serializable]
	public class HasItemDataDictionary : SerializableDictionary<string, int> { }
	public HasItemDataDictionary hasItemCodes = new HasItemDataDictionary();

	public void PushItem(string itemCode)
	{
		if (table.IsValidItem(itemCode) == false)
			return;

		if (hasItemCodes.ContainsKey(itemCode))
		{
			hasItemCodes[itemCode]++;
		}
		else
		{
			hasItemCodes[itemCode] = 1;
		}
	}
}
