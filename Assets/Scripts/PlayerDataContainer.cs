using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataContainer")]
public class PlayerDataContainer : ScriptableObject
{
	public TileDataContainer tileDataContainer;
	public int currentTileOrderIndex = 0;
	public float moveTimeByOneTile = 1.0f;

	public CharacterType[] characterMeshTypes = new CharacterType[(int)CharacterMeshType.Max];

	public event Action onChangeCharacterMeshType = null;

	public void SaveCurrentOrderIndex(int currentTileOrderIndex)
	{
		if (tileDataContainer.tileOrders.Length > currentTileOrderIndex)
		{
			this.currentTileOrderIndex = currentTileOrderIndex;
		}
		else
		{
			this.currentTileOrderIndex = tileDataContainer.tileOrders.Length - 1;
		}

		AssetDatabase.SaveAssetIfDirty(this);
	}

	public void ResetCharacterMeshType(CharacterType characterType)
	{
		for(int i = 0; i < characterMeshTypes.Length; i++)
		{
			characterMeshTypes[i] = characterType;
		}

		onChangeCharacterMeshType?.Invoke();
	}

	public CharacterType GetCharacterTypeByMeshType(CharacterMeshType meshType)
	{
		return characterMeshTypes[(int)meshType];
	}
}
