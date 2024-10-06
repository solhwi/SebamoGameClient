using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : ObjectView
{
	[SerializeField] protected CharacterHeadOnUI characterHeadOnUI = null;

	private Transform originCharacterTransform = null;
	private CharacterDataSetter characterDataSetter = null;
	private CharacterAnimationController characterAnimationController = null;

	private string playerGroup = string.Empty;
	private string playerName = string.Empty;

	private bool currentFlipX;
	private bool currentFlipY;

	public override void Initialize()
	{
		base.Initialize();

		if (spriteView != null)
		{
			spriteView.sortingOrder = (int)LayerConfig.Character;
		}

		if (characterHeadOnUI != null)
		{
			characterHeadOnUI.Initialize();
		}
	}

	protected override void OnBecameVisible()
	{
		base.OnBecameVisible();

		Replay();

		if (characterHeadOnUI != null)
		{
			characterHeadOnUI.gameObject.SetActive(true);
		}
	}

	protected override void OnBecameInvisible()
	{
		base.OnBecameInvisible();

		if (characterHeadOnUI != null)
		{
			characterHeadOnUI.gameObject.SetActive(false);
		}
	}

	protected override void OnCreateObject(GameObject obj)
	{
		base.OnCreateObject(obj);

#if UNITY_EDITOR
		obj.name = $"{playerName} ({playerGroup})";
#endif
	}

	protected override IEnumerator OnPrepareRendering()
	{
		yield return base.OnPrepareRendering();

		if (originObj == null)
			yield break;

		characterDataSetter = originObj.GetComponent<CharacterDataSetter>();
		if (characterDataSetter != null)
		{
			characterDataSetter.DoFullSetting(playerGroup, playerName);
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

	public void SetPlayerData(string playerGroup, string playerName)
	{
		this.playerGroup = playerGroup;
		this.playerName = playerName;

		if (characterHeadOnUI != null)
		{
			characterHeadOnUI.SetPlayerData(playerGroup, playerName);
		}
	}

	public void RefreshCharacter()
	{
		if (characterDataSetter != null)
		{
			characterDataSetter.DoFullSetting(playerGroup, playerName);
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

		if (meshView != null)
		{
			meshView.FlipX(flipX);
		}
	}

	public void FlipY(bool flipY)
	{
		currentFlipY = flipY;

		float yRot = flipY ? 180 : 0;

		if (originCharacterTransform != null)
		{
			originCharacterTransform.localEulerAngles = new Vector3(0, yRot, 0);
		}
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
