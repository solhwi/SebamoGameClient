using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ProfileType
{
	Image,
	Frame,
	Max,
}

public class ProfileSetter : MonoBehaviour
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Image[] profileImage = new Image[(int)ProfileType.Max];

	private void Update()
	{
		for(int i = 0; i < (int)ProfileType.Max; i++)
		{
			SetProfile((ProfileType)i);
		}
	}

	private void SetProfile(ProfileType profileType)
	{
		int type = (int)profileType;

		string itemCode = inventory.appliedProfileItems[type];
		profileImage[type].sprite = itemTable.GetItemIconSprite(itemCode);
	}
}
