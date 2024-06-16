using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TileIndexingWindow : CustomEditorWindow
{
	private static List<WorldTileData> boardTileDatas = new List<WorldTileData>();

	private static TileDataContainer tileDataContainer;
	private const string TileDataContainerPath = "Assets/Resources/Datas/TileDataContainer.asset";

	private static int[] tileOrders = null;

	private static Vector2 windowCenterPos = new Vector2(300, 150);
	private static float tileButtonSize = 30.0f;

	[MenuItem("Tools/TileIndexingWindow %#I")]
	public static void OpenWindow()
	{
		tileDataContainer = AssetDatabase.LoadAssetAtPath<TileDataContainer>(TileDataContainerPath);
		if (tileDataContainer == null)
		{
			Debug.LogError($"{TileDataContainerPath}에 타일 컨테이너 데이터 없음");
			return;
		}

		var tileDataManager = FindAnyObjectByType<TileDataManager>();
		if (tileDataManager == null)
		{
			Debug.LogError($"타일 매니저가 현재 씬에 없음");
			return;
		}

		boardTileDatas = tileDataManager.MakeBoardData().ToList();
		if (boardTileDatas == null || boardTileDatas.Count == 0)
		{
			Debug.LogError($"타일맵 내 그려진 타일이 없음");
			return;
		}

		if (tileDataContainer.tileOrders == null)
		{
			tileOrders = new int[boardTileDatas.Count];
			ClearTileOrderData();
		}
		else
		{
			tileOrders = tileDataContainer.tileOrders.ToArray();
		}

		var window = GetWindow<TileIndexingWindow>();
		if (window != null)
		{
			window.titleContent = new GUIContent("타일 인덱싱 도우미");

			var size = GetBoardSize(boardTileDatas);
			var safeSize = size * 1.5f;

			window.position = new Rect(100, 100, safeSize.x, safeSize.y);

			window.Show();
		}
	}

	private static void ClearTileOrderData()
	{
		for (int i = 0; i < tileOrders.Length; i++)
		{
			tileOrders[i] = -1;
		}
	}

	private static Vector2 GetBoardSize(List<WorldTileData> tileDatas)
	{
		float maxX = tileDatas.Max(d => d.tileWorldPosition.x);
		float maxY = tileDatas.Max(d => d.tileWorldPosition.y);

		float minX = tileDatas.Min(d => d.tileWorldPosition.x);
		float minY = tileDatas.Min(d => d.tileWorldPosition.y);

		return new Vector2(tileButtonSize * (maxX - minX) + windowCenterPos.x, tileButtonSize * (maxY - minY) + windowCenterPos.y);
	}

	protected override void SaveData()
	{
		tileDataContainer.SetTileOrder(tileOrders);

		AssetDatabase.SaveAssetIfDirty(tileDataContainer);
		// AssetDatabase.Refresh();
	}

	protected override void DrawAll()
	{
		DrawSpace(10);

		DrawAxis(Axis.Horizontal, () =>
		{
			DrawLabel("center x:", 50, 20);
			windowCenterPos.x = (float)DrawField(FieldType.Float, windowCenterPos.x, 30, 20);

			DrawSpace(5);

			DrawLabel("center y:", 50, 20);
			windowCenterPos.y = (float)DrawField(FieldType.Float, windowCenterPos.y, 30, 20);
		});

		DrawSpace(15);

		DrawAxis(Axis.Horizontal, () =>
		{
			DrawButton(50, 30, OnClickSave, "저장", null, null);

			DrawSpace(5);

			DrawButton(50, 30, OnClickClear, "초기화", null, null);
		});

		DrawBoardIndexingHelper();
	}

	private void DrawBoardIndexingHelper()
	{
		DrawAxis(Axis.Vertical, () =>
		{
			var tileDataList = boardTileDatas.ToList();

			for (int i = 0; i < tileDataList.Count; i++)
			{
				var data = tileDataList[i];

				float xPos = data.tileWorldPosition.x;
				float yPos = data.tileWorldPosition.y;

				int currentIndex = tileOrders[i];
				string buttonName = currentIndex >= 0 ? $"{currentIndex}" : string.Empty;

				DrawTileButton(xPos, yPos, tileButtonSize, tileButtonSize, windowCenterPos, () =>
				{
					OnClickIndex(i);
				}, buttonName, null, null);
			}
		});
	}

	private void OnClickSave()
	{
		SaveData();
	}

	private void OnClickClear()
	{
		ClearTileOrderData();
	}

	private void OnClickIndex(int index)
	{
		int currentOrder = tileOrders[index];

		// 기존에 순서가 있었다면
		if (currentOrder >= 0)
		{
			// 나보다 큰 수의 인덱스를 하나씩 줄임
			for (int i = 0; i < tileOrders.Length; i++)
			{
				if (tileOrders[i] > currentOrder)
				{
					tileOrders[i] = tileOrders[i] - 1;
				}	
			}

			// 초기화
			tileOrders[index] = -1;
		}
		else
		{
			tileOrders[index] = tileOrders.Max() + 1;
		}
	}

	
}
