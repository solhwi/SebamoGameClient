using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public static class ResourceHelper
{
	public static T Get<T>(this AssetReferenceT<T> assetReference) where T : Object
	{
		return ResourceManager.Instance.Load<T>(assetReference);
	}
}

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

	private void OnDestroy()
	{
		if (instance == this)
		{
			OnReleaseInstance();
		}
	}

	protected virtual void OnAwakeInstance()
	{

	}

	protected virtual void OnReleaseInstance()
	{

	}
}



public class ResourceManager : Singleton<ResourceManager>
{
	[SerializeField] private List<AssetReferenceT<DataContainer>> dataContainers = new List<AssetReferenceT<DataContainer>>();

	[SerializeField] private List<AssetLabelReference> labelRefs = new List<AssetLabelReference>();

	private Dictionary<string, AsyncOperationHandle> cachedObjectHandleDictionary = new Dictionary<string, AsyncOperationHandle>();

	/// <summary>
	/// 메가바이트 단위로 돌려준다.
	/// </summary>
	private IEnumerator GetDownLoadSize(AssetLabelReference labelRef, System.Action<float> onCompleted)
	{
		var handle = Addressables.GetDownloadSizeAsync(labelRef);

		while (handle.IsDone == false)
		{
			yield return null;
		}

		onCompleted?.Invoke(handle.Result / 1024f / 1024f);
	}

	private IEnumerator DownLoadAssets(AssetLabelReference labelRef, System.Action<float> onProgress, System.Action onCompleted = null)
	{
		var handle = Addressables.DownloadDependenciesAsync(labelRef);

		while (handle.IsDone == false)
		{
			yield return null;
			onProgress?.Invoke(handle.PercentComplete);
		}

		onCompleted?.Invoke();
	}

	public IEnumerator DownLoadAssets(System.Action<float> onProgress = null)
	{
		yield return DownLoadAssets(labelRefs.FirstOrDefault(), onProgress);
	}

	public IEnumerator GetDownLoadSize(System.Action<float> onCompleted = null)
	{
		yield return GetDownLoadSize(labelRefs.FirstOrDefault(), onCompleted);
	}

	public IEnumerator PreLoadAssets()
	{
		foreach (var container in dataContainers)
		{
			DataContainer result = null;

			yield return LoadAsync<DataContainer>(container, (c) => 
			{ 
				result = c; 
			});

			yield return result.Preload();
		}
	}

	public T Load<T>(AssetReference reference) where T : UnityEngine.Object
	{
		return Load<T>(reference.AssetGUID);
	}

	public T Load<T>(string path) where T : UnityEngine.Object
	{
		if (cachedObjectHandleDictionary.ContainsKey(path) == false)
		{
			Debug.LogError($"{path}에 프리로드가 필요합니다.");
			return null;
		}

		return GetResult<T>(cachedObjectHandleDictionary[path].Result);
	}

	public IEnumerator InstantiateAsync<T>(AssetReference reference, Transform parent, bool isActive = false, System.Action<T> onCompleted = null) where T : UnityEngine.Object
	{
		yield return InstantiateAsync(reference.AssetGUID, parent, isActive,onCompleted);
	}

	private IEnumerator InstantiateAsync<T>(string path, Transform parent, bool isActive, System.Action<T> onCompleted) where T : UnityEngine.Object
	{
		T obj = null;

		yield return LoadAsync<T>(path, (res) =>
		{
			if (res == null)
				return;

			obj = Instantiate<T>(res, parent);
			obj.GameObject().SetActive(isActive);
		});

		onCompleted?.Invoke(obj);
	}

	public IEnumerator LoadAsync<T>(AssetReference reference, System.Action<T> onCompleted = null) where T : UnityEngine.Object
	{
		yield return LoadAsync(reference.AssetGUID, onCompleted);
	}

	public IEnumerator LoadAsync<T>(string path, System.Action<T> onCompleted = null) where T : UnityEngine.Object
	{
		if (cachedObjectHandleDictionary.TryGetValue(path, out var handle))
		{
			var r = GetResult<T>(handle.Result);
			onCompleted?.Invoke(r);
			yield break;
		}

		var asyncOperation = LoadAssetAsync<T>(path);
		while (asyncOperation.IsDone == false)
		{
			yield return null;
		}

		cachedObjectHandleDictionary[path] = asyncOperation;

		var result = GetResult<T>(asyncOperation.Result);
		onCompleted?.Invoke(result);
	}

	protected override void OnReleaseInstance()
	{
		ReleaseAllCache();
	}

	private AsyncOperationHandle LoadAssetAsync<T>(string path)
	{
		if (typeof(T).IsSubclassOf(typeof(Component)))
		{
			return Addressables.LoadAssetAsync<GameObject>(path);
		}
		else
		{
			return Addressables.LoadAssetAsync<T>(path);
		}
	}

	private T GetResult<T>(object result) where T : UnityEngine.Object
	{
		if (result is T r)
		{
			return r;
		}
		else if (result is GameObject g)
		{
			return g.GetComponent<T>();
		}

		return null;
	}

	public void ReleaseAllCache()
	{
		foreach (var handle in cachedObjectHandleDictionary.Values)
		{
			Addressables.Release(handle);
		}

		cachedObjectHandleDictionary.Clear();
	}

	public Coroutine TryInstantiateAsync<T>(AssetReference reference, Transform parent, bool isActive = false, System.Action<T> onCompleted = null) where T : UnityEngine.Object
	{
		return TryInstantiateAsync(reference.AssetGUID, parent, isActive, onCompleted);
	}

	public Coroutine TryInstantiateAsync<T>(string path, Transform parent, bool isActive = false, System.Action<T> onCompleted = null) where T : UnityEngine.Object
	{
		return StartCoroutine(InstantiateAsync(path, parent, isActive, onCompleted));
	}

	public T Instantiate<T>(AssetReference reference, Transform parent) where T : Object
	{
		return Instantiate<T>(reference.AssetGUID, parent);
	}

	public T Instantiate<T>(string path, Transform parent) where T : Object
	{
		if (cachedObjectHandleDictionary.ContainsKey(path) == false)
		{
			Debug.LogError($"{path}에 프리 로드가 필요합니다.");
			return null;
		}

		if (cachedObjectHandleDictionary[path] is T prefab == false)
			return null;

		return Object.Instantiate<T>(prefab, parent);
	}
}
