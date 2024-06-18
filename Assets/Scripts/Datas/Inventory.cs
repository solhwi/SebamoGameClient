using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory")]
public class Inventory : ScriptableObject
{
	public List<string> hasItemCodes = new List<string>();
	public int coinCount;

	public void PushItem(string itemCode)
	{
		if (hasItemCodes.Contains(itemCode) == false)
		{
			hasItemCodes.Add(itemCode);
		}
	}
}
