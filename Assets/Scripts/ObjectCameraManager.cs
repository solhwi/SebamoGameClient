using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectCameraManager : Singleton<ObjectCameraManager>
{
	[SerializeField] private AssetReferenceGameObject objectCameraRef;
	[SerializeField] private int preloadCount = 0;

	private Stack<Camera> objectCameras = new Stack<Camera>();

	public override IEnumerator OnPrepareInstance()
	{
		objectCameras.Clear();

		for(int i = 0; i < preloadCount; i++)
		{
			yield return ResourceManager.Instance.InstantiateAsync<Camera>(objectCameraRef, transform, (newCamera) =>
			{
				if (newCamera == null)
					return;

				newCamera.gameObject.SetActive(false);
				objectCameras.Push(newCamera);
			});
		}
	}

	public Camera MakeCamera(bool isOrtho, float FOV)
	{
		if (objectCameras.TryPop(out Camera newCamera))
		{
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

			int offset = objectCameras.Count * 100;
			newCamera.transform.localPosition = new Vector3(offset, offset, 0);

			return newCamera;
		}

		return null;
	}
}
