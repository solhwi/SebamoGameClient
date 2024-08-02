using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSortingComparer : IComparer<int>
{
	private TileDataContainer tileDataContainer;

	public TileSortingComparer(TileDataContainer tileDataContainer)
	{
		this.tileDataContainer = tileDataContainer;
	}

	public int Compare(int tileIndex1, int tileIndex2)
	{
		if (tileDataContainer.tileOrders.Length <= tileIndex1 || tileIndex1 == -1)
			return -1;

		if (tileDataContainer.tileOrders.Length <= tileIndex2 || tileIndex2 == -1)
			return 1;

		int order1 = tileDataContainer.tileOrders[tileIndex1];
		int order2 = tileDataContainer.tileOrders[tileIndex2];

		return order1 > order2 ? 1 : -1;
	}

}

public class RankingBoard : MonoBehaviour
{
	[SerializeField] private TileDataContainer tileDataContainer;
	[SerializeField] private ScrollContent scrollContent;
	[SerializeField] private PlayerDataContainer playerDataContainer;

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
		playerDatas = MakePlayerTileDatas()?.OrderByDescending(d => d.playerTileIndex, tileSortingComparer).ToList();

		if (this.playerDatas.Count != playerDatas.Count)
		{
			return true;
		}

		for (int i = 0; i < this.playerDatas.Count; i++)
		{
			if (this.playerDatas[i] != playerDatas[i])
			{
				return true;
			}
		}

		return false;
	}

	private IEnumerable<PlayerPacketData> MakePlayerTileDatas()
	{
		yield return playerDataContainer.myPlayerPacketData;
		
		foreach (var otherPlayerData in playerDataContainer.otherPlayerPacketDatas)
		{
			yield return otherPlayerData;
		}
	}

	private int GetHasItemCount(int tabType)
	{
		return playerDatas.Count;
	}
}
