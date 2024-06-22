using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory")]
public class Inventory : ScriptableObject
{
	[SerializeField] private ItemTable table;
	public List<string> hasItemCodes = new List<string>();
	public int coinCount;

	public void PushItem(string itemCode)
	{
		if (table.IsValidItem(itemCode) == false)
			return;

		if (hasItemCodes.Contains(itemCode))
			return;

		hasItemCodes.Add(itemCode);
	}
}
