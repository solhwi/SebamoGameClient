using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfilePopup : BoardGamePopup
{
	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Profile;
	}
}
