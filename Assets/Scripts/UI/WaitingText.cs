using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingText : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI waitingText;

	[SerializeField] private string waitingAddString = ".";
	[SerializeField] private int maxAddStringCount = 1;

	[SerializeField] private float changeTime = 1.0f;

	private string waitingBaseString = string.Empty;

	private float t = 0.0f;
	private int currentAddCount = 0;

	private bool isProgressing = false;

	public void StartWaiting(string baseString)
	{
		waitingBaseString = baseString;
		waitingText.text = waitingBaseString;

		isProgressing = true;
		t = 0.0f;
		currentAddCount = 0;
	}

	public void SetText(string baseString)
	{
		StopWaiting();

		waitingBaseString = baseString;
		waitingText.text = waitingBaseString;
	}

	public void StopWaiting()
	{
		isProgressing = false;
		t = 0.0f;
		currentAddCount = 0;
	}

	private void Update()
	{
		if (isProgressing == false)
			return;

		t += Time.deltaTime;

		if (t > changeTime)
		{
			currentAddCount = currentAddCount >= maxAddStringCount ? 0 : currentAddCount + 1;
			waitingText.text = GetWaitingText(currentAddCount);

			t = 0.0f;
		}
	}

	private string GetWaitingText(int addCount)
	{
		StringBuilder sb = new StringBuilder(waitingBaseString);

		for (int i = 0; i < addCount; i++)
		{
			sb.Append(waitingAddString);
		}

		return sb.ToString();
	}
}
