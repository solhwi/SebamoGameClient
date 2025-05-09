using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileSortingComparer : IComparer<int>
{
	private TileDataContainer tileDataContainer;

	public TileSortingComparer(TileDataContainer tileDataContainer)
	{
		this.tileDataContainer = tileDataContainer;
	}

	public int Compare(int tileOrder1, int tileOrder2)
	{
		return tileOrder1 > tileOrder2 ? 1 : -1;
	}

}

public class RankingBoard : MonoBehaviour
{
	[SerializeField] private TileDataContainer tileDataContainer;
	[SerializeField] private ScrollContent scrollContent;
	[SerializeField] private PlayerDataContainer playerDataContainer;

	[SerializeField] private Image rankingToggleImage = null;
	[SerializeField] private Sprite[] rankingImages = null;

	private TileSortingComparer tileSortingComparer = null;
	private List<PlayerPacketData> playerDatas = new List<PlayerPacketData>();

	private void Awake()
	{
		tileSortingComparer = new TileSortingComparer(tileDataContainer);
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
		if (IsDirtyPlayerTileIndex(out var currentPlayerDatas) == false)
			return;

		playerDatas.Clear();
		playerDatas.AddRange(currentPlayerDatas);

		int myPlayerIndex = playerDatas.FindIndex(p => playerDataContainer.IsMine(p.playerGroup, p.playerName));
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
		playerDatas = playerDataContainer.GetAllPlayerData()?.OrderByDescending(d => d.playerTileOrder, tileSortingComparer).ToList();

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
