using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TileHelperWindow : EditorWindow
{
	public enum Axis
	{
		Horizontal,
		Vertical,
	}

	public enum FieldType
	{
		String,
		Float,
		Int,
		Object,
	}

	private static float dpi = 0.0f;
	private static List<WorldTileData> boardTileDatas = new List<WorldTileData>();

	private static TileDataContainer tileDataContainer;
	private const string TileDataContainerPath = "Assets/Resources/Datas/TileDataContainer.asset";

	private static List<int> makingIndexList = new List<int>();

	private Texture2D pressOffTexture;
	private Texture2D pressOnTexture;

	private Vector2 boardTileScrollPos;

	private const string pressOffTexturePathKey = "pressOffTexture";
	private const string pressOnTexturePathKey = "pressOnTexture";


	[MenuItem("Tools/TileHelperWindow %#D")]
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

		makingIndexList.Clear();

		var window = GetWindow<TileHelperWindow>();
		if (window != null)
		{
			window.Show();
		}
	}

	private void SaveData()
	{
		string path = pressOffTexture != null ? AssetDatabase.GetAssetPath(pressOffTexture) : string.Empty;
		EditorPrefs.SetString(pressOffTexturePathKey, path);

		path = pressOnTexture != null ? AssetDatabase.GetAssetPath(pressOnTexture) : string.Empty;
		EditorPrefs.SetString(pressOnTexturePathKey, path);
	}

	private void LoadData()
	{
		string path = EditorPrefs.GetString(pressOffTexturePathKey, string.Empty);
		if (!string.IsNullOrEmpty(path))
		{
			pressOffTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		}

		path = EditorPrefs.GetString(pressOnTexturePathKey, string.Empty);
		if (!string.IsNullOrEmpty(path))
		{
			pressOnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		}
	}

	private void OnEnable()
	{
		dpi = Screen.dpi;

		LoadData();
	}

	private void OnDisable()
	{
		SaveData();
	}

	private void OnGUI()
	{
		DrawAxis(Axis.Horizontal, DrawAll);
	}

	private void DrawGetTileTexture()
	{
		DrawAxis(Axis.Horizontal, () =>
		{
			DrawAxis(Axis.Vertical, () =>
			{
				DrawLabel("[Press Off 텍스처]", 150, 30);
				pressOffTexture = DrawField(FieldType.Object, pressOffTexture, 70, 70) as Texture2D;
			}, 120, 120);

			DrawAxis(Axis.Vertical, () =>
			{
				DrawLabel("[Press On 텍스처]", 150, 30);
				pressOnTexture = DrawField(FieldType.Object, pressOnTexture, 70, 70) as Texture2D;
			}, 120, 120);
		});
	}

	private void DrawAll()
	{
		DrawAxis(Axis.Vertical, () =>
		{
			DrawGetTileTexture();

			DrawSpace(20);

			DrawBoardIndexingHelper();
		});
	}

	private void DrawBoardIndexingHelper()
	{
		DrawLabel("[보드 타일 인덱싱 도우미]", 200, 20);

		DrawAxis(Axis.Vertical, () =>
		{
			DrawScrollView(ref boardTileScrollPos, () =>
			{
				var tileDataList = boardTileDatas.ToList();

				for (int i = 0; i < tileDataList.Count; i++)
				{
					var data = tileDataList[i];

					float xPos = data.tileWorldPosition.x;
					float yPos = data.tileWorldPosition.y;

					DrawButton(xPos, yPos, 30, 30, new Vector2(150, 80), () => { Debug.Log($"{i}번 눌림"); }, $"{i}", pressOffTexture, pressOnTexture);
				}
			});
		});
	}

	private void DrawButton(float width, float height, Action onClickButton, string buttonName, Texture2D normalTexture, Texture2D activeTexture)
	{
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

		if (normalTexture != null)
		{	
			buttonStyle.normal.background = normalTexture;
		}

		if (normalTexture != null)
		{
			buttonStyle.active.background = activeTexture;
		}

		if (GUILayout.Button(buttonName, buttonStyle, GUILayout.Width(width), GUILayout.Height(height)))
		{
			onClickButton?.Invoke();
		}
	}

	private void DrawButton(float x, float y, float width, float height, Vector2 centerPos, Action onClickButton, string buttonName, Texture2D normalTexture, Texture2D activeTexture)
	{
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

		float buttonWidth = width;
		float buttonHeight = height;

		if (normalTexture != null)
		{
			buttonStyle.normal.background = normalTexture;
			buttonWidth = normalTexture.width;
			buttonHeight = normalTexture.height;
		}

		if (normalTexture != null)
		{
			buttonStyle.active.background = activeTexture;
		}

		// y는 좌표계가 반대라서 -
		if (GUI.Button(new Rect(centerPos.x + x * buttonWidth, centerPos.y - y * buttonHeight, buttonWidth, buttonHeight), buttonName, buttonStyle))
		{
			onClickButton?.Invoke();
		}
	}

	private void DrawLabel(string label, float width, float height)
	{
		EditorGUILayout.LabelField(label, GUILayout.Width(width), GUILayout.Height(height));
	}

	private void DrawLabel(Texture2D texture, float width, float height)
	{
		bool isValidWidthAndHeight = width > 0.01f && height > 0.01f;
		if (isValidWidthAndHeight)
		{
			GUILayout.Label(texture, GUILayout.Width(width), GUILayout.Height(height));
		}
		else
		{
			GUILayout.Label(texture, GUILayout.Width(texture.width), GUILayout.Height(texture.height));
		}	
	}

	private void DrawLabel(Texture2D texture, float x, float y, float width, float height)
	{
		bool isValidWidthAndHeight = width > 0.01f && height > 0.01f;
		if (isValidWidthAndHeight)
		{
			GUI.Label(new Rect(x, y, width, height), texture);
		}
		else
		{
			GUI.Label(new Rect(x, y, texture.width, texture.height), texture);
		}

	}

	private object DrawField(FieldType fieldType, object oldValue, float width, float height)
	{
		switch (fieldType)
		{
			case FieldType.String:
				return EditorGUILayout.TextField((string)oldValue, GUILayout.Width(width), GUILayout.Height(height));

			case FieldType.Float:
				return EditorGUILayout.FloatField((float)oldValue, GUILayout.Width(width), GUILayout.Height(height));

			case FieldType.Int:
				return EditorGUILayout.IntField((int)oldValue, GUILayout.Width(width), GUILayout.Height(height));

			case FieldType.Object:
				return EditorGUILayout.ObjectField((UnityEngine.Object)oldValue, typeof(Texture2D), GUILayout.Width(width), GUILayout.Height(height));
			
			default:
				return EditorGUILayout.TextField((string)oldValue, GUILayout.Width(width), GUILayout.Height(height));
		}
	}

	private void DrawAxis(Axis type, Action onDrawLayout, float width = 0.0f, float height = 0.0f)
	{
		bool isValidWidthAndHeight = width > 0.01f && height > 0.01f;
		switch (type)
		{
			case Axis.Horizontal:

				if (isValidWidthAndHeight)
				{
					EditorGUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(height));
				}
				else
				{
					EditorGUILayout.BeginHorizontal();
				}

				onDrawLayout?.Invoke();
				EditorGUILayout.EndHorizontal();
				break;

			case Axis.Vertical:

				if (isValidWidthAndHeight)
				{
					EditorGUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
				}
				else
				{
					EditorGUILayout.BeginVertical();
				}

				onDrawLayout?.Invoke();
				EditorGUILayout.EndVertical();
				break;
		}
	}

	public void DrawScrollView(ref Vector2 scrollPosition, Action onDrawLayout, float width = 0.0f, float height = 0.0f)
	{
		bool isValidWidthAndHeight = width > 0.01f && height > 0.01f;

		if (isValidWidthAndHeight)
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(width), GUILayout.Height(height));
		}
		else
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		}
		

		onDrawLayout?.Invoke();

		EditorGUILayout.EndScrollView();
	}

	public void DrawSpace(float width = 6.0f)
	{
		GUILayout.Space(width);
	}

}
