using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
	[SerializeField] private Image view;

    public void SetItem(string itemCode, int itemCount = 1)
	{
		gameObject.SetActive(true);
	}
}
