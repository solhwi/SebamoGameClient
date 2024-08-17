using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
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

	public Vector3 isometricTileAnchor = new Vector3(0.1f, 0.1f, 0);
	public Vector3 dimetricTileAnchor = new Vector3(0.15f, 0.15f, 0);

	public Vector3 gridCellGap = new Vector3(0, 0, 0);

    public Grid.CellLayout layOutType = GridLayout.CellLayout.IsometricZAsY;
    public Grid.CellSwizzle SwizzleType = GridLayout.CellSwizzle.XYZ;
    public Tilemap.Orientation OrientationType = Tilemap.Orientation.XY;
    public TilemapRenderer.Mode tileRenderMode = TilemapRenderer.Mode.Chunk;
    public TilemapRenderer.SortOrder sortingOrderType = TilemapRenderer.SortOrder.TopRight;
    public float tileAnimationFrameRate = 1.0f;

	public int[] tileOrders = null;
	public string[] tileItems = null;

	private Dictionary<int, int> orderToTileIndexMap = new Dictionary<int, int>();

	public void SetTileOrder(IEnumerable<int> orders)
	{
		tileOrders = orders.ToArray();
		EditorUtility.SetDirty(this);
	}

	public void SetTileItems(IEnumerable<string> items)
	{
		tileItems = items.ToArray();
		EditorUtility.SetDirty(this);
	}

	public async Task<bool> TrySetTileItem(int index, string itemCode)
	{
		if (tileItems.Length <= index)
			return false;

		bool isSuccess = await HttpNetworkManager.Instance.TryPostTileData();
		if (isSuccess == false)
			return false;

		tileItems[index] = itemCode;
		return true;
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

	public int GetTileIndexByOrder(int currentOrder)
	{
		if (currentOrder < 0)
			return -1;

		if (currentOrder >= tileOrders.Length)
			return -1;

		if (orderToTileIndexMap.ContainsKey(currentOrder))
			return orderToTileIndexMap[currentOrder];

		for (int i = 0; i < tileOrders.Length; i++)
		{
			if (tileOrders[i] == currentOrder)
			{
				orderToTileIndexMap[currentOrder] = i;
				return i;
			}
		}

		return -1;
	}
}
