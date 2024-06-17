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
	BackHair = 1,
	FrontHair = 2,
	LeftEye = 3,
	RightEye = 4,
	Face = 5,
	Accessory = 6,
	Prop = 7,
	Max = 6,
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
	public GameObject[] characterPropPrefabs = new GameObject[(int)CharacterType.Max];

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

			case CharacterPartsType.BackHair:
				return characterBackHairPrefabs[(int)characterType];

			case CharacterPartsType.FrontHair:
				return characterFrontHairPrefabs[(int)characterType];

			case CharacterPartsType.LeftEye:
				return characterLeftEyePrefab;

			case CharacterPartsType.RightEye:
				return characterRightEyePrefab;

			case CharacterPartsType.Face:
				return characterFacePrefab;

			case CharacterPartsType.Accessory:
				return characterHeadAccessoryPrefabs[(int)characterType];

			case CharacterPartsType.Prop:
				return characterPropPrefabs[(int)characterType];
		}

		return null;
	}

	public Mesh GetMesh(CharacterType characterType, CharacterPartsType partsType)
	{
		switch(partsType)
		{
			case CharacterPartsType.LeftEye:
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
			case CharacterPartsType.LeftEye:
				return characterEyeMaterials[(int)characterType];

			case CharacterPartsType.RightEye:
				return characterEyeMaterials[(int)characterType];
		}

		return null;
	}
}
