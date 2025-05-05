using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffItemComponent : MonoBehaviour, IBoardGameSubscriber
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private List<ItemIcon> buffItemIcons = new List<ItemIcon>();

	[SerializeField] private GameObject highLightObj = null;

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

		highLightObj.SetActive(inventory.appliedBuffItems.Count > 0);

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
