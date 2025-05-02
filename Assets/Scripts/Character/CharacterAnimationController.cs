using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	BarricadeDown,      // 바리케이드에 맞고 넘어짐

	DoubleDiceBuff, // 주사위 두배 버프 획득
	HalfDiceBuff,	// 주사위 반감 디버프 획득
	MinusDiceBuff,	// 주사위 마이너스 디버프 획득
	DrunkBuff,			// 1 또는 6 버프 획득
	OddBuff,			// 홀수 버프 획득
	EvenBuff,			// 짝수 버프 획득
}

public enum CharacterStateType
{
	Idle,
	Run,
	Attack,
	LookAround,
	Clap,
	DropItem
}

public class CharacterAnimationController : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer = null;
	[SerializeField] private RuntimeAnimatorController controller = null;
	
	private float crossFadeTime = 0.0f;

	public Animator Animator
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

	private CharacterStateType currentStateType = CharacterStateType.Idle;
	public CharacterState currentState = CharacterState.Idle;

	private string playerGroup = string.Empty;
	private string playerName = string.Empty;

	public void SetPlayerData(string playerGroup, string playerName)
	{
		this.playerGroup = playerGroup;
		this.playerName = playerName;
	}

	public void SetAvatar(Avatar avatar)
	{
		Animator.avatar = avatar;
	}

	public void DoIdle(float crossFadeTime = 0.0f)
	{
		this.crossFadeTime = crossFadeTime;

		currentStateType = CharacterStateType.Idle;

		var propType = playerDataContainer.GetEquippedPropType(playerGroup, playerName).FirstOrDefault();
		if (propType == PropType.GreatSword)
		{
			ChangeState(CharacterStateType.Idle, CharacterState.Idle_GreatSword);
		}
		else if (propType == PropType.Net)
		{
			ChangeState(CharacterStateType.Idle, CharacterState.Idle_Net);
		}
		else if (propType == PropType.Umbrella)
		{
			ChangeState(CharacterStateType.Idle, CharacterState.Idle_Umbrella);
		}
		else if (propType == PropType.TwinDagger_L || propType == PropType.TwinDagger_R)
		{
			ChangeState(CharacterStateType.Idle, CharacterState.Idle_TwinHand);
		}
		else if (propType == PropType.Axe || propType == PropType.PickAx || propType == PropType.Shovel)
		{
			ChangeState(CharacterStateType.Idle, CharacterState.Idle_TwoHand);
		}
		else
		{
			ChangeState(CharacterStateType.Idle, CharacterState.Idle);
		}
	}

	public void DoRun(float crossFadeTime = 0.0f)
	{
		this.crossFadeTime = crossFadeTime;

		currentStateType = CharacterStateType.Run;

		var propType = playerDataContainer.GetEquippedPropType(playerGroup, playerName).FirstOrDefault();
		if (propType == PropType.GreatSword)
		{
			ChangeState(CharacterStateType.Run, CharacterState.Run_GreatSword);
		}
		else if (propType == PropType.Net)
		{
			ChangeState(CharacterStateType.Run, CharacterState.Run_Net);
		}
		else if (propType == PropType.Umbrella)
		{
			ChangeState(CharacterStateType.Run, CharacterState.Run_Umbrella);
		}
		else if (propType == PropType.TwinDagger_L || propType == PropType.TwinDagger_R)
		{
			ChangeState(CharacterStateType.Run, CharacterState.Run_TwinHand);
		}
		else if (propType == PropType.Axe || propType == PropType.PickAx || propType == PropType.Shovel)
		{
			ChangeState(CharacterStateType.Run, CharacterState.Run_TwoHand);
		}
		else
		{
			ChangeState(CharacterStateType.Run, CharacterState.Run);
		}
	}

	public void DoAttack()
	{
		currentStateType = CharacterStateType.Attack;

		var propType = playerDataContainer.GetEquippedPropType(playerGroup, playerName).FirstOrDefault();
		if (propType == PropType.GreatSword)
		{
			ChangeState(CharacterStateType.Attack, CharacterState.Attack_GreatSword);
		}
		else if (propType == PropType.TwinDagger_L || propType == PropType.TwinDagger_R)
		{
			ChangeState(CharacterStateType.Attack, CharacterState.Attack_TwinHand);
		}
	}

	public void ChangeState(CharacterStateType stateType, CharacterState state)
	{
		currentStateType = stateType;

		currentState = state;
		Animator.CrossFade(state.ToString(), crossFadeTime);
	}

	public IEnumerator ChangeItemDropState(FieldItem dropItem)
	{
		yield return dropItem.ChangeState(this);
	}

	public void Replay()
	{
		switch(currentStateType)
		{
			case CharacterStateType.Idle:
				DoIdle();
				break;

			case CharacterStateType.Run:
				DoRun();
				break;

			case CharacterStateType.Attack:
				DoAttack();
				break;

			default:
				ChangeState(currentStateType, currentState);
				break;
		}
	}
}
