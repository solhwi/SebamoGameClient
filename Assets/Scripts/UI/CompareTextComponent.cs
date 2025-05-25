using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CompareTextComponent : MonoBehaviour
{
	[SerializeField] private Text beforeText;
	[SerializeField] private Text afterText;

	private int beforeCount;
	private int afterCount;

	public void Set(string itemCode, int diffCount)
	{
		beforeCount = Inventory.Instance.GetHasCount(itemCode);
		afterCount = beforeCount + diffCount;

		beforeText.text = beforeCount.ToString();
		afterText.text = afterCount.ToString();
	}

	public void SetDiffCount(int diffCount)
	{
		int afterCount = beforeCount + diffCount;
		afterText.text = afterCount.ToString();
	}
}
