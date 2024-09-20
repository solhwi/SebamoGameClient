
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

	public  virtual IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
	}

	protected virtual void Reset()
	{
#if UNITY_EDITOR
		playerDataContainer = AssetDatabase.LoadAssetAtPath<PlayerDataContainer>("Assets/Resources/Datas/PlayerDataContainer.asset");
#endif
	}
}
