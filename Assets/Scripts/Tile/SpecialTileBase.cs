using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class SpecialTileBase : Tile
{
	[SerializeField] protected PlayerDataContainer playerDataContainer;

	public abstract void DoAction();

	private void Reset()
	{
		playerDataContainer = AssetDatabase.LoadAssetAtPath<PlayerDataContainer>("Assets/Resources/Datas/PlayerDataContainer.asset");
	}
}
