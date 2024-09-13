using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankScrollItem : MonoBehaviour
{
	[SerializeField] private Text rankText;
	[SerializeField] private ProfileSetter profileSetter;
	[SerializeField] private PressEventTrigger trigger;
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Text playerNameText;

	private string playerGroup = string.Empty;
	private string playerName = string.Empty;
	private string playerComment = string.Empty;

	private void Awake()
	{
		trigger.onEndPress += OnPress;
	}

	private void OnDestroy()
	{
		trigger.onEndPress -= OnPress;
	}

	public void SetData(PlayerPacketData data, int rankNumber)
	{
		playerGroup = data.playerGroup;
		playerName = data.playerName;
		playerComment = data.profileComment;

		playerNameText.text = playerName;
		rankText.text = rankNumber.ToString();

		profileSetter.SetPlayerData(playerGroup, playerName, playerComment);
	}

	public void OnClick()
	{
		var character = BoardGameManager.Instance.GetPlayerCharacter(playerGroup, playerName);
		if (character == null)
			return;

		CameraController.Instance.ChangeTarget(character.transform);
		CameraController.Instance.SetFollow(true);
	}

	public void OnPress(float time)
	{
		if (playerDataContainer.IsMine(playerGroup, playerName))
		{
			UIManager.Instance.TryOpen(PopupType.Profile, new ProfilePopup.Parameter(playerGroup, playerName, playerDataContainer.profileComment));
		}
		else
		{
			UIManager.Instance.TryOpen(PopupType.Profile, new ProfilePopup.Parameter(playerGroup, playerName, playerComment));
		}
	}
}
