using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public abstract class BoardGameSubscriber : MonoBehaviour
{
	public virtual IEnumerator OnRollDice(int diceCount) { yield return null; }
	public virtual IEnumerator OnMove(int currentOrder, int diceCount) { yield return null; }
	public virtual IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder) { yield return null; }
	public virtual IEnumerator OnDoTileAction(TileDataManager tileDataManager, int currentOrder, int nextOrder) { yield return null; }
}

public class BoardGameManager : Singleton<BoardGameManager>
{
	public enum GameState
	{
		None = 0,
		RollDice = 1,
		MoveCharacter = 2,
		GetItem = 3,
		TileAction = 4,
	}

	public class StateParam
	{
		public readonly int currentOrder = 0;
		public readonly int diceCount = 0;
		public readonly int nextOrder = 0;

		public StateParam(int currentOrder, int nextOrder)
		{
			this.currentOrder = currentOrder;
			this.nextOrder = nextOrder;
		}

		public StateParam(int currentOrder, int nextOrder, int diceCount)
		{
			this.currentOrder = currentOrder;
			this.nextOrder = nextOrder;

			this.diceCount = diceCount;
		}
	}

	[SerializeField] private bool isDiceDebugMode = false;

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileDataManager tileDataManager;

	[SerializeField] private CharacterMoveComponent characterMoveComponent;

	[SerializeField] private BoardGameSubscriber[] subscribers;

	private GameState currentGameState = GameState.None;

	private Dictionary<GameState, Func<IEnumerator>> stateFuncMap = new Dictionary<GameState, Func<IEnumerator>>();
	private Dictionary<GameState, Func<bool>> stateCheckFuncMap = new Dictionary<GameState, Func<bool>>();

	private StateParam currentStateParam = null;

	private ReplaceFieldItem currentReplaceFieldItem = null;

	protected override void Awake()
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
		characterMoveComponent.gameObject.SetActive(false);

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

	private void TryChangeState(GameState newState, StateParam param = null)
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
		int diceCount = GetNextDiceCount();

		int nextOrder = tileDataManager.GetNextOrder(currentOrder, diceCount);

		yield return playerDataContainer.SaveCurrentOrder(nextOrder); // 주사위를 굴리는 시점에 반영한다.

		foreach (var subscriber in subscribers)
		{
			yield return subscriber?.OnRollDice(diceCount); // 주사위 굴리기 연출
		}

		TryChangeState(GameState.MoveCharacter, new StateParam(currentOrder, nextOrder, diceCount));
	}

	private int GetNextDiceCount()
	{
		if (isDiceDebugMode)
		{
			return playerDataContainer.NextDiceCount;
		}
		else
		{
			return UnityEngine.Random.Range(1, 7);
		}
	}

	private IEnumerator ProcessMoveCharacter()
	{
		if (currentStateParam != null)
		{
			int currentOrder = currentStateParam.currentOrder;
			int diceCount = currentStateParam.diceCount;

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnMove(currentOrder, diceCount); // 캐릭터 움직임 관련 연출
			}
		}

		TryChangeState(GameState.GetItem, currentStateParam);
	}

	private IEnumerator ProcessGetItem()
	{
		int nextOrder = currentStateParam.nextOrder;
		int nextNextOrder = nextOrder;

		FieldItem item = tileDataManager.GetCurrentTileItem(nextOrder);
		if (item != null)
		{
			yield return item.Use(tileDataManager, playerDataContainer, nextOrder);
			item.Destroy();

			nextNextOrder = playerDataContainer.currentTileOrder;

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnGetItem(item, nextOrder, nextNextOrder);
			}
		}

		// 내부에서 아이템 효과로 인해 추가 이동한 경우, 연쇄 처리
		if (nextOrder != nextNextOrder)
		{
			TryChangeState(GameState.GetItem, new StateParam(nextOrder, nextNextOrder));
		}
		else
		{
			TryChangeState(GameState.TileAction, currentStateParam);
		}
	}

	private IEnumerator ProcessTileAction()
	{
		int nextOrder = currentStateParam.nextOrder;
		int nextNextOrder = nextOrder;

		var specialTile = tileDataManager.GetCurrentSpecialTile(nextOrder);
		if (specialTile != null)
		{
			yield return specialTile.DoAction(tileDataManager);

			nextNextOrder = playerDataContainer.currentTileOrder;

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnDoTileAction(tileDataManager, nextOrder, nextNextOrder);
			}
		}

		// 내부에서 타일 효과로 인해 추가 이동한 경우, 연쇄 처리
		if (nextOrder != nextNextOrder)
		{
			TryChangeState(GameState.GetItem, new StateParam(nextOrder, nextNextOrder));
		}
		else
		{
			TryChangeState(GameState.None);
		}
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
		return currentPlayerTileData.tilePlayerPosition;
	}

	public void StartReplaceMode(ReplaceFieldItem replaceItem)
	{
		currentReplaceFieldItem = replaceItem;

		if (currentReplaceFieldItem != null)
		{
			int min = playerDataContainer.currentTileOrder + replaceItem.ranges[0];
			int max = playerDataContainer.currentTileOrder + replaceItem.ranges[1];

			tileDataManager.SetSelectTiles(min, max);
		}

		UIManager.Instance.CloseMainCanvas();
		UIManager.Instance.Close(PopupType.Inventory);

		UIManager.Instance.TryOpen(PopupType.BatchMode, new BatchModePopup.Parameter(replaceItem));
	}

	public void EndReplaceMode(bool isReplaced)
	{
		currentReplaceFieldItem = null;

		tileDataManager.ClearSelectTile();

		UIManager.Instance.OpenMainCanvas();
		UIManager.Instance.Close(PopupType.BatchMode);

		if (isReplaced == false)
		{
			UIManager.Instance.TryOpen(PopupType.Inventory, new InventoryPopup.Parameter(InventoryPopup.TabType.Replace));
		}
	}
}
