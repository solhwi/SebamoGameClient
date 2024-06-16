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

	[SerializeField] private MeshFilter leftEyeRenderer = null;
	[SerializeField] private MeshFilter rightEyeRenderer = null;

	[SerializeField] private SkinnedMeshRenderer faceRenderer = null;

	[SerializeField] private SkinnedMeshRenderer frontHairRenderer = null;
	[SerializeField] private SkinnedMeshRenderer backHairRenderer = null;

	[SerializeField] private List<GameObject> accessories = new List<GameObject>();

	private void Awake()
	{
		playerDataContainer.onChangeCharacterMeshType += SetMeshData;
	}

	private void OnDestroy()
	{
		playerDataContainer.onChangeCharacterMeshType -= SetMeshData;
	}

	/// <summary>
	/// 나중에 부위마다 각자 캐릭터 타입을 저장하여 불러오도록 수정
	/// </summary>
	private void SetMeshData()
	{
		costumeRenderer.sharedMesh = GetMesh(CharacterMeshType.Costume);
		costumeSkinRenderer.sharedMesh = GetMesh(CharacterMeshType.CostumeSkin);

		leftEyeRenderer.mesh = GetMesh(CharacterMeshType.LeftEye);
		rightEyeRenderer.mesh = GetMesh(CharacterMeshType.RightEye);

		faceRenderer.sharedMesh = GetMesh(CharacterMeshType.Face);

		frontHairRenderer.sharedMesh = GetMesh(CharacterMeshType.FrontHair);
		backHairRenderer.sharedMesh = GetMesh(CharacterMeshType.BackHair);
	}

	private Mesh GetMesh(CharacterMeshType meshType)
	{
		CharacterType characterType = playerDataContainer.GetCharacterTypeByMeshType(meshType);
		return dataContainer.GetMesh(characterType, meshType);
	}

}
