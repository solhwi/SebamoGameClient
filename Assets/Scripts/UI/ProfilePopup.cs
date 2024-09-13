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

	private bool IsMine => playerDataContainer.IsMine(playerGroup, playerName);

	[SerializeField] private PlayerDataContainer playerDataContainer;

	[SerializeField] private ProfileSetter profileSetter;
	[SerializeField] private Text playerGroupText;
	[SerializeField] private Text playerNameText;
	[SerializeField] private InputField playerCommentField;

	[SerializeField] private Button editButton;
	[SerializeField] private Button editCompleteButton;
	
	private string playerGroup = string.Empty;
	private string playerName = string.Empty;
	private string playerComment = string.Empty;

	private bool isEditing = false;

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

		isEditing = false;

		editButton.gameObject.SetActive(IsMine);

		profileSetter.SetPlayerData(playerGroup, playerName);
	}

	private void Update()
	{
		if (playerCommentField.interactable != isEditing)
		{
			playerCommentField.interactable = isEditing;

			if (isEditing)
			{
				playerCommentField.ActivateInputField();
			}
		}

		editButton.gameObject.SetActive(isEditing == false);
		editCompleteButton.gameObject.SetActive(isEditing);
	}

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Profile;
	}

	public void OnClickEdit()
	{
		if (IsMine == false)
			return;

		isEditing = true;
	}

	public void OnEndEdit()
	{
		playerComment = playerCommentField.text;
		playerDataContainer.profileComment = playerComment;

		isEditing = false;
	}
}
