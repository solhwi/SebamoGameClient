using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory")]
public class Inventory : ScriptableObject
{
	[SerializeField] private ItemTable table;

	[System.Serializable]
	public class HasItemDataDictionary : SerializableDictionary<string, int> { }
	public HasItemDataDictionary hasItems = new HasItemDataDictionary();

	public void AddItem(string itemCode)
	{
		if (table.IsValidItem(itemCode) == false)
			return;

		if (hasItems.ContainsKey(itemCode))
		{
			hasItems[itemCode]++;
		}
		else
		{
			hasItems[itemCode] = 1;
		}
	}

	public int GetHasCoinCount()
	{
		if(hasItems.ContainsKey("Coin") == false)
			return 0;

		return hasItems["Coin"];
	}
}
