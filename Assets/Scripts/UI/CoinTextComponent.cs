using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinTextComponent : MonoBehaviour
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private Text coinText;

	void Update()
    {
		coinText.text = inventory.GetHasCoinCount().ToString("n0");
	}
}
