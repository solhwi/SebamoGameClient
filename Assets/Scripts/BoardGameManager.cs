using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardGameSubscriber : MonoBehaviour
{
	public virtual IEnumerator OnRollDice(int diceCount) { yield return null; }
	public virtual IEnumerator OnMove(int currentOrderIndex, int diceCount) { yield return null; }
	public virtual IEnumerator OnGetItem(string itemCode) { yield return null; }
	public virtual IEnumerator OnDoTileAction(int currentOrderIndex, int nextOrderIndex) { yield return null; }
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
		public readonly int currentOrderIndex = 0;
		public readonly int diceCount = 0;

		public MoveStateParam(int currentOrderIndex, int diceCount)
		{
			this.currentOrderIndex = currentOrderIndex;
			this.diceCount = diceCount;
		}
	}

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileDataManager tileDataManager;

	[SerializeField] private CharacterMoveComponent characterMoveComponent;

	[SerializeField] private BoardGameSubscriber[] subscribers;

	private Coroutine boardGameRoutine = null;
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

	private void OnDestroy()
	{
		ExitGame();
	}

	private void ExitGame()
	{
		if (boardGameRoutine != null)
		{
			StopCoroutine(boardGameRoutine);
		}
	}

	private void StartGame()
	{
		ExitGame();

		boardGameRoutine = StartCoroutine(ProcessBoardGame());
	}

	private IEnumerator Start()
	{
		characterMoveComponent.gameObject.SetActive(false);

		// 플레이어 캐릭터 뷰 타일 위로 배치
		Vector2 playerPos = GetPlayerPos();
		characterMoveComponent.SetPosition(playerPos);

		characterMoveComponent.gameObject.SetActive(true);

		yield return null;

		// 보드 게임 시작
		StartGame();
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
		int currentOrderIndex = playerDataContainer.currentTileOrderIndex;
		int diceCount = UnityEngine.Random.Range(1, 7); // 1 ~ 6 생성

		playerDataContainer.SaveCurrentOrderIndex(currentOrderIndex + diceCount); // 주사위를 굴리는 시점에 반영한다.

		foreach (var subscriber in subscribers)
		{
			yield return subscriber?.OnRollDice(diceCount); // 주사위 굴리기 연출
		}

		TryChangeState(GameState.MoveCharacter, new MoveStateParam(currentOrderIndex, diceCount));
	}

	private IEnumerator ProcessMoveCharacter()
	{
		var param = currentStateParam as MoveStateParam;
		if (param != null)
		{
			int currentOrderIndex = param.currentOrderIndex;
			int diceCount = param.diceCount;

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnMove(currentOrderIndex, diceCount); // 캐릭터 움직임 관련 연출
			}
		}

		TryChangeState(GameState.GetItem);
	}

	private IEnumerator ProcessGetItem()
	{
		// 1. 현재 타일 위치에 해당하는 아이템 코드를 가져온다. - 에디터로 저장한 so
		// 1-1. 없는 경우 TileAction으로 이동
		// 2. 아이템 코드로 드롭 아이템을 가져온다 - 드롭 아이템 팩토리
		// 3. 드롭 아이템을 사용한다
		// 4. 드롭 아이템으로부터 실제로 얻은 아이템 코드를 가져올 수 있다

		foreach (var subscriber in subscribers)
		{
			yield return subscriber?.OnGetItem(""); // 얻은 아이템 코드를 뿌림
		}

		TryChangeState(GameState.TileAction);

	}

	private IEnumerator ProcessTileAction()
	{
		int currentOrderIndex = playerDataContainer.currentTileOrderIndex;

		// 1. 현재 타일 위치로 타일 코드를 가져온다. - 에디터로 저장한 so
		// 1-1. 특수 타일이 아닌 경우 None으로 이동
		// 2. 타일 코드로 타일 액션을 가져온다 - 타일 액션 팩토리
		// 3. 타일 액션을 수행한다

		int nextOrderIndex = playerDataContainer.currentTileOrderIndex;

		foreach (var subscriber in subscribers)
		{
			yield return subscriber?.OnDoTileAction(currentOrderIndex, nextOrderIndex); // 이동
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
		int currentPlayerTileOrderIndex = playerDataContainer.currentTileOrderIndex;

		if (tileDataManager == null)
		{
			Debug.Log("타일 데이터 매니저 없음");
			return default;
		}

		var currentPlayerTileData = tileDataManager.GetTileDataByOrder(currentPlayerTileOrderIndex);
		return currentPlayerTileData.tileWorldPosition;
	}
}
