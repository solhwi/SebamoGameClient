using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : ObjectView
{
	private Transform originCharacterTransform = null;
	private CharacterDataSetter characterDataSetter = null;
	private CharacterAnimationController characterAnimationController = null;

	private bool currentFlipX;
	private bool currentFlipY;

	protected override void Awake()
	{
		base.Awake();

		if (spriteView != null)
		{
			spriteView.sortingOrder = (int)LayerConfig.Character;
		}
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
	}

	public void FlipX(bool flipX)
	{
		currentFlipX = flipX;
		meshView.FlipX(flipX);
	}

	public void FlipY(bool flipY)
	{
		float yRot = flipY ? 180 : 0;

		originCharacterTransform.localEulerAngles = new Vector3(0, yRot, 0);
		currentFlipY = flipY;
	}

	public void Replay()
	{
		if (characterAnimationController != null)
		{
			characterAnimationController.Replay();
		}

		FlipX(currentFlipX);
		FlipY(currentFlipY);
	}

	public void DoIdle(float crossFadeTime = 0.0f)
	{
		if (characterAnimationController != null)
		{
			characterAnimationController.DoIdle(crossFadeTime);
		}
	}

	public void DoRun()
	{
		if (characterAnimationController != null)
		{
			characterAnimationController.DoRun();
		}
	}
}
