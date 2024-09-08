using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankScrollItem : MonoBehaviour
{
	[SerializeField] private Text rankText;
	[SerializeField] private Image playerImage;
	[SerializeField] private Image playerFrameImage;
	[SerializeField] private Text playerNameText;

	private string playerGroup = string.Empty;
	private string playerName = string.Empty;

	public void SetData(PlayerPacketData data, int rankNumber)
	{
		playerGroup = data.playerGroup;
		playerName = data.playerName;

		playerNameText.text = playerName;
		rankText.text = rankNumber.ToString();
	}

	public void OnClick()
	{
		var character = BoardGameManager.Instance.GetPlayerCharacter(playerGroup, playerName);
		if (character == null)
			return;

		CameraController.Instance.ChangeTarget(character.transform);
		CameraController.Instance.SetFollow(true);
	}
}
