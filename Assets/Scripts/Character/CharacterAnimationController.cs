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
    [SerializeField] private Animator animator = null;

    public CharacterState currentState = CharacterState.Idle;

    public void ChangeState(CharacterState state)
    {
		currentState = state;
		animator.Play(state.ToString());
	}
}
