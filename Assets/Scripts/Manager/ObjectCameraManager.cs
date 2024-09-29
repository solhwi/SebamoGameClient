using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectCameraManager : Singleton<ObjectCameraManager>
{
	[SerializeField] private AssetReferenceGameObject objectCameraRef;
	[SerializeField] private int preloadCount = 0;

	private Stack<Camera> preloadStack = new Stack<Camera>();

	public IEnumerator PreLoadObjectCamera()
	{
		preloadStack.Clear();

		for(int i = 0; i < preloadCount; i++)
		{
			yield return ResourceManager.Instance.InstantiateAsync<Camera>(objectCameraRef, transform, (newCamera) =>
			{
				if (newCamera == null)
					return;

				newCamera.gameObject.SetActive(false);
				preloadStack.Push(newCamera);
			});
		}
	}

	public Camera MakeCamera(bool isOrtho, float FOV)
	{
		if (preloadStack.TryPop(out Camera newCamera) == false)
		{
			newCamera = ResourceManager.Instance.Instantiate<Camera>(objectCameraRef, transform);
		}

		newCamera.gameObject.SetActive(true);
		newCamera.orthographic = isOrtho;

		if (isOrtho)
		{
			newCamera.orthographicSize = FOV;
		}
		else
		{
			newCamera.fieldOfView = FOV;
		}

		int offset = preloadStack.Count * 100;
		newCamera.transform.localPosition = new Vector3(offset, offset, 0);

		return newCamera;
	}
}
