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
	[SerializeField] private WaitingText waitingText;

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
			waitingText.StartWaiting(p.waitingBaseText);
		}

		spinnerObj.SetActive(true);
	}

	protected override void OnClose()
	{
		base.OnClose();

		waitingText.StopWaiting();
	}

}
