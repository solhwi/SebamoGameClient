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

	private List<WorldTileData> tileBoardDatas = new List<WorldTileData>();

	private void Awake()
	{
		tileBoardDatas = MakeBoardData().ToList();
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
	/// <param name="nextIndex"></param>
	/// <returns></returns>
	public IEnumerable<WorldTileData> GetTilePath(int currentIndex, int nextIndex)
	{
		for(int i = currentIndex + 1; i < nextIndex; i++)
		{
			if (i >= tileBoardDatas.Count)
				yield break;

			yield return tileBoardDatas[i];
		}
	}
}
