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

	protected virtual void Awake()
	{

	}
}

public class ResourceManager : Singleton<ResourceManager>
{
	[SerializeField] private RecyclingObject fieldItemPrefab;

	private Dictionary<RecyclingType, Stack<RecyclingObject>> objectPool = new Dictionary<RecyclingType, Stack<RecyclingObject>>()
	{
		{ RecyclingType.fieldItem, new Stack<RecyclingObject>() },
	};
	
	private Dictionary<string, Object> cachedObjectDictionary = new Dictionary<string, Object>();

	private void Start()
	{
		for(int i = 0; i < 10; i++)
		{
			var obj = Instantiate(fieldItemPrefab);

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
		RecyclingObject obj = null;
		if (objectPool[RecyclingType.fieldItem].TryPop(out obj) == false)
		{
			obj = Instantiate(fieldItemPrefab);

#if UNITY_EDITOR
			obj.name = $"fieldItemPrefab ({objectPool[RecyclingType.fieldItem].Count}) - Cached";
#endif
		}

		string path = rawData.GetAssetPathWithoutResources();
		Object res = Load<Sprite>(path);
		if (res == null)
		{
			res = Load<RuntimeAnimatorController>(path);
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
