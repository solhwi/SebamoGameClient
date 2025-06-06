using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IBoardGameSubscriber
{
	public void OnStartTurn();

	public void OnEndTurn();

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, float nextBonusMultiplyCount);
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
		public readonly float bonusMultiplyDiceCount = 1.0f;

		public int CurrentOrder { get; private set; }

		private Queue<StateOrderData> stateOrderQueue = new Queue<StateOrderData>();

		public StateData(int startOrder, int moveNextOrder, int diceCount, int bonusAddDiceCount, float bonusMultiplyDiceCount)
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

	[SerializeField] private AssetReferenceGameObject myPlayerCharacterRef;
	[SerializeField] private AssetReferenceGameObject otherPlayerCharacterRef;

	private Dictionary<(string, string), CharacterComponent> playerCharacterDictionary = new Dictionary<(string, string), CharacterComponent>();

	private List<IBoardGameSubscriber> subscribers = new List<IBoardGameSubscriber>();

	private GameState currentGameState = GameState.None;

	private Dictionary<GameState, Func<IEnumerator>> stateFuncMap = new Dictionary<GameState, Func<IEnumerator>>();
	private Dictionary<GameState, Func<bool>> stateCheckFuncMap = new Dictionary<GameState, Func<bool>>();

	private StateData currentStateData = null;
	private ReplaceFieldItem currentReplaceFieldItem = null;

	protected override void OnAwakeInstance()
	{
		base.OnAwakeInstance();

		stateFuncMap = new Dictionary<GameState, Func<IEnumerator>>()
		{
			{ GameState.None, ProcessNone },
			{ GameState.RollDice, ProcessRollDice },
			{ GameState.MoveCharacter, ProcessMoveCharacter },
			{ GameState.GetItem, ProcessGetItem },
			{ GameState.TileAction, ProcessTileAction }
		};

		stateCheckFuncMap = new Dictionary<GameState, Func<bool>>()
		{
			{ GameState.None, CanNone },
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

	public IEnumerator PreLoadCharacter()
	{
		yield return ResourceManager.Instance.LoadAsync<MyCharacterComponent>(myPlayerCharacterRef);
		yield return ResourceManager.Instance.LoadAsync<CharacterComponent>(otherPlayerCharacterRef);
	}

	public IEnumerator PrepareCharacter()
	{
		playerCharacterDictionary.Clear();

		yield return ResourceManager.Instance.InstantiateAsync<CharacterComponent>(myPlayerCharacterRef, transform, false, (p) =>
		{
			if (p == null)
				return;

			var myPlayerData = PlayerDataContainer.Instance.GetMyPlayerData();
			if (myPlayerData == null)
				return;

			// 플레이어 본인 캐릭터 타일 위로 배치
			int myTileOrder = PlayerDataContainer.Instance.currentTileOrder;
			Vector2 playerPos = TileDataManager.Instance.GetPlayerPosByOrder(myTileOrder);

			p.gameObject.SetActive(true);
			p.SetPlayerData(myPlayerData.playerGroup, myPlayerData.playerName);
			p.SetPosition(playerPos);

#if UNITY_EDITOR
			p.gameObject.name = $"MyPlayer ({PlayerDataContainer.Instance.playerName})";
#endif
			playerCharacterDictionary.Add((myPlayerData.playerGroup, myPlayerData.playerName), p);
		});

		// 타 유저 캐릭터 타일 위로 배치
		if (PlayerDataContainer.Instance.otherPlayerPacketDatas == null)
			yield break;

		foreach (var otherPlayerData in PlayerDataContainer.Instance.otherPlayerPacketDatas)
		{
			yield return ResourceManager.Instance.InstantiateAsync<CharacterComponent>(otherPlayerCharacterRef, transform, false, (p) =>
			{
				if (p == null)
					return;

				int tileOrder = otherPlayerData.playerTileOrder;
				Vector2 playerPos = TileDataManager.Instance.GetPlayerPosByOrder(tileOrder);

				p.gameObject.SetActive(true);
				p.SetPlayerData(otherPlayerData.playerGroup, otherPlayerData.playerName);
				p.SetPosition(playerPos);
#if UNITY_EDITOR
				p.gameObject.name = $"Player ({otherPlayerData.playerName})";
#endif
				playerCharacterDictionary.Add((otherPlayerData.playerGroup, otherPlayerData.playerName), p);
			});
		}
	}

	public void OnClickRollDice()
	{
		if (PlayerDataContainer.Instance.hasDiceCount > 0 || isDiceDebugMode)
		{
			TryChangeState(GameState.RollDice);
		}
	}

	private void TryChangeState(GameState newState, StateData data = null)
	{
		if (stateCheckFuncMap.TryGetValue(newState, out var func))
		{
			if (func != null && func.Invoke())
			{
				Debug.Log($"Change Game State : {currentGameState} > {newState}");

				if (newState == GameState.None)
				{
					foreach (var subscriber in subscribers)
					{
						subscriber?.OnEndTurn();
					}
				}

				currentGameState = newState;
				currentStateData = data;
			}
		}
	}

	private bool CanNone()
	{
		return currentGameState != GameState.None;
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

	public IEnumerator ProcessBoardGame()
	{
		foreach (var playerCharacter in playerCharacterDictionary.Values)
		{
			playerCharacter.gameObject.SetActive(true);
		}

		while (true)
		{
			if (HttpNetworkManager.Instance.IsConnected)
			{
				if (stateFuncMap[currentGameState] != null)
				{
					yield return stateFuncMap[currentGameState].Invoke();
				}
			}

			yield return null;
		}
	}
	
	private IEnumerator ProcessNone()
	{
		yield return null;
	}

	private IEnumerator ProcessRollDice()
	{
		// 내 데이터 불러오기
		bool isMyGetSuccess = false;

		yield return HttpNetworkManager.Instance.TryGetMyPlayerData((d) =>
		{
			isMyGetSuccess = true;
		}, (error) =>
		{
			UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter(error));
		});

		if (isMyGetSuccess == false)
		{
			TryChangeState(GameState.None);
			yield break;
		}

		// 주사위 개수 확인
		if (PlayerDataContainer.Instance.hasDiceCount <= 0 || isDiceDebugMode)
		{
			UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter("주사위 개수가 부족합니다."));
			TryChangeState(GameState.None);
			yield break;
		}

		// 남의 데이터 불러오기
		bool isOtherGetSuccess = false;

		yield return HttpNetworkManager.Instance.TryGetOtherPlayerDatas((d) =>
		{
			isOtherGetSuccess = true;
		}, (error) =>
		{
			UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter(error));
		});

		if (isOtherGetSuccess == false)
		{
			TryChangeState(GameState.None);
			yield break;
		}

		// 타일 데이터 불러오기
		bool isTileGetSuccess = false;

		yield return HttpNetworkManager.Instance.TryGetTileData((d) =>
		{
			isTileGetSuccess = true;
		}, (error) =>
		{
			UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter(error));
		});

		if (isTileGetSuccess == false)
		{
			TryChangeState(GameState.None);
			yield break;
		}

		// 시작 전 노티
		foreach (var subscriber in subscribers)
		{
			subscriber?.OnStartTurn();
		}

		// 버프를 포함한 데이터 세팅
		ProcessData();

		// 서버에 내 정보 동기화
		bool isMyPostSuccess = false;

		yield return HttpNetworkManager.Instance.TryPostMyPlayerData((d) =>
		{
			isMyPostSuccess = true;
		}, (error) =>
		{
			UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter(error));
		});

		if (isMyPostSuccess == false)
		{
			TryChangeState(GameState.None);
			yield break;
		}

		// 서버에 타일 정보 동기화
		bool isTilePostSuccess = false;

		yield return HttpNetworkManager.Instance.TryPostTileData((d) =>
		{
			isTilePostSuccess = true;
		}, (error) =>
		{
			UIManager.Instance.TryOpen(PopupType.Notify, new NotifyPopup.Parameter(error));
		});

		if (isTilePostSuccess == false)
		{
			TryChangeState(GameState.None);
			yield break;
		}

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
		string buffItemCode = Inventory.Instance.GetUsableBuffItemCode();

		var buffItem = BuffItemFactory.Instance.Make(buffItemCode);
		if (buffItem != null)
		{
			buffItem.TryUse();
		}

		if (isDiceDebugMode == false)
		{
			PlayerDataContainer.Instance.UseDice();
		}

		// 주사위 굴리기
		int currentOrder = PlayerDataContainer.Instance.currentTileOrder;
		int diceCount = GetNextDiceCount();

		Inventory.Instance.TryAddItem(ItemTable.Coin, PlayerDataContainer.Instance.AddGoldCountByDiceCount * diceCount);

		int bonusAddDiceCount = PlayerDataContainer.Instance.NextBonusAddDiceCount;
		float bonusMultiplyDiceCount = PlayerDataContainer.Instance.NextBonusMultiplyDiceCount;

		int nextOrder = TileDataManager.Instance.GetNextOrder(currentOrder, (int)(diceCount * bonusMultiplyDiceCount) + bonusAddDiceCount, out var barricadeItem);

		if (barricadeItem != null)
		{
			barricadeItem.Use(nextOrder);
		}

		currentStateData = new StateData(currentOrder, nextOrder, diceCount, bonusAddDiceCount, bonusMultiplyDiceCount);

		PlayerDataContainer.Instance.ClearBonusDiceData();
		PlayerDataContainer.Instance.SaveCurrentOrder(nextOrder);

		// 아이템, 타일 반영
		ProcessItemData();
	}

	private void ProcessItemData()
	{
		int currentOrder = PlayerDataContainer.Instance.currentTileOrder;

		FieldItem item = TileDataManager.Instance.GetCurrentTileItem(currentOrder);
		string itemCode = string.Empty;

		if (item != null)
		{
			itemCode = item.fieldItemCode;
			item.Use(currentOrder);
		}

		int nextOrder = PlayerDataContainer.Instance.currentTileOrder;

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
		int currentOrder = PlayerDataContainer.Instance.currentTileOrder;

		var specialTile = TileDataManager.Instance.GetCurrentSpecialTile(currentOrder);
		if (specialTile != null)
		{
			specialTile.DoAction();
		}

		int nextOrder = PlayerDataContainer.Instance.currentTileOrder;

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
			return PlayerDataContainer.Instance.NextDiceCount;
		}
		else if (PlayerDataContainer.Instance.nextDiceBuffType == NextDiceChangeBuffType.OneOrSix)
		{
			return UnityEngine.Random.Range(1, 3) == 1 ? 1 : 6;
		}
		else if (PlayerDataContainer.Instance.nextDiceBuffType == NextDiceChangeBuffType.Odd)
		{
			return UnityEngine.Random.Range(1, 4) * 2 - 1;
		}
		else if (PlayerDataContainer.Instance.nextDiceBuffType == NextDiceChangeBuffType.Even)
		{
			return UnityEngine.Random.Range(1, 4) * 2;
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

	private  IEnumerator ProcessTileAction()
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
		currentReplaceFieldItem = FieldItemFactory.Instance.Make<ReplaceFieldItem>(fieldItemCode);

		if (currentReplaceFieldItem != null)
		{
			int min = PlayerDataContainer.Instance.currentTileOrder + currentReplaceFieldItem.ranges[0];
			int max = PlayerDataContainer.Instance.currentTileOrder + currentReplaceFieldItem.ranges[1];

			var tileOrders = GetReplaceableTileOrders(currentReplaceFieldItem, min, max);
			TileDataManager.Instance.SetSelectTiles(tileOrders);
		}

		UIManager.Instance.CloseMainCanvas();
		UIManager.Instance.Close(PopupType.Inventory);

		UIManager.Instance.TryOpen(PopupType.BatchMode, new BatchModePopup.Parameter(currentReplaceFieldItem));
	}

	public CharacterComponent GetMyPlayerCharacter()
	{
		string group = PlayerDataContainer.Instance.playerGroup;
		string name = PlayerDataContainer.Instance.playerName;

		return GetPlayerCharacter(group, name);
	}

	public CharacterComponent GetPlayerCharacter(string group, string name)
	{
		var data = PlayerDataContainer.Instance.GetPlayerData(group, name);
		if (data == null)
			return null;

		if (playerCharacterDictionary.TryGetValue((data.playerGroup, data.playerName), out var character) == false)
			return null;

		return character;
	}

	private IEnumerable<int> GetReplaceableTileOrders(ReplaceFieldItem replaceItem, int min, int max)
	{
		if (replaceItem == null)
			yield break;

		min = Mathf.Max(0, min);
		max = Mathf.Min(max, int.MaxValue);

		foreach (int tileOrder in CommonFunc.ToRange(min, max))
		{
			if (replaceItem.IsReplaceable(tileOrder))
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
