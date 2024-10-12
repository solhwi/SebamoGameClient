using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;



public class BackGroundCanvas : MonoBehaviour, IBoardGameSubscriber
{
	[SerializeField] private RawImage backGroundImage = null;

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileDataContainer tileDataContainer;

	private void Start()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Subscribe(this);
		}

		SetBackGround(playerDataContainer.currentTileOrder);
	}

	private void OnDestroy()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Unsubscribe(this);
		}
	}

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, int nextBonusMultiplyCount)
	{
		yield return null;
	}

	private void SetBackGround(int currentOrder)
	{
		backGroundImage.texture = tileDataContainer.GetBackGroundResource(currentOrder);
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount)
	{
		yield return null;
		SetBackGround(playerDataContainer.currentTileOrder);
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		yield return null;
		SetBackGround(playerDataContainer.currentTileOrder);
	}

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
		SetBackGround(playerDataContainer.currentTileOrder);
	}
}
