using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public interface IBoardGameSubscriber
{
	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, int nextBonusMultiplyCount);
	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount);
	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder);
	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder);
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
		public readonly int bonusAddDiceCount = 0;
		public readonly int bonusMultiplyDiceCount = 1;

		public int CurrentOrder { get; private set; }

		private Queue<StateOrderData> stateOrderQueue = new Queue<StateOrderData>();

		public StateData(int startOrder, int moveNextOrder, int diceCount, int bonusAddDiceCount, int bonusMultiplyDiceCount)
		{
			this.startOrder = startOrder;
			this.moveNextOrder = moveNextOrder;

			this.diceCount = diceCount;
			this.bonusAddDiceCount = bonusAddDiceCount;
			this.bonusMultiplyDiceCount = bonusMultiplyDiceCount;

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

	[SerializeField] private bool isDiceDebugMode = false;

	[SerializeField] private Inventory inventory;
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private BuffItemFactory buffItemFactory;
	[SerializeField] private FieldItemFactory fieldItemFactory;

	[SerializeField] private MyCharacterComponent myPlayerCharacter;
	[SerializeField] private CharacterComponent otherPlayerCharacterPrefab;

	private Dictionary<PlayerPacketData, CharacterComponent> playerCharacterDictionary = new Dictionary<PlayerPacketData, CharacterComponent>();

	private List<IBoardGameSubscriber> subscribers = new List<IBoardGameSubscriber>();

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

	public void Subscribe(IBoardGameSubscriber subscriber)
	{
		if (subscriber == null)
			return;

		if (subscribers.Contains(subscriber))
			return;

		subscribers.Add(subscriber);
	}

	public void Unsubscribe(IBoardGameSubscriber subscriber)
	{
		if (subscriber == null)
			return;

		if (subscribers.Contains(subscriber) == false)
			return;

		subscribers.Remove(subscriber);
	}

	private IEnumerator Start()
	{
		myPlayerCharacter.gameObject.SetActive(false);

		// 타일 위 아이템 이미지 배치
		yield return PrepareItem();

		// 캐릭터 준비
		yield return PrepareCharacter();

		// 보드 게임 시작
		yield return ProcessBoardGame();
	}

	private IEnumerator PrepareItem()
	{
		yield return TileDataManager.Instance.PrepareTile();
	}

	private IEnumerator PrepareCharacter()
	{
		playerCharacterDictionary.Clear();

		var myPlayerData = playerDataContainer.GetMyPlayerData();
		if (myPlayerData == null)
			yield break;

		// 플레이어 본인 캐릭터 타일 위로 배치
		int myTileOrder = playerDataContainer.currentTileOrder;
		Vector2 playerPos = TileDataManager.Instance.GetPlayerPosByOrder(myTileOrder);

		myPlayerCharacter.SetPlayerData(myPlayerData.playerGroup, myPlayerData.playerName);
		myPlayerCharacter.SetPosition(playerPos);
		myPlayerCharacter.gameObject.SetActive(true);

#if UNITY_EDITOR
		myPlayerCharacter.gameObject.name = $"MyPlayer ({playerDataContainer.playerName})";
#endif

		playerCharacterDictionary.Add(myPlayerData, myPlayerCharacter);

		yield return null;

		// 타 유저 캐릭터 타일 위로 배치
		if (playerDataContainer.otherPlayerPacketDatas == null)
			yield break;

		foreach (var otherPlayerData in playerDataContainer.otherPlayerPacketDatas)
		{
			var otherPlayerCharacter = Instantiate(otherPlayerCharacterPrefab);
			if (otherPlayerCharacter == null)
				continue;

			int tileOrder = otherPlayerData.playerTileOrder;
			playerPos = TileDataManager.Instance.GetPlayerPosByOrder(tileOrder);

			otherPlayerCharacter.SetPlayerData(otherPlayerData.playerGroup, otherPlayerData.playerName);
			otherPlayerCharacter.SetPosition(playerPos);
			otherPlayerCharacter.gameObject.SetActive(true);

#if UNITY_EDITOR
			otherPlayerCharacter.gameObject.name = $"Player ({otherPlayerData.playerName})" ;
#endif

			playerCharacterDictionary.Add(otherPlayerData, otherPlayerCharacter);
		}
	}

	public void OnClickRollDice()
	{
		TryChangeState(GameState.RollDice);
	}

	private void TryChangeState(GameState newState, StateData data = null)
	{
		if (stateCheckFuncMap.TryGetValue(newState, out var func))
		{
			if (func != null && func.Invoke())
			{
				Debug.Log($"Change Game State : {currentGameState} > {newState}");

				currentGameState = newState;
				currentStateData = data;
			}
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
		return currentGameState == GameState.MoveCharacter || currentGameState == GameState.TileAction;
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
		// 서버와 타일 데이터 동기화
		yield return HttpNetworkManager.Instance.TryGetOtherPlayerDatas();
		yield return HttpNetworkManager.Instance.TryGetTileData();

		// 버프를 포함한 데이터 세팅
		ProcessData();

		// 서버에 내 정보, 타일 데이터 동기화
		yield return HttpNetworkManager.Instance.TryPostMyPlayerData();
		yield return HttpNetworkManager.Instance.TryPostTileData();

		// 주사위 굴리기 연출 시작
		foreach (var subscriber in subscribers)
		{
			yield return subscriber?.OnRollDice(currentStateData.diceCount, currentStateData.bonusAddDiceCount, currentStateData.bonusMultiplyDiceCount); 
		}

		TryChangeState(GameState.MoveCharacter, currentStateData);
	}

	private void ProcessData()
	{
		// 버프 아이템 사전 적용
		string buffItemCode = inventory.GetUsableBuffItemCode();

		var buffItem = buffItemFactory.Make(buffItemCode);
		if (buffItem != null)
		{
			buffItem.TryUse(playerDataContainer);
		}

		// 주사위 굴리기
		int currentOrder = playerDataContainer.currentTileOrder;
		int diceCount = GetNextDiceCount();

		int bonusAddDiceCount = playerDataContainer.NextBonusAddDiceCount;
		int bonusMultiplyDiceCount = playerDataContainer.NextBonusMultiplyDiceCount;

		int nextOrder = TileDataManager.Instance.GetNextOrder(currentOrder, diceCount * bonusMultiplyDiceCount + bonusAddDiceCount, out var barricadeItem);

		if (barricadeItem != null)
		{
			barricadeItem.Use(playerDataContainer, nextOrder);
		}

		currentStateData = new StateData(currentOrder, nextOrder, diceCount, bonusAddDiceCount, bonusMultiplyDiceCount);

		playerDataContainer.ClearBonusDiceCount();
		playerDataContainer.SaveCurrentOrder(nextOrder);

		// 아이템, 타일 반영
		ProcessItemData();
	}

	private void ProcessItemData()
	{
		int currentOrder = playerDataContainer.currentTileOrder;

		FieldItem item = TileDataManager.Instance.GetCurrentTileItem(currentOrder);
		string itemCode = string.Empty;

		if (item != null)
		{
			itemCode = item.fieldItemCode;
			item.Use(playerDataContainer, currentOrder);
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

		var specialTile = TileDataManager.Instance.GetCurrentSpecialTile(currentOrder);
		if (specialTile != null)
		{
			specialTile.DoAction();
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

		FieldItem item = TileDataManager.Instance.GetFieldItem(currentOrder);
		if (item != null)
		{
			TileDataManager.Instance.RemoveFieldItem(currentOrder);

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

		var specialTile = TileDataManager.Instance.GetCurrentSpecialTile(currentOrder);
		if (specialTile != null)
		{
			foreach (var subscriber in subscribers)
			{
				yield return subscriber?.OnDoTileAction(currentOrder, nextOrder);
			}
		}

		TryChangeState(nextState, currentStateData);
	}

	public void StartReplaceMode(string fieldItemCode)
	{
		currentReplaceFieldItem = fieldItemFactory.Make<ReplaceFieldItem>(fieldItemCode);

		if (currentReplaceFieldItem != null)
		{
			int min = playerDataContainer.currentTileOrder + currentReplaceFieldItem.ranges[0];
			int max = playerDataContainer.currentTileOrder + currentReplaceFieldItem.ranges[1];

			var tileOrders = GetReplaceableTileOrders(currentReplaceFieldItem, min, max);
			TileDataManager.Instance.SetSelectTiles(tileOrders);
		}

		UIManager.Instance.CloseMainCanvas();
		UIManager.Instance.Close(PopupType.Inventory);

		UIManager.Instance.TryOpen(PopupType.BatchMode, new BatchModePopup.Parameter(currentReplaceFieldItem));
	}

	private IEnumerable<int> GetReplaceableTileOrders(ReplaceFieldItem replaceItem, int min, int max)
	{
		if (replaceItem == null)
			yield break;

		min = Mathf.Max(0, min);
		max = Mathf.Min(max, int.MaxValue);

		foreach (int tileOrder in CommonFunc.ToRange(min, max))
		{
			if (replaceItem.IsReplaceable(playerDataContainer, tileOrder))
			{
				yield return tileOrder;
			}
		}
	}

	public void EndReplaceMode(bool isReplaced)
	{
		currentReplaceFieldItem = null;

		TileDataManager.Instance.ClearSelectTile();

		UIManager.Instance.OpenMainCanvas();
		UIManager.Instance.Close(PopupType.BatchMode);

		if (isReplaced == false)
		{
			UIManager.Instance.TryOpen(PopupType.Inventory, new InventoryPopup.Parameter(InventoryPopup.TabType.Replace));
		}
	}
}
