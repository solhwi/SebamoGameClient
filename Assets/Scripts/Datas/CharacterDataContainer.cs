using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

	[HideInInspector] public List<GameObject> characterBodyPrefabs = new List<GameObject>();
	[SerializeField] private AssetReferenceGameObject[] characterBodyPrefabRefs = new AssetReferenceGameObject[(int)CharacterType.Max];

	[HideInInspector] public List<GameObject> characterBackHairPrefabs = new List<GameObject>();
	[SerializeField] private AssetReferenceGameObject[] characterBackHairPrefabRefs = new AssetReferenceGameObject[(int)CharacterType.Max];

	[HideInInspector] public List<GameObject> characterFrontHairPrefabs = new List<GameObject>();
	[SerializeField] private AssetReferenceGameObject[] characterFrontHairPrefabRefs = new AssetReferenceGameObject[(int)CharacterType.Max];

	[HideInInspector] public List<GameObject> characterHeadAccessoryPrefabs = new List<GameObject>();
	[SerializeField] private AssetReferenceGameObject[] characterHeadAccessoryPrefabRefs = new AssetReferenceGameObject[(int)CharacterType.Max];

	[Header("[0 : 대검 / 1, 2 : 쌍검 /  3 : 도끼 / 4 : 곡괭이 / 5 : 삽 / 6 : 우산 / 7 : 잠자리채]")]
	[Space]
	[HideInInspector] public List<GameObject> characterPropPrefabs = new List<GameObject>();
	[SerializeField] private AssetReferenceGameObject[] characterPropPrefabRefs = new AssetReferenceGameObject[(int)CharacterType.Max];

	[HideInInspector] public GameObject characterLeftEyePrefab = null;
	[SerializeField] private AssetReferenceGameObject characterLeftEyePrefabRef = null;

	[HideInInspector] public GameObject characterRightEyePrefab = null;
	[SerializeField] private AssetReferenceGameObject characterRightEyePrefabRef	= null;

	[HideInInspector] public GameObject characterFacePrefab = null;
	[SerializeField] private AssetReferenceGameObject characterFacePrefabRef = null;

	[HideInInspector] public List<Mesh> characterLeftEyeMeshes = new List<Mesh>();
	[SerializeField] private AssetReferenceT<Mesh>[] characterLeftEyeMeshRefs = new AssetReferenceT<Mesh>[(int)CharacterType.Max];

	[HideInInspector] public List<Mesh> characterRightEyeMeshes = new List<Mesh>();
	[SerializeField] private AssetReferenceT<Mesh>[] characterRightEyeMeshRefs = new AssetReferenceT<Mesh>[(int)CharacterType.Max];

	[HideInInspector] public List<Mesh> characterFaceMeshes = new List<Mesh>();
	[SerializeField] private AssetReferenceT<Mesh>[] characterFaceMeshRefs = new AssetReferenceT<Mesh>[(int)CharacterType.Max];

	[HideInInspector] public List<Material> characterEyeMaterials = new List<Material>();
	[SerializeField] private AssetReferenceT<Material>[] characterEyeMaterialRefs = new AssetReferenceT<Material>[(int)CharacterType.Max];

	public IEnumerator PreLoadCharacterParts()
	{
		characterBodyPrefabs.Clear();
		foreach (var reference in characterBodyPrefabRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<GameObject>(reference, (o) =>
			{
				characterBodyPrefabs.Add(o);
			});
		}

		characterBackHairPrefabs.Clear();
		foreach (var reference in characterBackHairPrefabRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<GameObject>(reference, (o) =>
			{
				characterBackHairPrefabs.Add(o);
			});
		}

		characterFrontHairPrefabs.Clear();
		foreach (var reference in characterFrontHairPrefabRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<GameObject>(reference, (o) =>
			{
				characterFrontHairPrefabs.Add(o);
			});
		}

		characterHeadAccessoryPrefabs.Clear();
		foreach (var reference in characterHeadAccessoryPrefabRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<GameObject>(reference, (o) =>
			{
				characterHeadAccessoryPrefabs.Add(o);
			});
		}

		characterPropPrefabs.Clear();
		foreach (var reference in characterPropPrefabRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<GameObject>(reference, (o) =>
			{
				characterPropPrefabs.Add(o);
			});
		}

		yield return ResourceManager.Instance.LoadAsync<GameObject>(characterLeftEyePrefabRef, (o) =>
		{
			characterLeftEyePrefab = o;
		});

		yield return ResourceManager.Instance.LoadAsync<GameObject>(characterRightEyePrefabRef, (o) =>
		{
			characterRightEyePrefab = o;
		});

		yield return ResourceManager.Instance.LoadAsync<GameObject>(characterFacePrefabRef, (o) =>
		{
			characterFacePrefab = o;
		});

		characterLeftEyeMeshes.Clear();
		foreach (var reference in characterLeftEyeMeshRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<Mesh>(reference, (m) =>
			{
				characterLeftEyeMeshes.Add(m);
			});
		}

		characterRightEyeMeshes.Clear();
		foreach (var reference in characterRightEyeMeshRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<Mesh>(reference, (m) =>
			{
				characterRightEyeMeshes.Add(m);
			});
		}

		characterFaceMeshes.Clear();
		foreach (var reference in characterFaceMeshRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<Mesh>(reference, (m) =>
			{
				characterFaceMeshes.Add(m);
			});
		}

		characterEyeMaterials.Clear();
		foreach (var reference in characterEyeMaterialRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<Material>(reference, (m) =>
			{
				characterEyeMaterials.Add(m);
			});
		}
	}

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
