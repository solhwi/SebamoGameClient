using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera virtualCamera;

	[SerializeField] private Transform targetTransform;
	[SerializeField] private float orthoSize = 5.0f;
	[SerializeField] private float cameraSpeed = 0.5f;
	[SerializeField] private Vector2 dampingVec = new Vector2(0.1f, 0.1f);

	private CinemachineTransposer transposer;

	private void Awake()
	{
		transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
	}

	private void Start()
	{
		SetDamping(dampingVec);
		SetFollow(true);
		SetOrthoSize(orthoSize);
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

	// 확대, 축소 시 사용
	public void SetOrthoSize(float size)
	{
		virtualCamera.m_Lens.OrthographicSize = size;
	}

	public void SetFollow(bool isFollow)
	{
		if (isFollow)
		{
			virtualCamera.Follow = targetTransform;
			virtualCamera.LookAt = targetTransform;
		}
		else
		{
			virtualCamera.Follow = null;
			virtualCamera.LookAt = null;
		}
	}

	public void Move(Vector3 deltaPos)
	{
		virtualCamera.ForceCameraPosition(virtualCamera.transform.position + deltaPos * cameraSpeed, Quaternion.identity);
	}

}
