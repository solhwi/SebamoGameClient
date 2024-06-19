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
		// 존재하는 아이템인 지 확인하고 넣자

		if (hasItemCodes.Contains(itemCode) == false)
		{
			hasItemCodes.Add(itemCode);
		}
	}
}
