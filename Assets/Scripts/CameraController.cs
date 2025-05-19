using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : Singleton<CameraController>
{
	[SerializeField] private CinemachineVirtualCamera virtualCamera;

	[SerializeField] private float cameraMoveSpeed = 0.5f;
	[SerializeField] private float cameraZoomSpeed = 5f;
	[SerializeField] private Vector2 dampingVec = new Vector2(0.1f, 0.1f);
	[SerializeField] private float minFOV = 40f; // 최소 FOV
	[SerializeField] private float maxFOV = 60f; // 최대 FOV

	private Transform currentTargetTransform = null;

	private CinemachineTransposer transposer;

	private float initialFOV = 50f;

	private bool isFollowingTarget;
	private bool isZooming = false;

	protected override void OnAwakeInstance()
	{
		base.OnAwakeInstance();

		transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
		initialFOV = virtualCamera.m_Lens.Orthographic ? virtualCamera.m_Lens.OrthographicSize : virtualCamera.m_Lens.FieldOfView;
	}

	private void Start()
	{
		// 세로 해상도 기준 (1080x1920 → 9:16)
		float targetAspect = 9f / 16f;

		// 현재 디바이스 화면 비율
		float windowAspect = (float)Screen.width / (float)Screen.height;

		// 스케일 비율
		float scaleWidth = windowAspect / targetAspect;

		Camera cam = Camera.main;

		if (scaleWidth < 1.0f)
		{
			// 좌우에 블랙 레터박스 (pillarbox)
			Rect rect = cam.rect;

			rect.width = scaleWidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scaleWidth) / 2.0f;
			rect.y = 0;

			cam.rect = rect;
		}
		else
		{
			// 위아래에 블랙 레터박스 (letterbox)
			float scaleHeight = 1.0f / scaleWidth;

			Rect rect = cam.rect;

			rect.width = 1.0f;
			rect.height = scaleHeight;
			rect.x = 0;
			rect.y = (1.0f - scaleHeight) / 2.0f;

			cam.rect = rect;
		}
	}

	private void LateUpdate()
	{
		if (isFollowingTarget == false)
		{
			if (Input.GetMouseButton(0))
			{
				// 이 부분 mousePositionDelta가 갑자기 확 튀는 경우가 있음
				Vector2 deltaPos = Input.mousePositionDelta;
				Move(Time.deltaTime * -deltaPos);
			}
		}

		if (isZooming)
		{
			float mouseScrollWheelValue = Input.GetAxis("Mouse ScrollWheel");
			if (CommonFunc.Equals(mouseScrollWheelValue, 0.0f) == false)
			{
				UpdateFOV(mouseScrollWheelValue);
			}
		}
	}

	public void ResetTarget()
	{
		var myCharacter = BoardGameManager.Instance.GetMyPlayerCharacter();
		if (myCharacter != null)
		{
			currentTargetTransform = myCharacter.transform;
		}

		SetDamping(dampingVec);
		SetFollow(true);
	}

	public void ChangeTarget(Transform targetTransform)
	{
		currentTargetTransform = targetTransform;
	}

	private void UpdateFOV(float delta)
	{
		if (virtualCamera.m_Lens.Orthographic)
		{
			virtualCamera.m_Lens.OrthographicSize -= delta * cameraZoomSpeed;
			virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize, minFOV, maxFOV);
		}
		else
		{
			virtualCamera.m_Lens.FieldOfView -= delta * cameraZoomSpeed;
			virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(virtualCamera.m_Lens.FieldOfView, minFOV, maxFOV);
		}
	}

	public void ResetZoom()
	{
		if (virtualCamera.m_Lens.Orthographic)
		{
			virtualCamera.m_Lens.OrthographicSize = initialFOV;
		}
		else
		{
			virtualCamera.m_Lens.FieldOfView = initialFOV;
		}
	}

	private void SetDamping(Vector2 dampingVec)
	{
		transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
		transposer.m_XDamping = dampingVec.x;
		transposer.m_YDamping = dampingVec.y;
		transposer.m_ZDamping = 0.0f;
		transposer.m_YawDamping = 0.0f;
		transposer.m_RollDamping = 0.0f;
		transposer.m_PitchDamping = 0.0f;
	}

	public void SetZoom(bool isZoom)
	{
		isZooming = isZoom;
	}

	public void SetFollow(bool isFollow)
	{
		isFollowingTarget = isFollow;

		if (isFollow)
		{
			virtualCamera.Follow = currentTargetTransform;
			virtualCamera.LookAt = currentTargetTransform;
		}
		else
		{
			virtualCamera.Follow = null;
			virtualCamera.LookAt = null;
		}
	}

	private void Move(Vector3 deltaPos)
	{
		virtualCamera.ForceCameraPosition(virtualCamera.transform.position + deltaPos * cameraMoveSpeed, Quaternion.identity);
	}

}
