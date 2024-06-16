using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AccessoryType
{

}

/// <summary>
/// 캐릭터 아바타, 애니메이터, 메시, 악세사리 등을 세팅
/// </summary>
public class CharacterDataSetter : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private CharacterDataContainer dataContainer = null;

	[SerializeField] private SkinnedMeshRenderer costumeRenderer = null;
	[SerializeField] private SkinnedMeshRenderer costumeSkinRenderer = null;

	[SerializeField] private MeshFilter leftEyeFilter = null;
	[SerializeField] private MeshFilter rightEyeFilter = null;

	[SerializeField] private MeshRenderer leftEyeRenderer = null;
	[SerializeField] private MeshRenderer rightEyeRenderer = null;

	[SerializeField] private MeshFilter leftEyeCoverFilter = null;
	[SerializeField] private MeshFilter rightEyeCoverFilter = null;

	[SerializeField] private SkinnedMeshRenderer faceRenderer = null;

	[SerializeField] private SkinnedMeshRenderer frontHairRenderer = null;
	[SerializeField] private SkinnedMeshRenderer backHairRenderer = null;

	[SerializeField] private Animator animator = null;

	[SerializeField] private List<GameObject> accessories = new List<GameObject>();

	public void SetAvatar()
	{
		CharacterType characterType = playerDataContainer.GetCharacterTypeByMeshType(CharacterMeshType.Costume);
		animator.avatar = dataContainer.characterAvatars[(int)characterType];
	}

	public void SetMeshData()
	{
		costumeRenderer.sharedMesh = GetMesh(CharacterMeshType.Costume);
		costumeSkinRenderer.sharedMesh = GetMesh(CharacterMeshType.CostumeSkin);

		leftEyeFilter.mesh = GetMesh(CharacterMeshType.LeftEye);
		rightEyeFilter.mesh = GetMesh(CharacterMeshType.RightEye);

		leftEyeCoverFilter.mesh = GetMesh(CharacterMeshType.LeftEyeCover);
		rightEyeCoverFilter.mesh = GetMesh(CharacterMeshType.RightEyeCover);

		faceRenderer.sharedMesh = GetMesh(CharacterMeshType.Face);

		frontHairRenderer.sharedMesh = GetMesh(CharacterMeshType.FrontHair);
		backHairRenderer.sharedMesh = GetMesh(CharacterMeshType.BackHair);
	}

	public void SetMaterial()
	{
		CharacterType characterType = playerDataContainer.GetCharacterTypeByMeshType(CharacterMeshType.LeftEye);
		leftEyeRenderer.material = dataContainer.GetMaterial(characterType);

		characterType = playerDataContainer.GetCharacterTypeByMeshType(CharacterMeshType.RightEye);
		rightEyeRenderer.material = dataContainer.GetMaterial(characterType);
	}

	private Mesh GetMesh(CharacterMeshType meshType)
	{
		CharacterType characterType = playerDataContainer.GetCharacterTypeByMeshType(meshType);
		return dataContainer.GetMesh(characterType, meshType);
	}

}
