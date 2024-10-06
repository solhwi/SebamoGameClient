using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPopup : BoardGamePopup
{
	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Setting;
	}
}
