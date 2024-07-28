using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICharacterView : CharacterView, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	[SerializeField] private float xMinRotationLimit = 0; // 상하 회전 제한
	[SerializeField] private float xMaxRotationLimit = 0; // 상하 회전 제한
	[SerializeField] private float rotateSpeed = 1.0f; // 회전 속도

	[SerializeField] private float zoomSpeed = 10f; // 줌 속도
	[SerializeField] private float minFOV = 5f; // 최소 FOV
	[SerializeField] private float maxFOV = 15f; // 최대 FOV

	private bool isDragging = false;
	private float defaultFOV = 10.0f;

	protected override void Awake()
	{
		base.Awake();

		defaultFOV = cam.fieldOfView;
	}

	protected override void Update()
	{
		base.Update();

		if (isDragging)
		{
#if UNITY_EDITOR
			if (Input.GetMouseButton(0))
			{
				Vector2 deltaPos = Input.mousePositionDelta;

				// 현재 카메라 암의 회전값을 사용해 상하 회전 제한을 적용
				var xRotation = cameraArm.eulerAngles.x > 180.0f ? cameraArm.eulerAngles.x - 360.0f : cameraArm.eulerAngles.x;
				xRotation += deltaPos.y * rotateSpeed * Time.deltaTime;
				xRotation = Mathf.Clamp(xRotation, xMinRotationLimit, xMaxRotationLimit);

				// 카메라 암의 상하 회전 적용
				cameraArm.rotation = Quaternion.Euler(xRotation, 0f, 0f);

				// 캐릭터의 좌우 회전 적용
				originObj.transform.Rotate(Vector3.down * deltaPos.x * rotateSpeed * Time.deltaTime);
			}
#else
			if (Input.touchCount == 0)
			{
				Touch touch1 = Input.GetTouch(0);
				Vector2 deltaPos = touch1.deltaPosition;

				// 현재 카메라 암의 회전값을 사용해 상하 회전 제한을 적용
				var xRotation = cameraArm.eulerAngles.x > 180.0f ? cameraArm.eulerAngles.x - 360.0f : cameraArm.eulerAngles.x;
				xRotation += deltaPos.y * rotateSpeed * Time.deltaTime;
				xRotation = Mathf.Clamp(xRotation, xMinRotationLimit, xMaxRotationLimit);

				// 카메라 암의 상하 회전 적용
				cameraArm.rotation = Quaternion.Euler(xRotation, 0f, 0f);

				// 캐릭터의 좌우 회전 적용
				originObj.transform.Rotate(Vector3.down * deltaPos.x * rotateSpeed * Time.deltaTime);
			}
#endif
		}

#if UNITY_EDITOR
		float mouseScrollWheelValue = Input.GetAxis("Mouse ScrollWheel");
		if (MathFloat.Equals(mouseScrollWheelValue, 0.0f) == false)
		{
			UpdateFOV(mouseScrollWheelValue);
		}
#else
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
			UpdateFOV(distanceDelta);
		}
#endif
		
	}

	private void UpdateFOV(float delta)
	{
		// FOV 조정
		cam.fieldOfView -= delta * zoomSpeed;
		cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
	}

	public void OnClickResetCamera()
	{
		cameraArm.rotation = Quaternion.Euler(initialCameraArmRot);
		originObj.transform.localRotation = Quaternion.Euler(spawnLocalRot);
		cam.fieldOfView = defaultFOV;
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
