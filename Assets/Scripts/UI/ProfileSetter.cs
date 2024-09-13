using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ProfileType
{
	Image,
	Frame,
	Max,
}

public class ProfileSetter : MonoBehaviour
{
	[SerializeField] private PressEventTrigger eventTrigger;

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private Image[] profileImage = new Image[(int)ProfileType.Max];

	[SerializeField] private bool isClickable = false;
	[SerializeField] private bool isPressable = false;

	private string playerGroup = string.Empty;
	private string playerName = string.Empty;
	private string playerComment = string.Empty;

	private void Awake()
	{
		eventTrigger.onEndPress += OnPress;
		SetPlayerData(playerDataContainer.playerGroup, playerDataContainer.playerName, playerDataContainer.profileComment);
	}

	private void OnDestroy()
	{
		eventTrigger.onEndPress -= OnPress;
	}

	public void SetPlayerData(string playerGroup, string playerName, string playerComment)
	{
		this.playerGroup = playerGroup;
		this.playerName = playerName;
		this.playerComment = playerComment;
	}

	private void Update()
	{
		for(int i = 0; i < (int)ProfileType.Max; i++)
		{
			SetProfile((ProfileType)i);
		}
	}

	private void SetProfile(ProfileType profileType)
	{
		int type = (int)profileType;

		var playerData = playerDataContainer.GetPlayerData(playerGroup, playerName);
		if (playerData == null)
			return;

		string itemCode = playerData.appliedProfileItems[type];
		profileImage[type].sprite = itemTable.GetItemIconSprite(itemCode);
	}

	public void OnClick()
	{
		if (isClickable == false)
			return;

		if (playerDataContainer.IsMine(playerGroup, playerName))
		{
			UIManager.Instance.TryOpen(PopupType.Profile, new ProfilePopup.Parameter(playerGroup, playerName, playerDataContainer.profileComment));
		}
		else
		{
			UIManager.Instance.TryOpen(PopupType.Profile, new ProfilePopup.Parameter(playerGroup, playerName, playerComment));
		}

	}

	public void OnPress(float time)
	{
		if (isPressable == false)
			return;

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
