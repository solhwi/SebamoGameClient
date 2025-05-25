using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
	Isometric = 0,
	Dimetric,
}

public class TileDataSetter : MonoBehaviour
{
	[SerializeField] private TileType tileType = TileType.Dimetric;

	[SerializeField] private Grid grid = null;
	[SerializeField] private Tilemap tileMap = null;
	[SerializeField] private Tilemap[] subTileMaps = null;

	[SerializeField] private TilemapRenderer tileMapRenderer = null;
	[SerializeField] private TilemapRenderer[] subTileMapRenderers = null;


	private void Awake()
	{
		SetData();
	}

	[ContextMenu("타일맵 데이터 세팅")]
	private void SetData()
	{
		SetGridData();
		SetTileData();
		SetTileMapRendererData();
		SetCameraData();
		SetZPos();
	}

	private void SetZPos()
	{
		transform.position = new Vector3(0, 0, 0);
	}

	private void SetCameraData()
	{
		Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
		Camera.main.transparencySortAxis = TileDataContainer.Instance.transparencySortAxis;
	}

	private void SetGridData()
	{
		grid.cellGap = TileDataContainer.Instance.gridCellGap;
		grid.cellSize = TileDataContainer.Instance.GetGridCellSize(tileType);
		grid.cellLayout = TileDataContainer.Instance.layOutType;
		grid.cellSwizzle = TileDataContainer.Instance.SwizzleType;
	}

	private void SetTileData()
	{
		tileMap.tileAnchor = TileDataContainer.Instance.GetTileAnchor(tileType);

		foreach (var sub in subTileMaps)
		{
			sub.tileAnchor = TileDataContainer.Instance.GetTileAnchor(tileType);
		}

		tileMap.orientation = TileDataContainer.Instance.OrientationType;
		tileMap.animationFrameRate = TileDataContainer.Instance.tileAnimationFrameRate;
	}

	private void SetTileMapRendererData()
	{
		tileMapRenderer.sortOrder = TileDataContainer.Instance.sortingOrderType;
		tileMapRenderer.mode = TileDataContainer.Instance.tileRenderMode;
		tileMapRenderer.sortingOrder = (int)LayerConfig.Tile;

		foreach (var sub in subTileMapRenderers)
		{
			sub.sortOrder = TileDataContainer.Instance.sortingOrderType;
			sub.mode = TileDataContainer.Instance.tileRenderMode;
			sub.sortingOrder = (int)LayerConfig.Tile;
		}
	}
}
