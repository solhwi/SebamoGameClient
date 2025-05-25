using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RankingBoard : MonoBehaviour
{
	[SerializeField] private ScrollContent scrollContent;

	[SerializeField] private Image rankingToggleImage = null;
	[SerializeField] private Sprite[] rankingImages = null;

	private List<PlayerPacketData> playerDatas = new List<PlayerPacketData>();

	private void Awake()
	{
		playerDatas.Clear();
	}

	private void OnEnable()
	{
		scrollContent.onUpdateContents += OnUpdateContents;
		scrollContent.onGetItemCount += GetHasItemCount;
	}

	private void OnDisable()
	{
		scrollContent.onUpdateContents -= OnUpdateContents;
		scrollContent.onGetItemCount -= GetHasItemCount;
	}

	private void Update()
	{
		if (PlayerDataContainer.Instance == null)
			return;

		if (IsDirtyPlayerTileIndex(out var currentPlayerDatas) == false)
			return;

		playerDatas.Clear();
		playerDatas.AddRange(currentPlayerDatas);

		int myPlayerIndex = playerDatas.FindIndex(p => PlayerDataContainer.Instance.IsMine(p.playerGroup, p.playerName));
		myPlayerIndex = Math.Clamp(myPlayerIndex, 0, rankingImages.Length - 1);

		rankingToggleImage.sprite = rankingImages[myPlayerIndex];

		scrollContent.UpdateContents();
	}

	private void OnUpdateContents(int index, GameObject contentObj)
	{
		if (index < 0 || playerDatas.Count <= index)
			return;

		var scrollItem = contentObj.GetComponent<RankScrollItem>();
		if (scrollItem == null) 
			return;

		scrollItem.SetData(playerDatas[index], index + 1);
	}
	
	private bool IsDirtyPlayerTileIndex(out List<PlayerPacketData> playerDatas)
	{
		playerDatas = new List<PlayerPacketData>();

		if (PlayerDataContainer.Instance == null)
			return false;

		var allPlayerData = PlayerDataContainer.Instance.GetAllPlayerData();
		if (allPlayerData == null || allPlayerData.Any() == false)
			return false;

		playerDatas = allPlayerData.OrderByDescending(d => d.playerTileOrder).ToList();

		if (this.playerDatas.Count != playerDatas.Count)
		{
			return true;
		}

		for (int i = 0; i < this.playerDatas.Count; i++)
		{
			if (this.playerDatas[i].playerTileOrder != playerDatas[i].playerTileOrder)
			{
				return true;
			}
		}

		return false;
	}

	private int GetHasItemCount(int tabType)
	{
		return playerDatas.Count;
	}
}
