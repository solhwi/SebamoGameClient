using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileBackGroundData
{
	public int tileOrder;
	public AssetReferenceTexture2D backGroundResource;
}

[CreateAssetMenu(fileName = "TileDataContainer")]
public class TileDataContainer : DataContainer<TileDataContainer>
{
	public List<TileBackGroundData> backGroundDataList = new List<TileBackGroundData>();

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

	public IEnumerator PreLoad()
	{
		foreach (var data in backGroundDataList)
		{
			yield return ResourceManager.Instance.LoadAsync<Texture2D>(data.backGroundResource);
		}
	}

	public void SetTileOrder(IEnumerable<int> orders)
	{
		tileOrders = orders.ToArray();

#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}

	public void SetTileItems(IEnumerable<string> items)
	{
		tileItems = items.ToArray();

#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}

	public void SetTileItemPacketData(TilePacketData data)
	{
		tileItems = new string[tileOrders.Length];

		if (data != null)
		{
			for (int i = 0; i < data.tileItemIndexes.Length; i++)
			{
				int index = data.tileItemIndexes[i];
				tileItems[index] = data.tileItemCodes[i];
			}
		}
	}

	public bool TrySetTileItem(int index, string itemCode)
	{
		if (tileItems.Length <= index)
			return false;

		tileItems[index] = itemCode;
		return true;
	}

	public Texture2D GetBackGroundResource(int currentOrder)
	{
		int index = GetBackGroundIndex(currentOrder);

		var backGroundData = backGroundDataList[index];
		if (backGroundData == null)
			return null;

		return ResourceManager.Instance.Load<Texture2D>(backGroundData.backGroundResource);
	}

	private int GetBackGroundIndex(int currentOrder)
	{
		int index = 0;

		for (int i = 0; i < backGroundDataList.Count; i++)
		{
			var backGroundData = backGroundDataList[i];
			if (backGroundData.tileOrder <= currentOrder)
			{
				index = i;
			}
		}

		return index;
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
