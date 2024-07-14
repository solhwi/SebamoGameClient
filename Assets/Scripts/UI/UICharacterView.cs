using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterView : ObjectView
{
	private CharacterDataSetter characterDataSetter = null;
	private CharacterAnimationController characterAnimationController = null;

	protected override void Start()
	{
		base.Start();

		characterDataSetter = originObj.GetComponent<CharacterDataSetter>();
		if (characterDataSetter != null)
		{
			characterDataSetter.DoFullSetting();

			characterAnimationController = characterDataSetter.GetComponentInChildren<CharacterAnimationController>();
		}
	}

	public void DoIdle()
	{
		characterAnimationController.DoIdle();
	}

	public void DoRun()
	{
		characterAnimationController.DoRun();
	}

	// 임의로 입혀볼 수 있는 함수 추가 예정
}
