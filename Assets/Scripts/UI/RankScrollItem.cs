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

    public void SetData(PlayerPacketData data, int rankNumber)
	{
		rankText.text = rankNumber.ToString();
		playerNameText.text = data.playerName;
	}
}
