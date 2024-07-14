using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : ObjectView
{
	private Transform originCharacterTransform = null;
	private CharacterAnimationController characterAnimationController = null;

	protected override void Start()
	{
		spawnLocalPos = new Vector3(0.05f, -0.5f, 10.0f);
		spawnLocalRot = new Vector3(15, 150, -15);

		base.Start();

		var setter = originObj.GetComponent<CharacterDataSetter>();
		if (setter != null)
		{
			setter.DoFullSetting();
		}

		characterAnimationController = originObj.GetComponentInChildren<CharacterAnimationController>();

		// 애니메이터가 달려있는 곳이 제어할 위치
		var characterOrigin = originObj.GetComponentInChildren<Animator>();
		if (characterOrigin != null)
		{
			originCharacterTransform = characterOrigin.transform;
		}

		spriteView.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
	}

	public void FlipX(bool flipX)
	{
		spriteView.flipX = flipX;
	}

	public void FlipY(bool flipY)
	{
		float yRot = flipY ? 180 : 0;

		originCharacterTransform.localEulerAngles = new Vector3(0, yRot, 0);
	}

	public void DoIdle()
	{
		characterAnimationController.DoIdle();
	}

	public void DoRun()
	{
		characterAnimationController.DoRun();
	}
}
