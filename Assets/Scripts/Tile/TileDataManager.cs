using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		float tileCenterXPos = tileWorldPosition.x - cellSize.x / 2;

		// 왼쪽
		if (tileCenterXPos - cellSize.x <= pos.x && pos.x <= tileCenterXPos)
		{
			// 아래
			if (tileCenterYPos - cellSize.y <= pos.y && pos.y <= tileCenterYPos)
			{
				return -cellSize.y * pos.x + tileCenterYPos - cellSize.y <= pos.y;
			}
			// 위쪽
			else if (tileCenterYPos + cellSize.y >= pos.y && pos.y >= tileCenterYPos)
			{
				return cellSize.y * pos.x + tileCenterYPos + cellSize.y >= pos.y;
			}
		}
		// 오른쪽
		else if (tileCenterXPos + cellSize.x >= pos.x && tileCenterXPos <= pos.x)
		{
			// 아래
			if (tileCenterYPos - cellSize.y <= pos.y && tileCenterYPos >= pos.y)
			{
				return cellSize.y * pos.x + tileCenterYPos - cellSize.y <= pos.y;
			}
			// 위쪽
			else if (tileCenterYPos + cellSize.y >= pos.y && pos.y >= tileCenterYPos)
			{
				return -cellSize.y * pos.x + tileCenterYPos + cellSize.y >= pos.y;
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
}

public class TileDataManager : MonoBehaviour
{
	[SerializeField] private Tilemap floorTileMap;
	[SerializeField] private Tilemap selectTileMap;

	[SerializeField] private Grid tileGrid;
	[SerializeField] private TileDataContainer dataContainer;
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private FieldItemFactory fieldItemFactory;

	[SerializeField] private TileBase selectTile;
	[SerializeField] private TileBase unSelectTile;

	public WorldTileData[] tileBoardDatas = null;

	private Dictionary<int, FieldItem> fieldItemDictionary = new Dictionary<int, FieldItem>();

	private void Awake()
	{
		tileBoardDatas = MakeBoardData().ToArray();
	}

	public void SetSelectTiles(int min, int max)
	{
		min = Mathf.Max(0, min);
		max = Mathf.Min(max, tileBoardDatas.Length - 1);

		int[] tileIndexes = CommonFunc.ToRange(min, max).Select(GetTileIndexByOrder).ToArray();

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
		if (index < 0 || dataContainer.tileItems.Length <= index)
			return true;

		return dataContainer.tileItems[index] != null && dataContainer.tileItems[index] != string.Empty;
	}

	public async Task<bool> TrySetTileItem(int tileOrder, FieldItem fieldItem)
	{
		if (tileOrder < 0 || fieldItem == null)
			return false;

		int index = GetTileIndexByOrder(tileOrder);
		bool isSuccess = await dataContainer.TrySetTileItem(index, fieldItem.fieldItemCode);
		if (isSuccess == false)
			return false;

		fieldItemDictionary[index] = fieldItem;
		fieldItem.Create(tileBoardDatas[index]);

		return true;
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
		if (dataContainer.tileOrders.Length <= tileIndex)
			return -1;

		return dataContainer.tileOrders[tileIndex];
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

		var fieldItem = fieldItemFactory.Make(itemCode);
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

	public SpecialTileBase GetCurrentSpecialTile(int currentOrder)
	{
		return GetCurrentTile(currentOrder) as SpecialTileBase;
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
