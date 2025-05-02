using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public class TileIndexingWindow : TileEditorWindow
{
	private static int[] tileOrders = null;

	[MenuItem("Tools/타일 인덱싱 도우미 %I")]
	public static void OpenWindow()
	{
		EditorSceneManager.OpenScene($"Assets/Scenes/{SceneType.Game}.unity");
		
		InitializeTileData();
		InitializeTileOrder();

		EditorName = "타일 인덱싱 도우미";
		OnOpenWindow<TileIndexingWindow>();
	}

	private static void InitializeTileOrder()
	{
		if (tileDataContainer.tileOrders == null || tileDataContainer.tileOrders.Length != boardTileDatas.Count)
		{
			tileOrders = new int[boardTileDatas.Count];
			ClearTileOrderData();
		}
		else
		{
			tileOrders = tileDataContainer.tileOrders.ToArray();
		}
	}
	
	private static void ClearTileOrderData()
	{
		for (int i = 0; i < tileOrders.Length; i++)
		{
			tileOrders[i] = -1;
		}
	}

	protected override void SaveData()
	{
		tileDataContainer.SetTileOrder(tileOrders);

		base.SaveData();
	}

	protected override void OnClickClear()
	{
		base.OnClickClear();

		ClearTileOrderData();
	}


	protected override void DrawBoard()
	{
		DrawAxis(Axis.Vertical, () =>
		{
			var tileDataList = boardTileDatas.ToList();

			for (int i = 0; i < tileDataList.Count; i++)
			{
				var data = tileDataList[i];

				float xPos = 0.0f;
				float yPos = 0.0f;

				if (isIsometric)
				{
					xPos = data.tileWorldPosition.x;
					yPos = data.tileWorldPosition.y;
				}
				else
				{
					xPos = data.tilePlaneWorldPosition.x;
					yPos = data.tilePlaneWorldPosition.y;
				}

				int currentIndex = tileOrders[i];
				string buttonName = currentIndex >= 0 ? $"{currentIndex}" : string.Empty;

				DrawTileButton(xPos, yPos, tileButtonSize, tileButtonSize, windowCenterPos, () =>
				{
					OnClickIndex(i);
				}, buttonName);
			}
		});
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
