using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataContainer")]
public class PlayerDataContainer : ScriptableObject
{
	public TileDataContainer tileDataContainer;

	[Header("[현재 타일 내 위치]")]
	public int currentTileOrderIndex = 0;

	[Header("[타일 당 이동 시간]")]
	public float moveTimeByOneTile = 1.0f;

	[Header("[장착 중인 바디 파츠]")]
	[Header("[(0) 바디 / (1) 머리 / (2) 눈 / (3) 얼굴 / (4) 악세사리]")]
	[Space]
	public CharacterType[] characterMeshTypes = new CharacterType[5];

	[Header("[장착 중인 소품]")]
	[Space]
	public PropType[] characterPropTypes = null;

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
		if (partsType == CharacterPartsType.Accessory)
			return characterMeshTypes[4];

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
