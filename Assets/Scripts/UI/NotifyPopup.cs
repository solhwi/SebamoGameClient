using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyPopup : BoardGamePopup
{
	protected override void Reset()
	{
		base.Reset();

		popupType = PopupManager.PopupType.Notify;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);


	}

	protected override void OnClose()
	{

		base.OnClose();
	}

}
