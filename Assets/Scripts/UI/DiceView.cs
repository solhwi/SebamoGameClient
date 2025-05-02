using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class AnimationHelper
{
	public static IEnumerator WaitForEnd(this Animator animator, string stateName)
	{
		while (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) == false)
		{
			yield return null;
		}

		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f)
		{
			yield return null;
		}
	}

	public static IEnumerator WaitForEnd(this CharacterAnimationController controller, string stateName)
	{
		yield return controller.Animator.WaitForEnd(stateName);
	}
}

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
			yield return objAnimator.WaitForEnd(stateName);
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
