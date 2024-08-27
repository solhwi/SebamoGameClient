using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class TileEditorWindow : CustomEditorWindow
{
	protected static List<WorldTileData> boardTileDatas = new List<WorldTileData>();

	protected static TileDataContainer tileDataContainer;
	protected const string TileDataContainerPath = "Assets/Resources/Datas/TileDataContainer.asset";

	protected static Vector2 windowCenterPos = new Vector2(300, 150);
	protected static float tileButtonSize = 50.0f;

	protected static string EditorName;

	protected static void InitializeTileData()
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
	}

	protected static void OnOpenWindow<TWindow>() where TWindow : TileEditorWindow
	{
		var window = GetWindow<TWindow>();
		if (window != null)
		{
			window.titleContent = new GUIContent(EditorName);

			var size = GetBoardSize(boardTileDatas);
			var safeSize = size * 1.5f;

			window.position = new Rect(100, 100, safeSize.x, safeSize.y);

			window.Show();
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
		base.SaveData();

		EditorPrefs.SetFloat("windowCenterPosX", windowCenterPos.x);
		EditorPrefs.SetFloat("windowCenterPosY", windowCenterPos.y);
		EditorPrefs.SetFloat("tileButtonSize", tileButtonSize);


		AssetDatabase.SaveAssetIfDirty(tileDataContainer);
	}

	protected override void LoadData()
	{
		base.LoadData();

		float x = EditorPrefs.GetFloat("windowCenterPosX");
		float y = EditorPrefs.GetFloat("windowCenterPosY");

		windowCenterPos = new Vector2(x, y);

		tileButtonSize = EditorPrefs.GetFloat("tileButtonSize");
	}

	protected override void OnPressMouseButton()
	{
		windowCenterPos += Event.current.delta;
	}

	protected override void DrawAll()
	{
		DrawSpace(10);

		DrawAxis(Axis.Horizontal, () =>
		{
			DrawLabel("Center X :", 60, 20);
			windowCenterPos.x = (float)DrawField(FieldType.Float, windowCenterPos.x, 30, 20);

			DrawSpace(5);

			DrawLabel("Center Y :", 60, 20);
			windowCenterPos.y = (float)DrawField(FieldType.Float, windowCenterPos.y, 30, 20);

			DrawSpace(5);

			DrawLabel("Button Size :", 70, 20);
			tileButtonSize = (float)DrawField(FieldType.Float, tileButtonSize, 30, 20);
			
		});

		DrawSpace(15);

		DrawAxis(Axis.Horizontal, () =>
		{
			isIsometric = DrawToggle(200, 30, isIsometric, " Isometric 적용 여부");
		});

		DrawSpace(15);

		DrawAxis(Axis.Horizontal, () =>
		{
			DrawButton(50, 30, OnClickSave, "저장", null, null);

			DrawSpace(5);

			DrawButton(50, 30, OnClickClear, "초기화", null, null);
		});

		DrawSpace(15);

		DrawPalette();

		DrawSpace(15);

		DrawBoard();
	}

	protected virtual void DrawPalette()
	{

	}

	protected virtual void DrawBoard()
	{

	}

	private void OnClickSave()
	{
		SaveData();
	}

	protected virtual void OnClickClear()
	{

	}
}
