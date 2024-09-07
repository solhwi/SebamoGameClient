using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCameraManager : Singleton<ObjectCameraManager>
{
	[SerializeField] private Camera objectCameraPrefab;

	private List<Camera> objectCameras = new List<Camera>();

	public Camera MakeCamera(bool isOrtho, float FOV)
	{
		var newCamera = Instantiate(objectCameraPrefab, transform);
		if (newCamera == null)
			return null;

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

		objectCameras.Add(newCamera);
		return newCamera;
	}
}
