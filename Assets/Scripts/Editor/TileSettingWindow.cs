using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TileSettingWindow : TileEditorWindow
{
	private static string[] tileKinds = null;

	private static string selectedTileCode = string.Empty;
	private static int selectedToggleIndex = 0;

	protected static TileTable tileTable;
	protected const string TileTablePath = "Assets/Resources/Datas/Parser/TileTable.asset";


	[MenuItem("Tools/타일 종류 배치 도우미 %#T")]
	public static void OpenWindow()
	{
		InitializeTileData();
		InitializeTileItems();

		tileTable = AssetDatabase.LoadAssetAtPath<TileTable>(TileTablePath);
		if (tileTable == null)
		{
			Debug.LogError($"{TileTablePath}에 타일 테이블 없음");
			return;
		}

		EditorName = "타일 종류 배치 도우미";

		windowCenterPos = new Vector2(450, 200);
		tileButtonSize = 50.0f;

		OnOpenWindow<TileSettingWindow>();
	}

	private static void InitializeTileItems()
	{
		if (tileDataContainer.tileKinds == null || tileDataContainer.tileKinds.Any() == false)
		{
			tileKinds = new string[boardTileDatas.Count];
			ClearTileItemData();
		}
		else
		{
			tileKinds = tileDataContainer.tileKinds.ToArray();
		}
	}

	private static void ClearTileItemData()
	{
		for (int i = 0; i < tileKinds.Length; i++)
		{
			tileKinds[i] = string.Empty;
		}
	}

	protected override void SaveData()
	{
		tileDataContainer.SetTileKinds(tileKinds);

		base.SaveData();
	}

	protected override void OnClickClear()
	{
		base.OnClickClear();

		ClearTileItemData();
	}

	protected override void DrawPalette()
	{
		base.DrawPalette();

		DrawLabel("[팔레트 목록]", 100, 30);

		DrawAxis(Axis.Vertical, () =>
		{
			for (int i = 0; i < tileTable.tileDataList.Count; i++)
			{
				var tileData = tileTable.tileDataList[i];
				bool isToggleOn = DrawToggle(80, 30, selectedToggleIndex == i, tileData.key);
				if (isToggleOn)
				{
					selectedTileCode = tileData.key;
					selectedToggleIndex = i;
				}
			}
		});
	}

	protected override void DrawBoard()
	{
		var _pressOnTexture = isOnTextureMode ? pressOnTexture : null;
		var _pressOffTexture = isOnTextureMode ? pressOffTexture : null;
		var _focusOnTexture = isOnTextureMode ? hoverTexture : null;

		DrawAxis(Axis.Vertical, () =>
		{
			var tileDataList = boardTileDatas.ToList();

			for (int i = 0; i < tileDataList.Count; i++)
			{
				var data = tileDataList[i];

				float xPos = data.tileWorldPosition.x;
				float yPos = data.tileWorldPosition.y;

				string currentItemCode = tileKinds[i];

				DrawTileButton(xPos, yPos, tileButtonSize, tileButtonSize, windowCenterPos, () =>
				{
					OnClickIndex(i);
				}, tileKinds[i]);
			}
		});
	}

	private void OnClickIndex(int index)
	{
		string currentItemCode = tileKinds[index];

		// 기존에 순서가 있었다면
		if (currentItemCode != string.Empty)
		{
			tileKinds[index] = string.Empty;
		}
		else
		{
			tileKinds[index] = selectedTileCode;
		}
	}
}
