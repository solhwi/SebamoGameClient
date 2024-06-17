using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    Idle = 0,
    Run = 1,
}

public class CharacterAnimationController : MonoBehaviour
{
	[SerializeField] private RuntimeAnimatorController controller = null;

	private Animator Animator
	{
		get
		{
			if (animator == null)
			{
				animator = GetComponentInChildren<Animator>();
				animator.runtimeAnimatorController = controller;
			}
			return animator;
		}
	}

	private Animator animator = null;
	public CharacterState currentState = CharacterState.Idle;

	public void SetAvatar(Avatar avatar)
	{
		Animator.avatar = avatar;
	}

	public void ChangeState(CharacterState state)
    {
		currentState = state;
		Animator.Play(state.ToString());
	}
}
