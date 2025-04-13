using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffItemComponent : MonoBehaviour
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private List<ItemIcon> buffItemIcons = new List<ItemIcon>();

	private void Update()
	{
		for (int i = 0; i < buffItemIcons.Count; i++)
		{
			if (inventory.appliedBuffItems.Count <= i)
			{
				buffItemIcons[i].gameObject.SetActive(false);
			}
			else
			{
				string buffItemCode = inventory.appliedBuffItems.ElementAt(i);
				buffItemIcons[i].SetItemData(buffItemCode);

				buffItemIcons[i].gameObject.SetActive(true);
			}
		}
	}
}
