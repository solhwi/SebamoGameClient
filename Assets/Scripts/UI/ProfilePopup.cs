using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePopup : BoardGamePopup
{
	public class Parameter : UIParameter
	{
		public readonly string playerGroup = string.Empty;
		public readonly string playerName = string.Empty;

		public Parameter(string playerGroup, string playerName)
		{
			this.playerGroup = playerGroup;
			this.playerName = playerName;
		}
	}

	[SerializeField] private ProfileSetter profileSetter;
	[SerializeField] private Text playerGroupText;
	[SerializeField] private Text playerNameText;
	
	private string playerGroup = string.Empty;
	private string playerName = string.Empty;

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		if (parameter is Parameter p)
		{
			playerGroup = p.playerGroup;
			playerName = p.playerName;
		}

		playerGroupText.text = playerGroup;
		playerNameText.text = playerName;

		profileSetter.SetPlayerData(playerGroup, playerName);
	}

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Profile;
	}
}
