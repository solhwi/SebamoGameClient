using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceView : ObjectView
{
	[SerializeField] private string startStateName = string.Empty;
	
	public IEnumerator DoDiceAction(int number)
	{
		string stateName = $"{startStateName}_{number}";

		gameObject.SetActive(true);
		originObj.SetActive(true);

		if (objAnimator != null)
		{
			objAnimator.Play(stateName, 0, 0);

			while (objAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName) == false)
			{
				yield return null;
			}

			while (objAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f)
			{
				yield return null;
			}
		}

		originObj.SetActive(false);
		gameObject.SetActive(false);

		yield return null;
	}

	protected override void OnCreateObject(GameObject obj)
	{
		base.OnCreateObject(obj);

		originObj.SetActive(false);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		isVisible = true;
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
