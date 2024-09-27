using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.ResourceManagement.AsyncOperations;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	[SerializeField] protected bool IsDontDestroyOnLoad = false;

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

	private void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
		}

		if (IsDontDestroyOnLoad)
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}
			else
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		OnAwakeInstance();
	}

	protected virtual void OnAwakeInstance()
	{

	}

	public virtual IEnumerator OnPrepareInstance()
	{
		yield return null;
	}
}

public class ResourceManager : Singleton<ResourceManager>
{
	[SerializeField] private ItemTable itemTable;
	[SerializeField] private AssetReferenceT<RecyclingObject> fieldItemPrefabRef;
	[SerializeField] private AssetReferenceGameObject characterRef;
	[SerializeField] private AssetReferenceGameObject myCharacterRef;

	private Dictionary<RecyclingType, Stack<RecyclingObject>> objectPool = new Dictionary<RecyclingType, Stack<RecyclingObject>>()
	{
		{ RecyclingType.fieldItem, new Stack<RecyclingObject>() },
	};
	
	private Dictionary<string, UnityEngine.Object> cachedObjectDictionary = new Dictionary<string, UnityEngine.Object>();

#if UNITY_EDITOR
	[MenuItem("WebGL/Enable Embedded Resources")]
	public static void EnableEmbeddedResources()
	{
		PlayerSettings.WebGL.useEmbeddedResources = true;
		PlayerSettings.WebGL.emscriptenArgs = "--memoryprofiler --profiling-funcs";
	}
#endif

	public override IEnumerator OnPrepareInstance()
	{
		yield return PreLoadItemData();
		yield return PreLoadFieldItemObject();
		yield return PreLoadCharacter();
	}

	private IEnumerator PreLoadFieldItemObject()
	{
		for (int i = 0; i < 10; i++)
		{
			yield return InstantiateAsync<RecyclingObject>(fieldItemPrefabRef, transform, (obj) =>
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

	private IEnumerator PreLoadItemData()
	{
		foreach (string path in itemTable.GetPreLoadTableDataPath())
		{
			string extension = Path.GetExtension(path);
			if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
			{
				yield return LoadAsync<Sprite>(path);
			}
			else
			{
				yield return LoadAsync<Object>(path);
			}
		}
	}

	private IEnumerator PreLoadCharacter()
	{
		yield return LoadAsync<MyCharacterComponent>(myCharacterRef);
		yield return LoadAsync<CharacterComponent>(characterRef);
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
			return Instantiate<GameObject>(path, transform);
		}

		UnityEngine.Object res = Load<Sprite>(path);
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

	public T Load<T>(string path) where T : UnityEngine.Object
	{
		if (cachedObjectDictionary.ContainsKey(path) == false)
		{
			Debug.LogError($"{path}에 프리로드가 필요합니다.");
			return null;
		}

		return cachedObjectDictionary[path] as T;
	}

	public IEnumerator InstantiateAsync<T>(AssetReference reference, Transform parent, System.Action<T> onCompleted) where T : UnityEngine.Object
	{
		yield return InstantiateAsync(reference.AssetGUID, parent, onCompleted);
	}


	private IEnumerator InstantiateAsync<T>(string path, Transform parent, System.Action<T> onCompleted) where T : UnityEngine.Object
	{
		T obj = null;

		yield return LoadAsync<T>(path, (res) =>
		{
			if (res == null)
				return;

			obj = Instantiate<T>(res, parent);
		});

		onCompleted?.Invoke(obj);
	}

	private IEnumerator LoadAsync<T>(AssetReference reference, System.Action<T> onCompleted = null) where T : UnityEngine.Object
	{
		yield return LoadAsync(reference.AssetGUID, onCompleted);
	}

	private IEnumerator LoadAsync<T>(string path, System.Action<T> onCompleted = null) where T : UnityEngine.Object
	{
		if (cachedObjectDictionary.TryGetValue(path, out var value))
		{
			onCompleted?.Invoke(value as T);
			yield break;
		}

		T result = null;

		if (typeof(T).IsSubclassOf(typeof(Component)))
		{
			var asyncOperation = Addressables.LoadAssetAsync<GameObject>(path);
			while (asyncOperation.IsDone == false)
			{
				yield return null;
			}

			var g = asyncOperation.Result;
			if (g == null)
				yield break;

			result = g.GetComponent<T>();
			Addressables.Release(asyncOperation);
		}
		else
		{
			var asyncOperation = Addressables.LoadAssetAsync<T>(path);
			while (asyncOperation.IsDone == false)
			{
				yield return null;
			}

			result = asyncOperation.Result;
			Addressables.Release(asyncOperation);
		}

		onCompleted?.Invoke(result);
		cachedObjectDictionary[path] = result;
	}

	public Coroutine TryInstantiateAsync<T>(AssetReference reference, Transform parent, System.Action<T> onCompleted) where T : UnityEngine.Object
	{
		return TryInstantiateAsync(reference.AssetGUID, parent, onCompleted);
	}

	private Coroutine TryInstantiateAsync<T>(string path, Transform parent, System.Action<T> onCompleted) where T : UnityEngine.Object
	{
		return StartCoroutine(InstantiateAsync(path, parent, onCompleted));
	}

	public T Instantiate<T>(AssetReference reference, Transform parent) where T : Object
	{
		return Instantiate<T>(reference.AssetGUID, parent);
	}

	public T Instantiate<T>(string path, Transform parent) where T : Object
	{
		if (cachedObjectDictionary.ContainsKey(path) == false)
		{
			Debug.LogError($"{path}에 프리 로드가 필요합니다.");
			return null;
		}

		if (cachedObjectDictionary[path] is T prefab == false)
			return null;

		return Object.Instantiate<T>(prefab, parent);
	}
}
