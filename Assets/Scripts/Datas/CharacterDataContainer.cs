using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
	UnityChan = 0,
	Yuco = 1,
	Misaki = 2,
	Max = 3,
}

public enum CharacterPartsType
{
	Body = 0,
	Hair = 1,
	FrontHair = 2,
	Eye = 3, // 왼쪽 눈
	RightEye = 4,
	Face = 5,
	Accessory = 6,
	Max,
}

public enum PropType
{
	GreatSword = 0,
	TwinDagger_L = 1,
	TwinDagger_R = 2,
	Axe = 3,
	PickAx = 4,
	Shovel = 5,
	Umbrella = 6,
	Net = 7,
	Max,
}


[CreateAssetMenu(fileName = "CharacterDataContainer")]
public class CharacterDataContainer : ScriptableObject
{
	[Header("[0 : UnityChan, 1 : Yuco, 2 : Misaki]")]
	[Space]

    public Avatar[] characterAvatars = new Avatar[(int)CharacterType.Max];

	public GameObject[] characterBodyPrefabs = new GameObject[(int)CharacterType.Max];
	public GameObject[] characterBackHairPrefabs = new GameObject[(int)CharacterType.Max];
	public GameObject[] characterFrontHairPrefabs = new GameObject[(int)CharacterType.Max];
	public GameObject[] characterHeadAccessoryPrefabs = new GameObject[(int)CharacterType.Max];

	[Header("[0 : 대검 / 1, 2 : 쌍검 /  3 : 도끼 / 4 : 곡괭이 / 5 : 삽 / 6 : 우산 / 7 : 잠자리채]")]
	[Space]
	public GameObject[] characterPropPrefabs = new GameObject[(int)PropType.Max];

	public GameObject characterLeftEyePrefab = null;
	public GameObject characterRightEyePrefab = null;
	public GameObject characterFacePrefab = null;

	public Mesh[] characterLeftEyeMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterRightEyeMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterFaceMeshes = new Mesh[(int)CharacterType.Max];

	public Material[] characterEyeMaterials = new Material[(int)CharacterType.Max];

	public GameObject GetPartsObject(CharacterType characterType, CharacterPartsType meshType)
	{
		switch (meshType)
		{
			case CharacterPartsType.Body:
				return characterBodyPrefabs[(int)characterType];

			case CharacterPartsType.Hair:
				return characterBackHairPrefabs[(int)characterType];

			case CharacterPartsType.FrontHair:
				return characterFrontHairPrefabs[(int)characterType];

			case CharacterPartsType.Eye:
				return characterLeftEyePrefab;

			case CharacterPartsType.RightEye:
				return characterRightEyePrefab;

			case CharacterPartsType.Face:
				return characterFacePrefab;

			case CharacterPartsType.Accessory:
				return characterHeadAccessoryPrefabs[(int)characterType];
		}

		return null;
	}

	public GameObject GetPropObject(PropType propType)
	{
		return characterPropPrefabs[(int)propType];
	}

	public Mesh GetMesh(CharacterType characterType, CharacterPartsType partsType)
	{
		switch(partsType)
		{
			case CharacterPartsType.Eye:
				return characterLeftEyeMeshes[(int)characterType];

			case CharacterPartsType.RightEye:
				return characterRightEyeMeshes[(int)characterType];

			case CharacterPartsType.Face:
				return characterFaceMeshes[(int)characterType];
		}

		return null;
	}

	public Material GetMaterial(CharacterType characterType, CharacterPartsType partsType)
	{
		switch (partsType)
		{
			case CharacterPartsType.Eye:
				return characterEyeMaterials[(int)characterType];

			case CharacterPartsType.RightEye:
				return characterEyeMaterials[(int)characterType];
		}

		return null;
	}
}
