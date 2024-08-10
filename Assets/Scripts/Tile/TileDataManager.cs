using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct WorldTileData
{
	public int index;
	public Vector2 tileWorldPosition;
	public Vector2 tilePlaneWorldPosition;
	public Vector3Int tilePosition;
	public TileData tileData;
	public TileBase tileBase;

	public WorldTileData(int index, Vector2 cellSize, Vector3Int tilePos, TileData tileData, TileBase tileBase)
	{
		this.index = index;
		this.tileWorldPosition = ConvertWorldPos(tilePos, cellSize);
		this.tilePlaneWorldPosition = ConvertPlaneWorldPos(tilePos);
		this.tilePosition = tilePos;
		this.tileData = tileData;
		this.tileBase = tileBase;
	}

	private static Vector2 ConvertWorldPos(Vector3Int tilePos, Vector2 cellSize)
	{
		float x = tilePos.y * -1 * cellSize.x + tilePos.x * cellSize.x; // y에 1을 더하면 -1, x에 1을 더하면 1
		float y = tilePos.y * cellSize.y + tilePos.x * cellSize.y; // y에 1을 더하면 0.5, x에 1을 더하면 0.5 

		return new Vector2(x, y);
	}

	private static Vector3 ConvertPlaneWorldPos(Vector3Int tilePos)
	{
		float x = tilePos.y * -1; // x에 1 더하면 위로 한 칸
		float y = tilePos.x; // y에 1을 더하면 왼 쪽으로 한 칸

		return new Vector2(x, y);
	}

	public TTile GetTile<TTile>() where TTile : TileBase
	{
		if(tileBase is TTile t)
		{
			return t;
		}

		return null;
	}
}

public class TileDataManager : MonoBehaviour
{
	[SerializeField] private Tilemap tilemap;
	[SerializeField] private Grid tileGrid;
	[SerializeField] private TileDataContainer dataContainer;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private FieldItemFactory fieldItemFactory;

	public WorldTileData[] tileBoardDatas = null;

	private Dictionary<int, FieldItem> fieldfieldItemDictionary = new Dictionary<int, FieldItem>();

	private void Awake()
	{
		tileBoardDatas = MakeBoardData().ToArray();
	}

	public IEnumerator PrepareTile()
	{
		for (int i = 0; i < dataContainer.tileItems.Length; i++)
		{
			var itemCode = dataContainer.tileItems[i];
			if (itemCode == string.Empty)
				continue;

			var fieldItem = fieldItemFactory.Make(itemCode);
			if (fieldItem == null)
				continue;

			fieldfieldItemDictionary[i] = fieldItem;

			var boardData = tileBoardDatas[i];
			fieldItem.Create(boardData);

			yield return null;
		}
	}

	public IEnumerable<WorldTileData> MakeBoardData()
	{
		int i = 0;

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

					yield return new WorldTileData(i++, tileGrid.cellSize, tilePos, data, curTile);
				}
			}
		}
	}

	public FieldItem GetCurrentTileItem(int currentOrder)
	{
		string itemCode = GetCurrentTileItemCode(currentOrder);
		if (itemCode == string.Empty)
			return null;

		int tileIndex = GetTileIndexByOrder(currentOrder);
		if (tileIndex == -1)
			return null;

		if (fieldfieldItemDictionary.TryGetValue(tileIndex, out var data))
			return data;

		var fieldItem = fieldItemFactory.Make(itemCode);
		fieldfieldItemDictionary[tileIndex] = fieldItem;

		return fieldItem;
	}

	public SpecialTileBase GetCurrentSpecialTile(int currentOrder)
	{
		int tileIndex = GetTileIndexByOrder(currentOrder);
		if (tileIndex == -1 || tileBoardDatas.Length <= tileIndex)
			return null;

		return tileBoardDatas[tileIndex].tileBase as SpecialTileBase;
	}

	public string GetCurrentTileItemCode(int currentOrder)
	{
		int tileIndex = GetTileIndexByOrder(currentOrder);
		if (tileIndex < 0 || tileIndex >= dataContainer.tileItems.Length)
			return string.Empty;

		return dataContainer.tileItems[tileIndex];
	}

	/// <summary>
	/// 현재 위치부터, 목표 위치까지 경로 반환
	/// </summary>
	/// <param name="currentIndex"></param>
	/// <param name="progressCount"></param>
	/// <returns></returns>
	public IEnumerable<WorldTileData> GetTilePath(int currentOrder, int progressCount)
	{
		for(int i = currentOrder + 1; i <= currentOrder + progressCount; i++)
		{
			int tileIndex = GetTileIndexByOrder(i);
			if (tileIndex < 0 || tileIndex >= tileBoardDatas.Length)
				yield break;

			yield return tileBoardDatas[tileIndex];
		}
	}

	/// <summary>
	/// 현재 타일 순서에 맞는 타일 인덱스 가져오기
	/// </summary>
	/// <param name="currentOrder"></param>
	/// <returns></returns>
	public int GetTileIndexByOrder(int currentOrder)
	{
		if (dataContainer == null)
			return -1;

		return dataContainer.GetTileIndexByOrder(currentOrder);
	}

	/// <summary>
	/// 현재 타일 순서에 맞는 타일 데이터 가져오기
	/// </summary>
	/// <param name="currentOrder"></param>
	/// <returns></returns>
	public WorldTileData GetTileDataByOrder(int currentOrder)
	{
		int tileIndex = GetTileIndexByOrder(currentOrder);

		if (tileBoardDatas.Length <= tileIndex || tileIndex < 0)
		{
			Debug.LogError($"{currentOrder}가 타일 데이터 인덱스를 넘어감");
			return default;
		}	

		return tileBoardDatas[tileIndex];
	}
}
