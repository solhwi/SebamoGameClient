using System.Linq;
using UnityEditor;
using UnityEngine;

public class TileItemSettingWindow : TileEditorWindow
{
	private static string[] tileItems = null;

	private static string selectedItemCode = string.Empty;
	private static int selectedToggleIndex = 0;

	protected static ItemTable itemTable;
	protected const string ItemTablePath = "Assets/Bundles/Datas/Parser/ItemTable.asset";

	[MenuItem("Tools/타일 아이템 배치 도우미 %#S")]
	public static void OpenWindow()
	{
		InitializeTileData();
		InitializeTileItems();

		itemTable = AssetDatabase.LoadAssetAtPath<ItemTable>(ItemTablePath);
		if (itemTable == null)
		{
			Debug.LogError($"{ItemTablePath}에 아이템 테이블 없음");
			return;
		}

		EditorName = "타일 아이템 배치 도우미";

		windowCenterPos = new Vector2(450, 200);
	    tileButtonSize = 50.0f;

		OnOpenWindow<TileItemSettingWindow>();
	}

	private static void InitializeTileItems()
	{
		if (tileDataContainer.tileItems == null || tileDataContainer.tileItems.Length != boardTileDatas.Count)
		{
			tileItems = new string[boardTileDatas.Count];
			ClearTileItemData();
		}
		else
		{
			tileItems = tileDataContainer.tileItems.ToArray();
		}
	}

	private static void ClearTileItemData()
	{
		for (int i = 0; i < tileItems.Length; i++)
		{
			tileItems[i] = string.Empty;
		}
	}

	protected override void SaveData()
	{
		tileDataContainer.SetTileItems(tileItems);

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
			for (int i = 0; i < itemTable.fieldItemDataList.Count; i++)
			{
				var dropItemData = itemTable.fieldItemDataList[i];
				bool isToggleOn = DrawToggle(80, 30, selectedToggleIndex == i, dropItemData.key);
				if (isToggleOn)
				{
					selectedItemCode = dropItemData.key;
					selectedToggleIndex = i;
				}
			}
		});
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

				string currentItemCode = tileItems[i];

				DrawTileButton(xPos, yPos, tileButtonSize, tileButtonSize, windowCenterPos, () =>
				{
					OnClickIndex(i);
				}, tileItems[i]);
			}
		});
	}

	private void OnClickIndex(int index)
	{
		string currentItemCode = tileItems[index];

		// 기존에 순서가 있었다면
		if (currentItemCode != string.Empty)
		{
			tileItems[index] = string.Empty;
		}
		else
		{
			tileItems[index] = selectedItemCode;
		}
	}
}
