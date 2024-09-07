using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SpecialTileType
{
	Jump,
	RollBack,
	Teleport,
}

public abstract class SpecialTileBase : Tile
{
	[SerializeField] public SpecialTileType specialTileType;
	[SerializeField] protected PlayerDataContainer playerDataContainer;

	public virtual void DoAction()
	{
		
	}

	public async virtual Task OnDoTileAction(int currentOrder, int nextOrder)
	{
		await Task.Yield();
	}

	protected virtual void Reset()
	{
		playerDataContainer = AssetDatabase.LoadAssetAtPath<PlayerDataContainer>("Assets/Resources/Datas/PlayerDataContainer.asset");
	}
}
