using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffItemComponent : MonoBehaviour, IBoardGameSubscriber
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private List<ItemIcon> buffItemIcons = new List<ItemIcon>();

	[SerializeField] private GameObject highLightObj = null;
	[SerializeField] private GameObject noBuffTextObj = null;

	private bool isUpdateBuffItem = true;

	private void Start()
	{
		BoardGameManager.Instance?.Subscribe(this);
		isUpdateBuffItem = true;
	}

	private void OnDestroy()
	{
		BoardGameManager.Instance?.Unsubscribe(this);
		isUpdateBuffItem = false;
	}

	public void OnStartTurn()
	{
		isUpdateBuffItem = false;
	}

	public void OnEndTurn()
	{
		isUpdateBuffItem = true;
	}

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield break;
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		yield break;
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount)
	{
		yield break;
	}

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, float nextBonusMultiplyCount)
	{
		yield break;
	}

	private void Update()
	{
		if (isUpdateBuffItem == false)
			return;

		if (inventory.appliedBuffItemList.Count > 0)
		{
			highLightObj.SetActive(true);
			noBuffTextObj.SetActive(false);
		}
		else
		{
			highLightObj.SetActive(false);
			noBuffTextObj.SetActive(true);
		}

		for (int i = 0; i < buffItemIcons.Count; i++)
		{
			if (inventory.appliedBuffItemList.Count <= i)
			{
				buffItemIcons[i].gameObject.SetActive(false);
			}
			else
			{
				string buffItemCode = inventory.appliedBuffItemList.ElementAt(i);
				buffItemIcons[i].SetBuffItemData(buffItemCode);

				buffItemIcons[i].gameObject.SetActive(true);
			}
		}
	}
}
