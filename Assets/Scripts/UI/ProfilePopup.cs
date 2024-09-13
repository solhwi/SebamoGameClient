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
		public readonly string playerComment = string.Empty;

		public Parameter(string playerGroup, string playerName, string playerComment)
		{
			this.playerGroup = playerGroup;
			this.playerName = playerName;
			this.playerComment = playerComment;
		}
	}

	[SerializeField] private PlayerDataContainer playerDataContainer;

	[SerializeField] private ProfileSetter profileSetter;
	[SerializeField] private Text playerGroupText;
	[SerializeField] private Text playerNameText;
	[SerializeField] private InputField playerCommentField;
	
	private string playerGroup = string.Empty;
	private string playerName = string.Empty;
	private string playerComment = string.Empty;

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		if (parameter is Parameter p)
		{
			playerGroup = p.playerGroup;
			playerName = p.playerName;
			playerComment = p.playerComment;
		}

		playerGroupText.text = playerGroup;
		playerNameText.text = playerName;

		playerCommentField.text = playerComment;
		playerCommentField.interactable = false;

		profileSetter.SetPlayerData(playerGroup, playerName, playerComment);
	}

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Profile;
	}

	public void OnClickEdit()
	{
		if (playerDataContainer.IsMine(playerGroup, playerName) == false)
			return;

		playerCommentField.interactable = !playerCommentField.interactable;
	}
}
