using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectManager : Singleton<ObjectManager>
{
	[SerializeField] private AssetReferenceGameObject fieldItemPrefabRef;

	private Dictionary<RecyclingType, Stack<RecyclingObject>> objectPool = new Dictionary<RecyclingType, Stack<RecyclingObject>>()
	{
		{ RecyclingType.fieldItem, new Stack<RecyclingObject>() },
	};

	public IEnumerator PreInstantiateFieldItemObject()
	{
		for (int i = 0; i < 10; i++)
		{
			yield return ResourceManager.Instance.InstantiateAsync<RecyclingObject>(fieldItemPrefabRef, transform, (obj) =>
			{
#if UNITY_EDITOR
				obj.name = $"fieldItemPrefab ({i}) - Cached";
#endif
				obj.transform.position = new Vector3(-1000, -1000, 0);
				obj.gameObject.SetActive(false);

				var renderer = obj.GetComponentInChildren<Renderer>();
				if (renderer != null)
				{
					renderer.sortingOrder = (int)LayerConfig.Item;
				}

				objectPool[RecyclingType.fieldItem].Push(obj);
			});
		}
	}

	// 실질적인 리소스 관리를 위해 fieldItemFactory 쪽에서 일로 이관함
	public GameObject GetFieldItemObject(ItemTable.FieldItemData rawData, WorldTileData worldTileData)
	{
		var obj = GetFieldItemObject(rawData);
		if (obj != null)
		{
			obj.name = $"{rawData.key} ({worldTileData.index})";
			obj.transform.position = new Vector3(worldTileData.tileWorldPosition.x, worldTileData.tileWorldPosition.y, 0);
			obj.gameObject.SetActive(true);
		}

		return obj;
	}

	public GameObject GetFieldItemObject(ItemTable.FieldItemData rawData)
	{
		string path = rawData.GetAssetPathWithoutResources();

		RecyclingObject obj = null;
		if (objectPool[RecyclingType.fieldItem].TryPop(out obj) == false)
		{
			Debug.LogError($"{RecyclingType.fieldItem}에 풀링이 더욱 필요합니다.");
			return ResourceManager.Instance.Instantiate<GameObject>(path, transform);
		}

		UnityEngine.Object res = ResourceManager.Instance.Load<Sprite>(path);
		if (res == null)
		{
			res = ResourceManager.Instance.Load<RuntimeAnimatorController>(path);
		}

		var renderer = obj.GetComponentInChildren<SpriteRenderer>();
		if (renderer != null)
		{
			renderer.sortingOrder = (int)LayerConfig.Item;
		}

		var animator = obj.gameObject.GetComponent<Animator>();
		if (animator != null)
		{
			if (renderer != null && res is Sprite sprite)
			{
				animator.runtimeAnimatorController = null;
				renderer.sprite = sprite;
			}
			else if (res is RuntimeAnimatorController anim)
			{
				animator.runtimeAnimatorController = anim;
			}
		}

		return obj.gameObject;
	}

	public void Destroy(GameObject obj)
	{
		if (obj == null)
		{
			Debug.LogError("삭제를 시도한 오브젝트가 이미 null입니다.");
			return;
		}

		RecyclingObject recyclingObj = obj.GetComponent<RecyclingObject>();

		if (recyclingObj != null)
		{
			recyclingObj.transform.SetParent(transform);
			recyclingObj.transform.position = new Vector3(-1000, -1000, 0);
			recyclingObj.gameObject.SetActive(false);

			var renderer = obj.GetComponentInChildren<SpriteRenderer>();
			if (renderer != null)
			{
				renderer.color = Color.white;
				renderer.sortingOrder = 0;
			}

			objectPool[recyclingObj.type].Push(recyclingObj);
		}
		else
		{
			MonoBehaviour.Destroy(obj);
		}
	}

}
