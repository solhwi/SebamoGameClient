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

	[Header("[(0) 바디 / (1) 바디 피부 / (2) 왼쪽 눈 / (3) 오른쪽 눈 / (4) 앞 머리]")]
	[Header("[(5) 뒷 머리, (6) 얼굴, (7) 왼쪽 눈 커버 / (8) 오른쪽 눈 커버]")]
	[Space]
	public CharacterType[] characterMeshTypes = new CharacterType[(int)CharacterMeshType.Max];

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

	public CharacterType GetCharacterTypeByMeshType(CharacterMeshType meshType)
	{
		return characterMeshTypes[(int)meshType];
	}
}
