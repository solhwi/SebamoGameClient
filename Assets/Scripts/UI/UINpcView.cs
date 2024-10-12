using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINpcView : ObjectView
{
	[SerializeField] private string startStateName = string.Empty;

	protected override void OnCreateObject(GameObject obj)
	{
		base.OnCreateObject(obj);

		if (objAnimator != null)
		{
			objAnimator.Play(startStateName, 0, 0);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		isVisible = true;
		if (objAnimator != null)
		{
			objAnimator.Play(startStateName, 0, 0);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		isVisible = false;
	}

	protected override void OnBecameVisible()
	{

	}

	protected override void OnBecameInvisible()
	{

	}
}
