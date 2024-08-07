using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindAnyObjectByType<T>();
			}

			return instance;
		}
	}

	private static T instance;
}

public class ResourceManager : Singleton<ResourceManager>
{
	[SerializeField] private RecyclingObject dropItemPrefab;

	private Dictionary<RecyclingType, Stack<RecyclingObject>> objectPool = new Dictionary<RecyclingType, Stack<RecyclingObject>>()
	{
		{ RecyclingType.DropItem, new Stack<RecyclingObject>() },
	};
	
	private Dictionary<string, Object> cachedObjectDictionary = new Dictionary<string, Object>();

	private void Start()
	{
		for(int i = 0; i < 10; i++)
		{
			var obj = Instantiate(dropItemPrefab);

#if UNITY_EDITOR
			obj.name = $"DropItemPrefab ({i}) - Cached";
#endif
			obj.transform.position = new Vector3(-1000, -1000, 0);
			obj.gameObject.SetActive(false);

			var renderer = obj.GetComponentInChildren<Renderer>();
			if (renderer != null)
			{
				renderer.sortingOrder = (int)LayerConfig.Item;
			}

			objectPool[RecyclingType.DropItem].Push(obj);
		}
	}

	// 실질적인 리소스 관리를 위해 DropItemFactory 쪽에서 일로 이관함
	public GameObject GetDropItemObject(ItemTable.DropItemData rawData, WorldTileData worldTileData)
	{
		RecyclingObject obj = null;
		if (objectPool[RecyclingType.DropItem].TryPop(out obj) == false)
		{
			obj = Instantiate(dropItemPrefab);

#if UNITY_EDITOR
			obj.name = $"DropItemPrefab ({objectPool[RecyclingType.DropItem].Count}) - Cached";
#endif

			objectPool[RecyclingType.DropItem].Push(obj);
		}

		obj.name = $"{rawData.key} ({worldTileData.index})";
		obj.transform.position = new Vector3(worldTileData.tileWorldPosition.x, worldTileData.tileWorldPosition.y, 0);
		obj.gameObject.SetActive(true);

		var renderer = obj.GetComponentInChildren<SpriteRenderer>();
		if (renderer != null)
		{
			renderer.sortingOrder = (int)LayerConfig.Item;
		}

		string path = rawData.GetAssetPathWithoutResources();
		Object res = Load<Sprite>(path);
		if (res == null)
		{
			res = Load<RuntimeAnimatorController>(path);
		}

		if (renderer != null && res is Sprite sprite)
		{
			renderer.sprite = sprite;
		}
		else if (res is RuntimeAnimatorController anim)
		{
			var animator = obj.gameObject.GetComponent<Animator>();
			animator.runtimeAnimatorController = anim;
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
			recyclingObj.transform.position = new Vector3(-1000, -1000, 0);
			recyclingObj.gameObject.SetActive(false);

			var renderer = obj.GetComponentInChildren<SpriteRenderer>();
			if (renderer != null)
			{
				renderer.sortingOrder = 0;
			}

			objectPool[recyclingObj.type].Push(recyclingObj);	
		}
		else
		{
			MonoBehaviour.Destroy(obj);
		}
	}

	public T Load<T>(string path) where T : Object
	{
		if (cachedObjectDictionary.ContainsKey(path) == false)
		{
			return Resources.Load<T>(path);
		}

		return cachedObjectDictionary[path] as T;
	}

	public void Unload<T>(T obj) where T : Object
	{
		// 이 언로드는 리소스가 더이상 사용되지 않을 때를 체크하여 사용하도록 한다.
		Resources.UnloadAsset(obj);
	}
}
