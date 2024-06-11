using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct CustomTileData
{
	public Vector3 tilePosition;
	public TileData tileData;
	public TileBase tileBase;

	public CustomTileData(int x, int y, TileData tileData, TileBase tileBase)
	{
		this.tilePosition = new Vector3(x, y);
		this.tileData = tileData;
		this.tileBase = tileBase;
	}
}

public class TileDataManager : MonoBehaviour
{
	[SerializeField] private Tilemap tilemap;

	private List<CustomTileData> tileBoardDatas = new List<CustomTileData>();

	private void Awake()
	{
		tileBoardDatas = MakeBoardData().ToList();
	}

	private IEnumerable<CustomTileData> MakeBoardData()
	{
		for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
		{
			for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
			{
				TileBase curTile = tilemap.GetTile(new Vector3Int(x, y));
				if (curTile != null)
				{
					TileData data = default;
					curTile.GetTileData(new Vector3Int(x, y), tilemap, ref data);

					CustomTileData customData = new CustomTileData(x, y, data, curTile);
					yield return customData;
				}
			}
		}
	}
}
