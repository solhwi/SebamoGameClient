using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINpcView : ObjectView
{
	[SerializeField] private string startStateName = string.Empty;
	private Animator objAnimator;

	protected override void InitializeTarget()
	{
		base.InitializeTarget();

		objAnimator = originObj.GetComponent<Animator>();
		if (objAnimator != null)
		{
			objAnimator.Play(startStateName, 0, 0);
		}
	}

	private void OnEnable()
	{
		if (objAnimator != null)
		{
			objAnimator.Play(startStateName, 0, 0);
		}
	}
}
