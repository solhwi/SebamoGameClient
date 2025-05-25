using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTextComponent : MonoBehaviour
{
	[SerializeField] private Text diceText;

	void Update()
	{
		diceText.text = PlayerDataContainer.Instance.hasDiceCount.ToString("n0");
	}
}
