using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileDataContainer")]
public class TileDataContainer : ScriptableObject
{
    // 고정값
	// https://docs.unity3d.com/kr/2020.3/Manual/Tilemap-Isometric-CreateIso.html
	public Vector3 transparencySortAxis = new Vector3(0, 0.1f, -0.26f);
	public Vector3 isometricGridCellSize = new Vector3(1, 0.57735f, 1);

	public Vector3 dimetricGridCellSize = new Vector3(1, 0.51f, 1);

	public Vector3 isometricTileAnchor = new Vector3(0, 0, 0);
	public Vector3 dimetricTileAnchor = new Vector3(0.15f, 0.15f, 0);

	public Vector3 gridCellGap = new Vector3(0, 0, 0);

    public Grid.CellLayout layOutType = GridLayout.CellLayout.IsometricZAsY;
    public Grid.CellSwizzle SwizzleType = GridLayout.CellSwizzle.XYZ;
    public Tilemap.Orientation OrientationType = Tilemap.Orientation.XY;
    public TilemapRenderer.Mode tileRenderMode = TilemapRenderer.Mode.Chunk;
    public TilemapRenderer.SortOrder sortingOrderType = TilemapRenderer.SortOrder.TopRight;
    public float tileAnimationFrameRate = 1.0f;

	/// <summary>
	/// 타일의 진행 순서 리스트
	/// </summary>
	public List<int> tileOrderList = new List<int>();

	public void SetTileIndices(IEnumerable<int> orders)
	{
		tileOrderList = orders.ToList();
	}

    public Vector3 GetGridCellSize(TileType type)
    {
        switch(type)
        {
            case TileType.Dimetric:
                return dimetricGridCellSize;

            case TileType.Isometric:
                return isometricGridCellSize;
        }

        return Vector3.zero;
    }

    public Vector3 GetTileAnchor(TileType type)
    {
		switch (type)
		{
			case TileType.Dimetric:
				return dimetricTileAnchor;

			case TileType.Isometric:
				return isometricTileAnchor;
		}

		return Vector3.zero;
	}
}
