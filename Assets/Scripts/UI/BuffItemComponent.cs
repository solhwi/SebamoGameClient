using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffItemComponent : MonoBehaviour, IBoardGameSubscriber
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private List<ItemIcon> buffItemIcons = new List<ItemIcon>();

	private bool isUpdateBuffItem = true;

	private void OnEnable()
	{
		BoardGameManager.Instance?.Subscribe(this);
		isUpdateBuffItem = true;
	}

	private void OnDisable()
	{
		BoardGameManager.Instance?.Unsubscribe(this);
		isUpdateBuffItem = false;
	}

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		isUpdateBuffItem = true;
		yield break;
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		isUpdateBuffItem = true;
		yield break;
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount)
	{
		yield break;
	}

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, float nextBonusMultiplyCount)
	{
		isUpdateBuffItem = false;
		yield break;
	}

	private void Update()
	{
		if (isUpdateBuffItem == false)
			return;

		for (int i = 0; i < buffItemIcons.Count; i++)
		{
			if (inventory.appliedBuffItems.Count <= i)
			{
				buffItemIcons[i].gameObject.SetActive(false);
			}
			else
			{
				string buffItemCode = inventory.appliedBuffItems.ElementAt(i);
				buffItemIcons[i].SetBuffItemData(buffItemCode);

				buffItemIcons[i].gameObject.SetActive(true);
			}
		}
	}
}
