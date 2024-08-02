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
	[SerializeField] private Inventory inventory;
	[SerializeField] private CharacterDataContainer dataContainer = null;

	[SerializeField] private CharacterAnimationController animationController = null;

	[SerializeField] private Transform bodyRoot = null;

	private List<GameObject> partsObjList = new List<GameObject>();

	public void DoFullSetting()
	{
		MakeParts();
		SetMeshes();
		SetAvatar();
	}

	public void SetAvatar()
	{
		CharacterType characterType = inventory.GetCurrentPartsCharacterType(CharacterPartsType.Body);
		if (characterType == CharacterType.Max)
			return;

		var avatar = dataContainer.characterAvatars[(int)characterType];
		animationController.SetAvatar(avatar);
	}

	public void SetMeshes()
	{
		Transform leftEye = GetLeftEyeTransform();
		var leftEyeFilter = leftEye.GetComponentInChildren<MeshFilter>();

		leftEyeFilter.mesh = GetMesh(CharacterPartsType.Eye);

		var leftEyeRenderer = leftEye.GetComponentInChildren<MeshRenderer>();
		leftEyeRenderer.material = GetMaterial(CharacterPartsType.Eye);

		Transform rightEye = GetRightEyeTransform();
		var rightEyeFilter = rightEye.GetComponentInChildren<MeshFilter>();
		rightEyeFilter.mesh = GetMesh(CharacterPartsType.RightEye);

		var rightEyeRenderer = rightEye.GetComponentInChildren<MeshRenderer>();
		rightEyeRenderer.material = GetMaterial(CharacterPartsType.Eye);

		Transform face = GetHeadTransform();
		var faceRenderer = face.GetComponentInChildren<SkinnedMeshRenderer>();
		faceRenderer.sharedMesh = GetMesh(CharacterPartsType.Face);
	}

	public void MakeParts()
	{
		// 기존 파츠들 파괴
		foreach (var obj in partsObjList)
		{
			DestroyImmediate(obj);
		}

		partsObjList.Clear();

		// 바디
		var bodyPrefab = GetParts(CharacterPartsType.Body);
		if (bodyPrefab != null)
		{
			var bodyObj = Instantiate(bodyPrefab, bodyRoot);
			bodyObj.transform.localPosition = Vector3.zero;
			bodyObj.transform.localRotation = Quaternion.identity;

			partsObjList.Add(bodyObj);
		}

		// 머리
		Transform headRoot = GetHeadTransform();

		// 얼굴
		var facePrefab = GetParts(CharacterPartsType.Face);
		if (facePrefab != null)
		{
			var faceObj = Instantiate(facePrefab, headRoot);
			partsObjList.Add(faceObj);
		}

		// 뒷머리
		var backHairPrefab = GetParts(CharacterPartsType.Hair);
		if (backHairPrefab != null)
		{
			var backHairObj = Instantiate(backHairPrefab, headRoot);

			var backHairRootHead = backHairObj.transform.RecursiveFindChild("root_head");
			backHairRootHead.localPosition = Vector3.zero;
			backHairRootHead.localRotation = Quaternion.identity;

			partsObjList.Add(backHairObj);
		}

		// 앞머리
		var frontHairPrefab = GetParts(CharacterPartsType.FrontHair);
		if (frontHairPrefab != null)
		{
			var frontHairObj = Instantiate(frontHairPrefab, headRoot);

			var frontHairRootHead = frontHairObj.transform.RecursiveFindChild("root_head");
			frontHairRootHead.localPosition = Vector3.zero;
			frontHairRootHead.localRotation = Quaternion.identity;

			partsObjList.Add(frontHairObj);
		}

		// 머리 악세사리
		var headAccessoryPrefab = GetParts(CharacterPartsType.Accessory);
		if (headAccessoryPrefab != null)
		{
			var headAccessoryObj = Instantiate(headAccessoryPrefab, headRoot);
			partsObjList.Add(headAccessoryObj);
		}
		
		// 왼쪽 눈
		Transform leftEyeTransform = GetLeftEyeTransform();
		var leftEyePrafab = GetParts(CharacterPartsType.Eye);
		if (leftEyePrafab != null)
		{
			var leftEyeObj = Instantiate(leftEyePrafab, leftEyeTransform);
			partsObjList.Add(leftEyeObj);
		}

		// 오른쪽 눈
		Transform rightEyeTransform = GetRightEyeTransform();
		var rightEyePrafab = GetParts(CharacterPartsType.RightEye);
		if (rightEyePrafab != null)
		{
			var rightEyeObj = Instantiate(rightEyePrafab, rightEyeTransform);
			partsObjList.Add(rightEyeObj);
		}

		// 소품
		foreach (var propType in inventory.GetEquippedPropType())
		{
			if (propType == PropType.Max)
				continue;

			Transform propTransform = GetPropTransform(propType);
			var propPrefab = dataContainer.GetPropObject(propType);
			var propObj = Instantiate(propPrefab, propTransform);

			propObj.transform.localPosition = Vector3.zero;
			propObj.transform.localRotation = Quaternion.identity;

			partsObjList.Add(propObj);
		}
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

	private Transform GetPropTransform(PropType type)
	{
		if (type == PropType.TwinDagger_L)
			return GetTransform("Prop_L");

		return GetTransform("Prop_R");
	}

	private Transform GetTransform(string name)
	{
		return bodyRoot.RecursiveFindChild(name);
	}

	private GameObject GetParts(CharacterPartsType partsType)
	{
		CharacterType characterType = inventory.GetCurrentPartsCharacterType(partsType);
		if (characterType == CharacterType.Max)
			return null;

		return dataContainer.GetPartsObject(characterType, partsType);
	}

	private Mesh GetMesh(CharacterPartsType partsType)
	{
		CharacterType characterType = inventory.GetCurrentPartsCharacterType(partsType);
		if (characterType == CharacterType.Max)
			return null;

		return dataContainer.GetMesh(characterType, partsType);
	}

	private Material GetMaterial(CharacterPartsType partsType)
	{
		CharacterType characterType = inventory.GetCurrentPartsCharacterType(partsType);
		if (characterType == CharacterType.Max)
			return null;

		return dataContainer.GetMaterial(characterType, partsType);
	}

}
