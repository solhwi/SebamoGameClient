using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPopup : BoardGamePopup
{
	public class Parameter : UIParameter
	{
		public readonly string waitingBaseText = string.Empty;

		public Parameter(string waitingBaseText)
		{
			this.waitingBaseText = waitingBaseText;
		}
	}

	[SerializeField] private GameObject spinnerObj;
	[SerializeField] private Text waitingText;

	[SerializeField] private string waitingAddString = ".";
	[SerializeField] private int maxAddStringCount = 1;

	[SerializeField] private float changeTime = 1.0f;

	private string waitingBaseString = string.Empty;

	private float t = 0.0f;
	private int currentAddCount = 0;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Wait;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		if (parameter is Parameter p)
		{
			waitingBaseString = p.waitingBaseText;
			waitingText.text = waitingBaseString;
		}

		spinnerObj.SetActive(true);
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
