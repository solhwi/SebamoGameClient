using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : ObjectView
{
	private Transform originCharacterTransform = null;
	private CharacterDataSetter characterDataSetter = null;
	private CharacterAnimationController characterAnimationController = null;

	protected override void Awake()
	{
		base.Awake();

		spriteView.sortingOrder = (int)LayerConfig.Character;
	}

	protected override void Start()
	{
		base.Start();

		characterDataSetter = originObj.GetComponent<CharacterDataSetter>();
		if (characterDataSetter != null)
		{
			characterDataSetter.DoFullSetting();
		}

		characterAnimationController = originObj.GetComponentInChildren<CharacterAnimationController>();

		// 애니메이터가 달려있는 곳이 제어할 위치
		var characterOrigin = originObj.GetComponentInChildren<Animator>();
		if (characterOrigin != null)
		{
			originCharacterTransform = characterOrigin.transform;
		}

		characterAnimationController.DoIdle();
	}

	public void RefreshCharacter()
	{
		if (characterDataSetter != null)
		{
			characterDataSetter.DoFullSetting();
		}

		// 애니메이터가 달려있는 곳이 제어할 위치
		var characterOrigin = originObj.GetComponentInChildren<Animator>();
		if (characterOrigin != null)
		{
			originCharacterTransform = characterOrigin.transform;
		}

		characterAnimationController.DoIdle(0.3f);
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
