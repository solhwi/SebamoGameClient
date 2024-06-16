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

public enum CharacterMeshType
{
	Costume = 0,
	CostumeSkin = 1,
	LeftEye = 2,
	RightEye = 3,
	FrontHair = 4,
	BackHair = 5,
	Face = 6,
	LeftEyeCover = 7,
	RightEyeCover = 8,
	Max = 7,
}

[CreateAssetMenu(fileName = "CharacterDataContainer")]
public class CharacterDataContainer : ScriptableObject
{
	[Header("[0 : UnityChan, 1 : Yuco, 2 : Misaki]")]
	[Space]

    public Avatar[] characterAvatars = new Avatar[(int)CharacterType.Max];

	public Mesh[] characterCostumeMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterCostumeSkinMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterLeftEyeMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterRightEyeMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterFaceMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterFrontHairMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterBackHairMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterLeftEyeCoverMeshes = new Mesh[(int)CharacterType.Max];
	public Mesh[] characterRightEyeCoverMeshes = new Mesh[(int)CharacterType.Max];

	public Material[] characterEyeMaterials = new Material[(int)CharacterType.Max];

	public Mesh GetMesh(CharacterType characterType, CharacterMeshType meshType)
	{
		switch(meshType)
		{
			case CharacterMeshType.Costume:
				return characterCostumeMeshes[(int)characterType];

			case CharacterMeshType.CostumeSkin:
				return characterCostumeSkinMeshes[(int)characterType];

			case CharacterMeshType.LeftEye:
				return characterLeftEyeMeshes[(int)characterType];

			case CharacterMeshType.RightEye:
				return characterRightEyeMeshes[(int)characterType];

			case CharacterMeshType.FrontHair:
				return characterFrontHairMeshes[(int)characterType];

			case CharacterMeshType.BackHair:
				return characterBackHairMeshes[(int)characterType];

			case CharacterMeshType.Face:
				return characterFaceMeshes[(int)characterType];

			case CharacterMeshType.LeftEyeCover:
				return characterLeftEyeCoverMeshes[(int)characterType];

			case CharacterMeshType.RightEyeCover:
				return characterRightEyeCoverMeshes[(int)characterType];
		}

		return null;
	}

	public Material GetMaterial(CharacterType characterType)
	{
		return characterEyeMaterials[(int)characterType];
	}
}
