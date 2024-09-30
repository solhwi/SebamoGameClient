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
		public readonly bool bUseBackGround = false;

		public Parameter(string waitingBaseText, bool bUseBackGround = false)
		{
			this.waitingBaseText = waitingBaseText;
			this.bUseBackGround = bUseBackGround;
		}
	}

	[SerializeField] private Image backgroundImage;
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
			backgroundImage.color = backgroundImage.color.Alpha(p.bUseBackGround ? 1f : 0f);
		}

		spinnerObj.SetActive(true);
	}

	protected override void OnClose()
	{
		base.OnClose();

		waitingText.StopWaiting();
	}

}
