using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomEditorWindow : EditorWindow
{
	protected static Texture2D pressOffTexture;
	protected static Texture2D pressOnTexture;
	protected static Texture2D hoverTexture;

	protected bool isIsometric = true;

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
	private static bool isPointerDown = false;

	protected virtual void SaveData()
	{
		EditorPrefs.SetBool("isIsometric", isIsometric);
	}

	protected virtual void LoadData()
	{
		isIsometric = EditorPrefs.GetBool("isIsometric");

		pressOffTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edit/PressOff.png");
		pressOnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edit/PressOn.png");
		hoverTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edit/Hover.png");
	}

	private void OnEnable()
	{
		dpi = Screen.dpi;
		isPointerDown = false;

		LoadData();
	}

	private void OnDisable()
	{
		isPointerDown = false;
		SaveData();
	}

	private void Update()
	{
		if (isIsometric)
		{
			Repaint();
		}
	}

	private void OnGUI()
	{
		if (EditorApplication.isPlaying)
		{
			EditorGUILayout.LabelField("재생 중...");
		}
		else
		{
			DrawAxis(Axis.Vertical, DrawAll);
		}

		if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			isPointerDown = true;
		}
		else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
		{
			isPointerDown = false;
		}

		if (isPointerDown)
		{
			OnPressMouseButton();
		}
	}

	protected virtual void OnPressMouseButton()
	{

	}

	protected virtual void DrawAll()
	{
		
	}

	protected bool DrawToggle(float width, float height, bool toggle, string buttonName)
	{
		return GUILayout.Toggle(toggle, buttonName, GUILayout.Width(width), GUILayout.Height(height));
	}

	protected void DrawButton(float width, float height, Action onClickButton, string buttonName, Texture2D normalTexture, Texture2D activeTexture)
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

	protected void DrawTileButton(float x, float y, float width, float height, Vector2 centerPos, Action onClickButton, string buttonName)
	{
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

		float buttonWidth = width;
		float buttonHeight = height;

		float adjustValue = 0.0f;

		if (isIsometric)
		{
			if (pressOffTexture != null)
			{
				buttonStyle.normal.background = pressOffTexture;
				buttonWidth = pressOffTexture.width;
				buttonHeight = pressOffTexture.height;
			}

			if (pressOnTexture != null)
			{
				buttonStyle.active.background = pressOnTexture;
			}

			if (hoverTexture != null)
			{
				buttonStyle.hover.background = hoverTexture;
			}

			// 마름모 텍스처로 인한 보정 값
			adjustValue = y * buttonWidth / 2;
		}

		// y는 좌표계가 반대라서 -
		if (GUI.Button(new Rect(centerPos.x + x * buttonWidth, centerPos.y - (y * buttonHeight + adjustValue), buttonWidth, buttonHeight), buttonName, buttonStyle))
		{
			onClickButton?.Invoke();
		}
	}

	protected void DrawLabel(string label, float width, float height)
	{
		EditorGUILayout.LabelField(label, GUILayout.Width(width), GUILayout.Height(height));
	}

	protected void DrawPrefixLabel(string label)
	{
		EditorGUILayout.PrefixLabel(label);
	}

	protected void DrawLabel(Texture2D texture, float width, float height)
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

	protected object DrawField(FieldType fieldType, object oldValue, float width, float height)
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

	protected void DrawAxis(Axis type, Action onDrawLayout, float width = 0.0f, float height = 0.0f)
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

	protected void DrawScrollView(ref Vector2 scrollPosition, Action onDrawLayout, float width = 0.0f, float height = 0.0f)
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

	protected void DrawSpace(float width = 6.0f)
	{
		GUILayout.Space(width);
	}

}
