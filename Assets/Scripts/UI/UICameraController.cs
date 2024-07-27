using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class UICameraController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	[SerializeField] private Camera uiCamera;
	[SerializeField] private Transform cameraArm;
	private Transform target;

	[SerializeField] private float xMinRotationLimit = -80f; // 상하 회전 제한
	[SerializeField] private float xMaxRotationLimit = 10f; // 상하 회전 제한
	[SerializeField] private float rotateSpeed = 1.0f; // 회전 속도

	[SerializeField] private float zoomSpeed = 10f; // 줌 속도
	[SerializeField] private float minFOV = 0.1f; // 최소 FOV
	[SerializeField] private float maxFOV = 1f; // 최대 FOV

	private bool isDragging = false;
	private Vector3 cameraInitialRotationAngle = Vector3.zero;
	private Vector3 targetInitialRotationAngle = Vector3.zero;

	private void Start()
	{
		cameraInitialRotationAngle = cameraArm.transform.eulerAngles;
	}

	private void Update()
	{
		if (target == null)
		{
			target = cameraArm.transform.GetChild(0);
			targetInitialRotationAngle = target.eulerAngles;
		}

		if (isDragging == false)
			return;

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

		}
		else if (Input.GetMouseButton(0))
		{
			Vector2 deltaPos = Input.mousePositionDelta;

			// 현재 카메라 암의 회전값을 사용해 상하 회전 제한을 적용
			var xRotation = cameraArm.eulerAngles.x > 180.0f ? cameraArm.eulerAngles.x - 360.0f : cameraArm.eulerAngles.x;
			xRotation += deltaPos.y * rotateSpeed * Time.deltaTime;
			xRotation = Mathf.Clamp(xRotation, xMinRotationLimit, xMaxRotationLimit);

			// 카메라 암의 상하 회전 적용
			cameraArm.rotation = Quaternion.Euler(xRotation, 0f, 0f);

			// 캐릭터의 좌우 회전 적용
			target.Rotate(Vector3.down * deltaPos.x * rotateSpeed * Time.deltaTime);
		}
	}

	public void OnClickResetCamera()
	{
		cameraArm.rotation = Quaternion.Euler(cameraInitialRotationAngle);
		target.rotation = Quaternion.Euler(targetInitialRotationAngle);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		isDragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
	}

	// 이게 없으니까 안 들어오네...
	public void OnDrag(PointerEventData eventData)
	{
		
	}
}
