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

	[Header("[(0) 바디 / (1) 머리 / (2) 눈 / (3) 얼굴]")]
	[Space]
	public CharacterType[] characterMeshTypes = new CharacterType[4];

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

	public CharacterType GetCharacterTypeByPartsType(CharacterPartsType partsType)
	{
		if (partsType == CharacterPartsType.Face)
			return characterMeshTypes[3];

		if (partsType == CharacterPartsType.RightEye || partsType == CharacterPartsType.LeftEye)
			return characterMeshTypes[2];

		if(partsType == CharacterPartsType.BackHair || partsType == CharacterPartsType.FrontHair) 
			return characterMeshTypes[1];

		if (partsType == CharacterPartsType.Body)
			return characterMeshTypes[0];

		return CharacterType.UnityChan;
	}
}
