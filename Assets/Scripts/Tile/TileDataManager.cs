using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;

public struct WorldTileData
{
	public int index;
	public Vector2 tileWorldPosition;
	public Vector2 tilePlayerPosition;
	public Vector2 tilePlaneWorldPosition;
	public Vector3Int tilePosition;
	public TileData tileData;
	public TileBase tileBase;
	public Vector2 cellSize;

	public WorldTileData(int index, Vector2 cellSize, Vector3Int tilePos, TileData tileData, TileBase tileBase)
	{
		this.index = index;
		this.cellSize = cellSize;
		this.tileWorldPosition = ConvertWorldPos(tilePos, cellSize);
		this.tilePlayerPosition = new Vector2(tileWorldPosition.x, tileWorldPosition.y + cellSize.y / 2);
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

	public bool IsCollision(Vector2 pos)
	{
		// 계산 오차로 인한 보정값
		float tileCenterYPos = tileWorldPosition.y - cellSize.y;
		float tileCenterXPos = tileWorldPosition.x;

		float x = pos.x - tileCenterXPos;
		float y = pos.y - tileCenterYPos;

		// 왼쪽
		if (-cellSize.x <= x && x <= 0)
		{
			// 아래
			if (-cellSize.y <= y && y <= 0)
			{
				return -cellSize.y * x - cellSize.y <= y;
			}
			// 위쪽
			else if (cellSize.y >= y && y >= 0)
			{
				return cellSize.y * x + cellSize.y >= y;
			}
		}
		// 오른쪽
		else if (cellSize.x >= x && 0 <= x)
		{
			// 아래
			if (-cellSize.y <= y && y >= 0)
			{
				return cellSize.y * x - cellSize.y <= y;
			}
			// 위쪽
			else if (cellSize.y >= y && y >= 0)
			{
				return -cellSize.y * x + cellSize.y >= y;
			}
		}

		return false;
	}

	public TTile GetTile<TTile>() where TTile : TileBase
	{
		if(tileBase is TTile t)
		{
			return t;
		}

		return null;
	}

	public bool IsSpecialTile()
	{
		return tileBase is SpecialTileBase;
	}
}

public class TileDataManager : Singleton<TileDataManager>
{
	[SerializeField] private Tilemap floorTileMap;
	[SerializeField] private Tilemap selectTileMap;

	[SerializeField] private Grid tileGrid;	

	[SerializeField] private TileBase selectTile;
	[SerializeField] private TileBase unSelectTile;

	public WorldTileData[] tileBoardDatas = null;

	private Dictionary<int, FieldItem> fieldItemDictionary = new Dictionary<int, FieldItem>();

	public IEnumerator PrepareBoardData()
	{
		List<WorldTileData> tileBoardDataList = new List<WorldTileData>();

		foreach (var d in MakeBoardData())
		{
			yield return null;

			tileBoardDataList.Add(d);
		}

		tileBoardDatas = tileBoardDataList.ToArray();
	}

	public void SetSelectTiles(IEnumerable<int> tileOrders)
	{
		int[] tileIndexes = tileOrders.Select(GetTileIndexByOrder).ToArray();

		for(int i = 0; i < tileBoardDatas.Length; i++)
		{
			var tilePos = tileBoardDatas[i].tilePosition;

			if (tileIndexes.Contains(i))
			{
				selectTileMap.SetTile(tilePos, selectTile);
			}
			else
			{
				selectTileMap.SetTile(tilePos, unSelectTile);
			}
		}
	}

	public bool IsAlreadyReplaced(int tileOrder)
	{
		int index = GetTileIndexByOrder(tileOrder);
		if (index < 0 || TileDataContainer.Instance.tileItems.Length <= index)
			return true;

		return TileDataContainer.Instance.tileItems[index] != null && TileDataContainer.Instance.tileItems[index] != string.Empty;
	}

	public bool IsSpecialTile(int tileOrder)
	{
		return GetCurrentSpecialTile(tileOrder) != null;
	}

	public bool TrySetTileItem(int tileOrder, FieldItem fieldItem, bool isImmediately = false)
	{
		if (tileOrder < 0)
			return false;

		int index = GetTileIndexByOrder(tileOrder);
		bool isSuccess = TileDataContainer.Instance.TrySetTileItem(index, fieldItem?.fieldItemCode ?? string.Empty);
		if (isSuccess == false)
			return false;

		if (fieldItem != null)
		{
			fieldItemDictionary[index] = fieldItem;
			fieldItem.Create(tileBoardDatas[index]);
		}
		else if (isImmediately)
		{
			RemoveFieldItem(tileOrder);
		}
		
		return true;
	}

	public void RemoveFieldItem(int tileOrder)
	{
		if (tileOrder < 0)
			return;

		int tileIndex = GetTileIndexByOrder(tileOrder);

		if (fieldItemDictionary.TryGetValue(tileIndex, out var fieldItem))
		{
			fieldItem.Destroy();
		}

		fieldItemDictionary.Remove(tileIndex);
	}

	public int GetTileIndexFromPos(Vector2 tilePos)
	{
		for (int i = 0; i < tileBoardDatas.Length; i++)
		{
			var tile = tileBoardDatas[i];
			if (tile.IsCollision(tilePos))
			{
				return i;
			}
		}

		return -1;
	}

	public void ClearSelectTile()
	{
		selectTileMap.ClearAllTiles();
	}

	public WorldTileData GetTileData(int tileOrder)
	{
		int tileIndex = GetTileIndexByOrder(tileOrder);
		if (tileIndex < 0)
			return default;

		return tileBoardDatas[tileIndex];
	}

	public int GetTileOrder(int tileIndex)
	{
		if (TileDataContainer.Instance.tileOrders.Length <= tileIndex)
			return -1;

		return TileDataContainer.Instance.tileOrders[tileIndex];
	}

	public IEnumerator PrepareTile()
	{
		for (int i = 0; i < TileDataContainer.Instance.tileItems.Length; i++)
		{
			var itemCode = TileDataContainer.Instance.tileItems[i];
			if (itemCode == string.Empty)
				continue;

			var fieldItem = FieldItemFactory.Instance.Make(itemCode);
			if (fieldItem == null)
				continue;

			fieldItemDictionary[i] = fieldItem;
			fieldItem.Create(tileBoardDatas[i]);

			yield return null;
		}
	}

	public IEnumerable<WorldTileData> MakeBoardData()
	{
		int i = 0;

		for (int x = floorTileMap.cellBounds.xMax; x >= floorTileMap.cellBounds.xMin; x--)
		{
			for (int y = floorTileMap.cellBounds.yMax; y >= floorTileMap.cellBounds.yMin; y--)
			{
				var tilePos = new Vector3Int(x, y);
				TileBase curTile = floorTileMap.GetTile(tilePos);
				if (curTile != null)
				{
					TileData data = default;
					curTile.GetTileData(tilePos, floorTileMap, ref data);

					yield return new WorldTileData(i++, tileGrid.cellSize, tilePos, data, curTile);
				}
			}
		}
	}

	public int GetNextOrder(int currentOrder, int count, out BarricadeItem barricadeItem)
	{
		barricadeItem = null;

		int nextOrder = currentOrder;
		if (count > 0)
		{
			for (; nextOrder < currentOrder + count; nextOrder++)
			{
				var fieldItem = GetCurrentTileItem(nextOrder);
				if (fieldItem is BarricadeItem b)
				{
					barricadeItem = b;
					break;
				}
			}
		}
		else
		{
			for (; nextOrder > currentOrder + count; nextOrder--)
			{
				var fieldItem = GetCurrentTileItem(nextOrder);
				if (fieldItem is BarricadeItem b)
				{
					barricadeItem = b;
					break;
				}
			}
		}		

		return nextOrder;
	}

	public FieldItem GetCurrentTileItem(int currentOrder)
	{
		string itemCode = GetCurrentTileItemCode(currentOrder);
		if (itemCode == string.Empty)
			return null;

		int tileIndex = GetTileIndexByOrder(currentOrder);
		if (tileIndex == -1)
			return null;

		if (fieldItemDictionary.TryGetValue(tileIndex, out var data))
			return data;

		var fieldItem = FieldItemFactory.Instance.Make(itemCode);
		fieldItemDictionary[tileIndex] = fieldItem;

		return fieldItem;
	}

	public Tile GetCurrentTile(int currentOrder)
	{
		int tileIndex = GetTileIndexByOrder(currentOrder);
		if (tileIndex == -1 || tileBoardDatas.Length <= tileIndex)
			return null;

		return tileBoardDatas[tileIndex].tileBase as Tile;
	}

	public FieldItem GetFieldItem(int currentOrder)
	{
		int tileIndex = GetTileIndexByOrder(currentOrder);
		if (tileIndex == -1)
			return null;

		if (fieldItemDictionary.TryGetValue(tileIndex, out var data))
			return data;

		return null;
	}

	public SpecialTileBase GetCurrentSpecialTile(int currentOrder)
	{
		return GetCurrentTile(currentOrder) as SpecialTileBase;
	}

	public string GetCurrentTileItemCode(int currentOrder)
	{
		int tileIndex = GetTileIndexByOrder(currentOrder);
		if (tileIndex < 0 || tileIndex >= TileDataContainer.Instance.tileItems.Length)
			return string.Empty;

		return TileDataContainer.Instance.tileItems[tileIndex];
	}

	/// <summary>
	/// 현재 위치부터, 목표 위치까지 경로 반환
	/// </summary>
	/// <param name="currentIndex"></param>
	/// <param name="progressCount"></param>
	/// <returns></returns>
	public IEnumerable<WorldTileData> GetTilePath(int currentOrder, int progressCount)
	{
		if (progressCount > 0)
		{
			for (int i = currentOrder + 1; i <= currentOrder + progressCount; i++)
			{
				int tileIndex = GetTileIndexByOrder(i);
				if (tileIndex < 0 || tileIndex >= tileBoardDatas.Length)
					yield break;

				yield return tileBoardDatas[tileIndex];
			}
		}
		else
		{
			for (int i = currentOrder - 1; i >= currentOrder + progressCount; i--)
			{
				int tileIndex = GetTileIndexByOrder(i);
				if (tileIndex < 0 || tileIndex >= tileBoardDatas.Length)
					yield break;

				yield return tileBoardDatas[tileIndex];
			}
		}
		
	}

	/// <summary>
	/// 현재 타일 순서에 맞는 타일 인덱스 가져오기
	/// </summary>
	/// <param name="currentOrder"></param>
	/// <returns></returns>
	public int GetTileIndexByOrder(int currentOrder)
	{
		if (TileDataContainer.Instance == null)
			return -1;

		return TileDataContainer.Instance.GetTileIndexByOrder(currentOrder);
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

	public Vector3 GetPlayerPosByOrder(int currentOrder)
	{
		var currentPlayerTileData = GetTileDataByOrder(currentOrder);
		return currentPlayerTileData.tilePlayerPosition;
	}
}
