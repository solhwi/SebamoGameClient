using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct WorldTileData
{
	public Vector2 tileWorldPosition;
	public Vector3Int tilePosition;
	public TileData tileData;
	public TileBase tileBase;

	public WorldTileData(Vector3Int tilePos, TileData tileData, TileBase tileBase)
	{
		this.tileWorldPosition = ConvertWorldPos(tilePos);
		this.tilePosition = tilePos;
		this.tileData = tileData;
		this.tileBase = tileBase;
	}

	private static Vector2 ConvertWorldPos(Vector3Int tilePos)
	{
		float x = tilePos.y * -1 + tilePos.x; // y에 1을 더하면 -1, x에 1을 더하면 1
		float y = tilePos.y * 0.5f + tilePos.x * 0.5f; // y에 1을 더하면 0.5, x에 1을 더하면 0.5 

		return new Vector2(x, y);
	}
}

public class TileDataManager : MonoBehaviour
{
	[SerializeField] private Tilemap tilemap;
	[SerializeField] private TileDataContainer dataContainer;

	public WorldTileData[] tileBoardDatas = null;

	private void Awake()
	{
		tileBoardDatas = MakeBoardData().ToArray();
	}

	public IEnumerable<WorldTileData> MakeBoardData()
	{
		for (int x = tilemap.cellBounds.xMax; x >= tilemap.cellBounds.xMin; x--)
		{
			for (int y = tilemap.cellBounds.yMax; y >= tilemap.cellBounds.yMin; y--)
			{
				var tilePos = new Vector3Int(x, y);
				TileBase curTile = tilemap.GetTile(tilePos);
				if (curTile != null)
				{
					TileData data = default;
					curTile.GetTileData(tilePos, tilemap, ref data);

					yield return new WorldTileData(tilePos, data, curTile);
				}
			}
		}
	}

	/// <summary>
	/// 현재 위치부터, 목표 위치까지 경로 반환
	/// </summary>
	/// <param name="currentIndex"></param>
	/// <param name="progressCount"></param>
	/// <returns></returns>
	public IEnumerable<WorldTileData> GetTilePath(int currentOrderIndex, int progressCount)
	{
		for(int i = currentOrderIndex + 1; i <= currentOrderIndex + progressCount; i++)
		{
			int tileIndex = GetTileIndexByOrder(i);
			if (tileIndex >= tileBoardDatas.Length)
				yield break;

			yield return tileBoardDatas[tileIndex];
		}
	}

	/// <summary>
	/// 현재 타일 순서에 맞는 타일 인덱스 가져오기
	/// </summary>
	/// <param name="currentOrderIndex"></param>
	/// <returns></returns>
	public int GetTileIndexByOrder(int currentOrderIndex)
	{
		if (dataContainer == null)
			return 0;

		var orderMap = dataContainer.tileOrders;

		// 넘어간 경우 오더가 가장 큰 타일 인덱스 돌려주기
		if (currentOrderIndex >= orderMap.Length)
		{
			int maxOrder = orderMap.Max();

			for (int i = 0; i < orderMap.Length; i++)
			{
				if (orderMap[i] == maxOrder)
				{
					return i;
				}
			}
		}

		for(int i = 0; i < orderMap.Length; i++)
		{
			if (orderMap[i] == currentOrderIndex)
			{
				return i;
			}
		}

		return 0;
	}

	/// <summary>
	/// 현재 타일 순서에 맞는 타일 데이터 가져오기
	/// </summary>
	/// <param name="currentOrderIndex"></param>
	/// <returns></returns>
	public WorldTileData GetTileDataByOrder(int currentOrderIndex)
	{
		int tileIndex = GetTileIndexByOrder(currentOrderIndex);

		if (tileBoardDatas.Length <= tileIndex)
		{
			Debug.LogError($"{currentOrderIndex}가 타일 데이터 인덱스를 넘어감");
			return default;
		}	

		return tileBoardDatas[tileIndex];
	}
}
