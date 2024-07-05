using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class SpecialTileBase : Tile
{
	[SerializeField] protected PlayerDataContainer playerDataContainer;

	public async virtual Task DoAction()
	{
		await Task.Yield();
	}

	private void Reset()
	{
		playerDataContainer = AssetDatabase.LoadAssetAtPath<PlayerDataContainer>("Assets/Resources/Datas/PlayerDataContainer.asset");
	}
}
