using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class UICameraController : MonoBehaviour
{
	[SerializeField] private Camera uiCamera;
	private Transform cameraBoom;

	[SerializeField] private float rotateSpeed = 1.0f; // 회전 속도

	[SerializeField] private float zoomSpeed = 10f; // 줌 속도
	[SerializeField] private float minFOV = 0.1f; // 최소 FOV
	[SerializeField] private float maxFOV = 1f; // 최대 FOV

	private Vector3 initialRotationAngle = Vector3.zero;

	private void Update()
	{
		if (cameraBoom == null)
		{
			cameraBoom = uiCamera.transform.GetChild(0);
			initialRotationAngle = cameraBoom.transform.eulerAngles;
		}

		if (Input.touchCount == 2)
		{
			Touch touch1 = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);

			// 두 터치의 이전 위치
			Vector2 prevPos1 = touch1.position - touch1.deltaPosition;
			Vector2 prevPos2 = touch2.position - touch2.deltaPosition;

			// 현재 거리
			float currentDistance = Vector2.Distance(touch1.position, touch2.position);
			// 이전 거리
			float previousDistance = Vector2.Distance(prevPos1, prevPos2);

			// 거리 차이
			float distanceDelta = currentDistance - previousDistance;

			// FOV 조정
			uiCamera.fieldOfView -= distanceDelta * zoomSpeed * Time.deltaTime;
			uiCamera.fieldOfView = Mathf.Clamp(uiCamera.fieldOfView, minFOV, maxFOV);
		}
		else if (Input.touchCount == 1)
		{
			Touch touch1 = Input.GetTouch(0);
			Vector2 deltaPos = touch1.deltaPosition;

			Vector3 currentRot = Vector2.zero;
			currentRot.y -= deltaPos.x * rotateSpeed * Time.deltaTime;
			cameraBoom.Rotate(currentRot);

			currentRot = Vector2.zero;
			currentRot.x -= deltaPos.y * rotateSpeed * Time.deltaTime;
			cameraBoom.Rotate(currentRot);

		}
		else if (Input.GetMouseButton(0))
		{
			Vector2 deltaPos = Input.mousePositionDelta;

			Vector3 currentRot = Vector2.zero;
			currentRot.y -= deltaPos.x * rotateSpeed * Time.deltaTime;
			cameraBoom.Rotate(currentRot);

			currentRot = Vector2.zero;
			currentRot.x -= deltaPos.y * rotateSpeed * Time.deltaTime;
			cameraBoom.Rotate(currentRot);
		}
	}

	public void OnClickResetCamera()
	{
		cameraBoom.rotation = Quaternion.Euler(initialRotationAngle);
	}
}
