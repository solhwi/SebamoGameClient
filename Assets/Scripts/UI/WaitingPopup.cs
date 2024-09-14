using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPopup : MonoBehaviour
{
	[SerializeField] private GameObject spinnerObj;
	[SerializeField] private Text waitingText;


	[SerializeField] private string waitingBaseString = string.Empty;

	[SerializeField] private string waitingAddString = ".";
	[SerializeField] private int maxAddStringCount = 1;

	[SerializeField] private float changeTime = 1.0f;

	private float t = 0.0f;
	private int currentAddCount = 0;

	public void SetActive(bool isActive)
	{
		gameObject.SetActive(isActive);
	}

	private void OnEnable()
	{
		spinnerObj.SetActive(true);
		waitingText.text = waitingBaseString;

		t = 0.0f;
	}

	private void Update()
	{
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

		for(int i = 0; i < addCount; i++)
		{
			sb.Append(waitingAddString);
		}

		return sb.ToString();
	}
}
