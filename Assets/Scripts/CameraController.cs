using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : Singleton<CameraController>
{
	[SerializeField] private CinemachineVirtualCamera virtualCamera;

	[SerializeField] private Transform targetTransform;
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

		ResetTarget();
	}

	private void Start()
	{
		SetDamping(dampingVec);
		SetFollow(true);
	}

	private void LateUpdate()
	{
		if (isFollowingTarget == false)
		{
			if (Input.GetMouseButton(0))
			{
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
		currentTargetTransform = targetTransform;
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
