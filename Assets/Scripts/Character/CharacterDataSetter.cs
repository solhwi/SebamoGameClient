using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TransformExtension
{
	public static Transform RecursiveFindChild(this Transform parent, string childName)
	{
		Transform child = null;
		for (int i = 0; i < parent.childCount; i++)
		{
			child = parent.GetChild(i);
			if (child.name == childName)
			{
				break;
			}
			else
			{
				child = RecursiveFindChild(child, childName);
				if (child != null)
				{
					break;
				}
			}
		}

		return child;
	}

	public static void ResetTransform(this Transform parentTransform)
	{
		// 부모 Transform의 자식들을 순회하면서 초기화합니다.
		foreach (Transform child in parentTransform)
		{
			// 자식의 위치, 회전, 스케일을 초기화합니다.
			child.localPosition = Vector3.zero;
			child.localRotation = Quaternion.identity;
			child.localScale = Vector3.one;

			// 재귀적으로 하위 자식도 초기화합니다.
			ResetTransform(child);
		}
	}
}
/// <summary>
/// 캐릭터 아바타, 애니메이터, 메시, 악세사리 등을 세팅
/// </summary>
public class CharacterDataSetter : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private CharacterDataContainer dataContainer = null;

	[SerializeField] private CharacterAnimationController animationController = null;

	[SerializeField] private Transform bodyRoot = null;

	public void SetAvatar()
	{
		CharacterType characterType = playerDataContainer.GetCharacterTypeByPartsType(CharacterPartsType.Body);
		var avatar = dataContainer.characterAvatars[(int)characterType];

		animationController.SetAvatar(avatar);
	}

	public void SetMeshs()
	{
		Transform leftEye = GetLeftEyeTransform();
		var leftEyeFilter = leftEye.GetComponentInChildren<MeshFilter>();

		leftEyeFilter.mesh = GetMesh(CharacterPartsType.LeftEye);

		var leftEyeRenderer = leftEye.GetComponentInChildren<MeshRenderer>();
		leftEyeRenderer.material = GetMaterial(CharacterPartsType.LeftEye);

		Transform rightEye = GetRightEyeTransform();
		var rightEyeFilter = rightEye.GetComponentInChildren<MeshFilter>();
		rightEyeFilter.mesh = GetMesh(CharacterPartsType.RightEye);

		var rightEyeRenderer = rightEye.GetComponentInChildren<MeshRenderer>();
		rightEyeRenderer.material = GetMaterial(CharacterPartsType.LeftEye);

		Transform face = GetHeadTransform();
		var faceRenderer = face.GetComponentInChildren<SkinnedMeshRenderer>();
		faceRenderer.sharedMesh = GetMesh(CharacterPartsType.Face);
	}

	public void SetParts()
	{
		var bodyPrefab = GetParts(CharacterPartsType.Body);
		var bodyObj = Instantiate(bodyPrefab, bodyRoot);
		bodyObj.transform.localPosition = Vector3.zero;
		bodyObj.transform.localRotation = Quaternion.identity;

		Transform headRoot = GetHeadTransform();

		var facePrefab = GetParts(CharacterPartsType.Face);
		var faceObj = Instantiate(facePrefab, headRoot);

		var backHairPrefab = GetParts(CharacterPartsType.BackHair);
		var backHairObj = Instantiate(backHairPrefab, headRoot);

		var backHairRootHead = backHairObj.transform.RecursiveFindChild("root_head");		
		backHairRootHead.localPosition = Vector3.zero;
		backHairRootHead.localRotation = Quaternion.identity;

		var frontHairPrefab = GetParts(CharacterPartsType.FrontHair);
		var frontHairObj = Instantiate(frontHairPrefab, headRoot);

		var frontHairRootHead = frontHairObj.transform.RecursiveFindChild("root_head");
		frontHairRootHead.localPosition = Vector3.zero;
		frontHairRootHead.localRotation = Quaternion.identity;

		Transform leftEyeTransform = GetLeftEyeTransform();

		var leftEyePrafab = GetParts(CharacterPartsType.LeftEye);
		var leftEyeObj = Instantiate(leftEyePrafab, leftEyeTransform);

		Transform rightEyeTransform = GetRightEyeTransform();

		var rightEyePrafab = GetParts(CharacterPartsType.RightEye);
		var rightEyeObj = Instantiate(rightEyePrafab, rightEyeTransform);
	}

	private Transform GetHeadTransform()
	{ 
		return GetTransform("Head");
	}

	private Transform GetLeftEyeTransform()
	{
		return GetTransform("Eye_L");
	}

	private Transform GetRightEyeTransform()
	{
		return GetTransform("Eye_R");
	}

	private Transform GetTransform(string name)
	{
		return bodyRoot.RecursiveFindChild(name);
	}

	private GameObject GetParts(CharacterPartsType partsType)
	{
		CharacterType characterType = playerDataContainer.GetCharacterTypeByPartsType(partsType);
		return dataContainer.GetPartsObject(characterType, partsType);
	}

	private Mesh GetMesh(CharacterPartsType partsType)
	{
		CharacterType characterType = playerDataContainer.GetCharacterTypeByPartsType(partsType);
		return dataContainer.GetMesh(characterType, partsType);
	}

	private Material GetMaterial(CharacterPartsType partsType)
	{
		CharacterType characterType = playerDataContainer.GetCharacterTypeByPartsType(partsType);
		return dataContainer.GetMaterial(characterType, partsType);
	}

}
