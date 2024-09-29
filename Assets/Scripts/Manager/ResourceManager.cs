using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.ResourceManagement.AsyncOperations;

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
}

public class ResourceManager : Singleton<ResourceManager>
{
	private Dictionary<string, Object> cachedObjectDictionary = new Dictionary<string, Object>();

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

	public IEnumerator LoadAsync<T>(AssetReference reference, System.Action<T> onCompleted = null) where T : UnityEngine.Object
	{
		yield return LoadAsync(reference.AssetGUID, onCompleted);
	}

	public IEnumerator LoadAsync<T>(string path, System.Action<T> onCompleted = null) where T : UnityEngine.Object
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
