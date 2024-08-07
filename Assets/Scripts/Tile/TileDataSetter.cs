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
    [SerializeField] private TileDataContainer tileDataContainer = null;
	[SerializeField] private TileType tileType = TileType.Dimetric;

	[SerializeField] private Grid grid = null;
	[SerializeField] private Tilemap tileMap = null;
	[SerializeField] private TilemapRenderer tileMapRenderer = null;

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
		Camera.main.transparencySortAxis = tileDataContainer.transparencySortAxis;
	}

	private void SetGridData()
	{
		grid.cellGap = tileDataContainer.gridCellGap;
		grid.cellSize = tileDataContainer.GetGridCellSize(tileType);
		grid.cellLayout = tileDataContainer.layOutType;
		grid.cellSwizzle = tileDataContainer.SwizzleType;
	}

	private void SetTileData()
	{
		tileMap.tileAnchor = tileDataContainer.GetTileAnchor(tileType);
		tileMap.orientation = tileDataContainer.OrientationType;
		tileMap.animationFrameRate = tileDataContainer.tileAnimationFrameRate;
	}

	private void SetTileMapRendererData()
	{
		tileMapRenderer.sortOrder = tileDataContainer.sortingOrderType;
		tileMapRenderer.mode = tileDataContainer.tileRenderMode;
		tileMapRenderer.sortingOrder = (int)LayerConfig.Tile;
	}
}
