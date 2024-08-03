using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTextComponent : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Text diceText;

	void Update()
	{
		diceText.text = playerDataContainer.hasDiceCount.ToString("n0");
	}
}
