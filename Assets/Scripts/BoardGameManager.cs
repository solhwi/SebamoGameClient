using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardGameSubscriber : MonoBehaviour
{
	public virtual IEnumerator OnRollDice(int diceCount) { yield return null; }
	public virtual IEnumerator OnMove(int currentOrder, int diceCount) { yield return null; }
	public virtual IEnumerator OnGetItem(DropItem dropItem) { yield return null; }
	public virtual IEnumerator OnDoTileAction(SpecialTileBase tile, int currentOrder, int nextOrderIndex) { yield return null; }
}

public class BoardGameManager : MonoBehaviour
{
	public enum GameState
	{
		None = 0,
		RollDice = 1,
		MoveCharacter = 2,
		GetItem = 3,
		TileAction = 4,
	}

	public interface IStateParam
	{

	}

	public class MoveStateParam : IStateParam
	{
		public readonly int currentOrder = 0;
		public readonly int diceCount = 0;

		public MoveStateParam(int currentOrder, int diceCount)
		{
			this.currentOrder = currentOrder;
			this.diceCount = diceCount;
		}
	}

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileDataManager tileDataManager;

	[SerializeField] private CharacterMoveComponent characterMoveComponent;

	[SerializeField] private BoardGameSubscriber[] subscribers;

	private GameState currentGameState = GameState.None;

	private Dictionary<GameState, Func<IEnumerator>> stateFuncMap = new Dictionary<GameState, Func<IEnumerator>>();
	private Dictionary<GameState, Func<bool>> stateCheckFuncMap = new Dictionary<GameState, Func<bool>>();

	private IStateParam currentStateParam = null;

	private void Awake()
	{
		stateFuncMap = new Dictionary<GameState, Func<IEnumerator>>()
		{
			{ GameState.None, null },
			{ GameState.RollDice, ProcessRollDice },
			{ GameState.MoveCharacter, ProcessMoveCharacter },
			{ GameState.GetItem, ProcessGetItem },
			{ GameState.TileAction, ProcessTileAction }
		};

		stateCheckFuncMap = new Dictionary<GameState, Func<bool>>()
		{
			{ GameState.None, () => true },
			{ GameState.RollDice, CanRollDice },
			{ GameState.MoveCharacter, CanMoveCharacter },
			{ GameState.GetItem, CanGetItem },
			{ GameState.TileAction, CanDoTileAction }
		};
	}

	private IEnumerator Start()
	{
		// 타일 위 아이템 이미지 배치
		yield return PrepareItem();

		// 캐릭터 준비
		yield return PrepareCharacter();

		// 보드 게임 시작
		yield return ProcessBoardGame();
	}

	private IEnumerator PrepareItem()
	{
		yield return tileDataManager.PrepareTile();
	}

	private IEnumerator PrepareCharacter()
	{
		characterMoveComponent.gameObject.SetActive(false);

		yield return null;

		// 플레이어 캐릭터 뷰 타일 위로 배치
		Vector2 playerPos = GetPlayerPos();
		characterMoveComponent.SetPosition(playerPos);

		characterMoveComponent.gameObject.SetActive(true);
	}

	public void OnClickRollDice()
	{
		TryChangeState(GameState.RollDice);
	}

	private void TryChangeState(GameState newState, IStateParam param = null)
	{
		if (stateCheckFuncMap.TryGetValue(newState, out var func))
		{
			Debug.Log($"Change Game State : {currentGameState} > {newState}");

			currentGameState = newState;
			currentStateParam = param;
		}
	}

	private bool CanRollDice()
	{
		return currentGameState == GameState.None;
	}

	private bool CanMoveCharacter()
	{
		return currentGameState == GameState.RollDice;
	}

	private bool CanGetItem()
	{
		return currentGameState == GameState.MoveCharacter;
	}

	private bool CanDoTileAction()
	{
		return currentGameState == GameState.GetItem;
	}


	private IEnumerator ProcessBoardGame()
	{
		while(true)
		{
			yield return stateFuncMap[currentGameState]?.Invoke();
		}
	}

	private IEnumerator ProcessRollDice()
	{
		int currentOrder = playerDataContainer.currentTileOrder;
		int diceCount = UnityEngine.Random.Range(1, 7); // 1 ~ 6 생성

		playerDataContainer.SaveCurrentTile(currentOrder + diceCount); // 주사위를 굴리는 시점에 반영한다.

		foreach (var subscriber in subscribers)
		{
			yield return subscriber?.OnRollDice(diceCount); // 주사위 굴리기 연출
		}

		TryChangeState(GameState.MoveCharacter, new MoveStateParam(currentOrder, diceCount));
	}

	private IEnumerator ProcessMoveCharacter()
	{
		var param = currentStateParam as MoveStateParam;
		if (param != null)
		{
			int currentOrder = param.currentOrder;
			int diceCount = param.diceCount;

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnMove(currentOrder, diceCount); // 캐릭터 움직임 관련 연출
			}
		}

		TryChangeState(GameState.GetItem);
	}

	private IEnumerator ProcessGetItem()
	{
		int currentOrder = playerDataContainer.currentTileOrder;

		DropItem item = tileDataManager.GetCurrentTileItem(currentOrder);
		if (item != null)
		{
			item.Use();

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnGetItem(item);
			}
		}

		TryChangeState(GameState.TileAction);
	}

	private IEnumerator ProcessTileAction()
	{
		int currentOrder = playerDataContainer.currentTileOrder;

		var specialTile = tileDataManager.GetCurrentSpecialTile(currentOrder);
		if (specialTile != null)
		{
			specialTile.DoAction();

			int nextOrderIndex = playerDataContainer.currentTileOrder;

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnDoTileAction(specialTile, currentOrder, nextOrderIndex);
			}
		}

		TryChangeState(GameState.None);
	}

	private Vector2 GetPlayerPos()
	{
		if (playerDataContainer == null)
		{
			Debug.Log("플레이어 데이터 없음");
			return default;
		}

		// 내 타일이 몇 번째 순서인 지
		int currentPlayerTileOrderIndex = playerDataContainer.currentTileOrder;

		if (tileDataManager == null)
		{
			Debug.Log("타일 데이터 매니저 없음");
			return default;
		}

		var currentPlayerTileData = tileDataManager.GetTileDataByOrder(currentPlayerTileOrderIndex);
		return currentPlayerTileData.tileWorldPosition;
	}
}
