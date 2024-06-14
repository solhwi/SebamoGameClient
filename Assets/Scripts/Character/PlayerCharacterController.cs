using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    Idle = 0,
    Jump = 1,
}

/// <summary>
/// 캐릭터 스테이트 관리
/// </summary>
public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;

    private CharacterState currentState = CharacterState.Idle;

    public void TryChangeState(CharacterState state)
    {
        if (currentState == state)
            return;

        animator.Play(state.ToString());
	}

	private void Update()
	{
		if (currentState == CharacterState.Jump)
        {
            // 여기서 목적지까지 이동한다.
        }
	}
}
