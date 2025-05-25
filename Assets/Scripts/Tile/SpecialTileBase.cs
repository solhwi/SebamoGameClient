
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SpecialTileType
{
	Jump,
	RollBack,
	Teleport,
	DontMove,
}

public abstract class SpecialTileBase : Tile
{
	[SerializeField] public SpecialTileType specialTileType;

	public virtual void DoAction()
	{
		
	}

	public  virtual IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
	}

	protected virtual void Reset()
	{
	}
}
