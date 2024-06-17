using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    Idle, // 서있기 (노무기, 한손일반무기)
	Run, // 뛰기 (노무기, 한손일반무기)

	Idle_TwinHand, // 서있기 (쌍검)
	Run_TwinHand, // 뛰기 (쌍검)
	Idle_TwoHand, // 서있기 (두손무기)
	Run_TwoHand, // 뛰기 (두손무기)
	Idle_Net, // 서있기 (잠자리채)
	Run_Net, // 뛰기 (잠자리채)
	Idle_Umbrella, // 서있기 (우산)
	Run_Umbrella, // 뛰기 (우산)
	Idle_GreatSword, // 서있기 (그레이트소드)
	Run_GreatSword, // 뛰기 (그레이트소드)

	Attack_GreatSword, // 공격 (그레이트소드)
	Attack_TwinHand, // 공격 (쌍검)

	Clap, // 박수치기
	LookAround, // 둘러보기
}

public class CharacterAnimationController : MonoBehaviour
{
	[SerializeField] private RuntimeAnimatorController controller = null;
	[SerializeField] private PlayerDataContainer playerDataContainer = null;

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

	private void Start()
	{
		DoIdle();
	}

	public void SetAvatar(Avatar avatar)
	{
		Animator.avatar = avatar;
	}

	public void DoIdle()
	{
		if (playerDataContainer.characterPropTypes == null || playerDataContainer.characterPropTypes.Length <= 0)
		{
			ChangeState(CharacterState.Idle);
		}
		else
		{
			var propType = playerDataContainer.characterPropTypes[0];
			if (propType == PropType.GreatSword)
			{
				ChangeState(CharacterState.Idle_GreatSword);
			}
			else if(propType == PropType.Net)
			{
				ChangeState(CharacterState.Idle_Net);
			}
			else if(propType == PropType.Umbrella)
			{
				ChangeState(CharacterState.Idle_Umbrella);
			}
			else if(propType == PropType.TwinDagger_L || propType == PropType.TwinDagger_R)
			{
				ChangeState(CharacterState.Idle_TwinHand);
			}
			else if(propType == PropType.Axe || propType == PropType.PickAx || propType == PropType.Shovel)
			{
				ChangeState(CharacterState.Idle_TwoHand);
			}
			else
			{
				ChangeState(CharacterState.Idle);
			}
		}
	}

	public void DoRun()
	{
		if (playerDataContainer.characterPropTypes == null || playerDataContainer.characterPropTypes.Length <= 0)
		{
			ChangeState(CharacterState.Run);
		}
		else
		{
			var propType = playerDataContainer.characterPropTypes[0];
			if (propType == PropType.GreatSword)
			{
				ChangeState(CharacterState.Run_GreatSword);
			}
			else if (propType == PropType.Net)
			{
				ChangeState(CharacterState.Run_Net);
			}
			else if (propType == PropType.Umbrella)
			{
				ChangeState(CharacterState.Run_Umbrella);
			}
			else if (propType == PropType.TwinDagger_L || propType == PropType.TwinDagger_R)
			{
				ChangeState(CharacterState.Run_TwinHand);
			}
			else if (propType == PropType.Axe || propType == PropType.PickAx || propType == PropType.Shovel)
			{
				ChangeState(CharacterState.Run_TwoHand);
			}
			else
			{
				ChangeState(CharacterState.Run);
			}
		}
	}

	public void DoAttack()
	{
		if (playerDataContainer.characterPropTypes == null || playerDataContainer.characterPropTypes.Length <= 0)
			return;

		var propType = playerDataContainer.characterPropTypes[0];
		if (propType == PropType.GreatSword)
		{
			ChangeState(CharacterState.Attack_GreatSword);
		}
		else if (propType == PropType.TwinDagger_L || propType == PropType.TwinDagger_R)
		{
			ChangeState(CharacterState.Attack_TwinHand);
		}
	}

	public void DoLookAround()
	{
		ChangeState(CharacterState.LookAround);
	}

	public void DoClap()
	{
		ChangeState(CharacterState.Clap);
	}

	private void ChangeState(CharacterState state)
    {
		currentState = state;
		Animator.Play(state.ToString());
	}
}
