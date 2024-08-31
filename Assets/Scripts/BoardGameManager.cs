using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public abstract class BoardGameSubscriber : MonoBehaviour
{
	public virtual IEnumerator OnRollDice(int diceCount) { yield return null; }
	public virtual IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount) { yield return null; }
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

	public struct StateOrderData
	{
		public GameState currentState;
		public string currentItemCode;
		public int nextOrder;

		public static StateOrderData Make(GameState state, string currentItemCode, int nextOrder)
		{
			var data = new StateOrderData();

			data.currentState = state;
			data.currentItemCode = currentItemCode;
			data.nextOrder = nextOrder;

			return data;
		}
	}

	public class StateData
	{
		public readonly int startOrder = 0;
		public readonly int diceCount = 0;
		public readonly int moveNextOrder = 0;

		public int CurrentOrder { get; private set; }

		private Queue<StateOrderData> stateOrderQueue = new Queue<StateOrderData>();

		public StateData(int startOrder, int moveNextOrder, int diceCount)
		{
			this.startOrder = startOrder;
			this.moveNextOrder = moveNextOrder;

			this.diceCount = diceCount;

			CurrentOrder = moveNextOrder;
		}

		public void PushActionOrder(GameState currentState, string currentItemCode, int nextOrder)
		{
			stateOrderQueue.Enqueue(StateOrderData.Make(currentState, currentItemCode, nextOrder));
		}

		public bool TryGetNextOrder(GameState currentState, out string currentItemCode, out GameState nextState, out int nextOrder)
		{
			nextState = GameState.None;
			nextOrder = 0;
			currentItemCode = string.Empty;

			if (stateOrderQueue.TryDequeue(out var currentData))
			{
				if (currentState == currentData.currentState)
				{
					if (stateOrderQueue.TryPeek(out var nextData))
					{
						nextState = nextData.currentState;
					}
					else
					{
						nextState = GameState.None;
					}

					nextOrder = currentData.nextOrder;
					currentItemCode = currentData.currentItemCode;

					CurrentOrder = nextOrder;

					return true;
				}
				else
				{
					Debug.LogError($"{currentState} 상태에서 {currentData.currentState} 상태 데이터 접근을 시도했습니다.");
					return false;
				}
				
			}

			return false;
		}
	}

	public class StateViewData
	{

	}

	[SerializeField] private bool isDiceDebugMode = false;

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileDataManager tileDataManager;

	[SerializeField] private CharacterMoveComponent characterMoveComponent;

	[SerializeField] private BoardGameSubscriber[] subscribers;

	private GameState currentGameState = GameState.None;

	private Dictionary<GameState, Func<IEnumerator>> stateFuncMap = new Dictionary<GameState, Func<IEnumerator>>();
	private Dictionary<GameState, Func<bool>> stateCheckFuncMap = new Dictionary<GameState, Func<bool>>();

	private StateData currentStateData = null;
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

	private void TryChangeState(GameState newState, StateData data = null)
	{
		if (stateCheckFuncMap.TryGetValue(newState, out var func))
		{
			Debug.Log($"Change Game State : {currentGameState} > {newState}");

			currentGameState = newState;
			currentStateData = data;
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
		// 사전에 데이터 전부 세팅
		yield return ProcessData();

		// 주사위 굴리기 연출부터 시작
		foreach (var subscriber in subscribers)
		{
			yield return subscriber?.OnRollDice(currentStateData.diceCount); 
		}

		TryChangeState(GameState.MoveCharacter, currentStateData);
	}

	private IEnumerator ProcessData()
	{
		int currentOrder = playerDataContainer.currentTileOrder;
		int diceCount = GetNextDiceCount();

		int nextOrder = tileDataManager.GetNextOrder(currentOrder, diceCount, out var barricadeItem);

		if (barricadeItem != null)
		{
			barricadeItem.Use(tileDataManager, playerDataContainer, nextOrder);
		}

		currentStateData = new StateData(currentOrder, nextOrder, diceCount);
		playerDataContainer.SaveCurrentOrder(nextOrder);

		ProcessItemData();

		yield return HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	private void ProcessItemData()
	{
		int currentOrder = playerDataContainer.currentTileOrder;

		FieldItem item = tileDataManager.GetCurrentTileItem(currentOrder);
		string itemCode = string.Empty;

		if (item != null)
		{
			itemCode = item.fieldItemCode;
			item.Use(tileDataManager, playerDataContainer, currentOrder);
		}

		int nextOrder = playerDataContainer.currentTileOrder;

		currentStateData.PushActionOrder(GameState.GetItem, itemCode, nextOrder);

		if(currentOrder != nextOrder)
		{
			ProcessItemData();
		}
		else
		{
			ProcessTileData();
		}
	}

	private void ProcessTileData()
	{
		int currentOrder = playerDataContainer.currentTileOrder;

		var specialTile = tileDataManager.GetCurrentSpecialTile(currentOrder);
		if (specialTile != null)
		{
			specialTile.DoAction(tileDataManager);
		}

		int nextOrder = playerDataContainer.currentTileOrder;

		currentStateData.PushActionOrder(GameState.TileAction, string.Empty, nextOrder);

		if (currentOrder != nextOrder)
		{
			ProcessItemData();
		}
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
		if (currentStateData != null)
		{
			int currentOrder = currentStateData.startOrder;
			int nextOrder = currentStateData.moveNextOrder;
			int diceCount = currentStateData.diceCount;

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnMove(currentOrder, nextOrder, diceCount); // 캐릭터 움직임 관련 연출
			}
		}

		TryChangeState(GameState.GetItem, currentStateData);
	}

	private IEnumerator ProcessGetItem()
	{
		if (currentStateData == null)
			yield break;

		int currentOrder = currentStateData.CurrentOrder;
		if (currentStateData.TryGetNextOrder(currentGameState, out var currentItemCode, out var nextState, out int nextOrder) == false)
			yield break;

		FieldItem item = tileDataManager.GetFieldItem(currentOrder);
		if (item != null)
		{
			tileDataManager.RemoveFieldItem(currentOrder);

			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnGetItem(item, currentOrder, nextOrder);
			}
		}

		TryChangeState(nextState, currentStateData);
	}

	private IEnumerator ProcessTileAction()
	{
		if (currentStateData == null)
			yield break;

		int currentOrder = currentStateData.CurrentOrder;
		if (currentStateData.TryGetNextOrder(currentGameState, out var currentItemCode, out var nextState, out int nextOrder) == false)
			yield break;

		var specialTile = tileDataManager.GetCurrentSpecialTile(currentOrder);
		if (specialTile != null)
		{
			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnDoTileAction(tileDataManager, currentOrder, nextOrder);
			}
		}

		TryChangeState(nextState, currentStateData);
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

			var tileOrders = GetReplaceableTileOrders(replaceItem, min, max);
			tileDataManager.SetSelectTiles(tileOrders);
		}

		UIManager.Instance.CloseMainCanvas();
		UIManager.Instance.Close(PopupType.Inventory);

		UIManager.Instance.TryOpen(PopupType.BatchMode, new BatchModePopup.Parameter(replaceItem));
	}

	private IEnumerable<int> GetReplaceableTileOrders(ReplaceFieldItem replaceItem, int min, int max)
	{
		if (replaceItem == null)
			yield break;

		min = Mathf.Max(0, min);
		max = Mathf.Min(max, int.MaxValue);

		foreach (int tileOrder in CommonFunc.ToRange(min, max))
		{
			if (replaceItem.IsReplaceable(tileDataManager, playerDataContainer, tileOrder))
			{
				yield return tileOrder;
			}
		}
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
