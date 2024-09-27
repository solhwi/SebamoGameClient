using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINpcView : ObjectView
{
	[SerializeField] private string startStateName = string.Empty;
	private Animator objAnimator;

	protected override IEnumerator OnPrepareRendering()
	{
		yield return base.OnPrepareRendering();

		objAnimator = originObj.GetComponent<Animator>();
		if (objAnimator != null)
		{
			objAnimator.Play(startStateName, 0, 0);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if (objAnimator != null)
		{
			objAnimator.Play(startStateName, 0, 0);
		}
	}
}
