using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectData
{
	public string path;
	public Vector3 offset;
}

public class CharacterEffectComponent : MonoBehaviour, IBoardGameSubscriber
{
	[SerializeField] private PlayerDataContainer playerDataContainer = null;

	[SerializeField] private Inventory inventory;
	[SerializeField] private BuffItemFactory buffItemFactory;

	[SerializeField] protected CharacterView characterView = null;

	[SerializeField] private List<EffectData> goalEffectDataList = new List<EffectData>();

	private List<GameObject> goalEffectObjs = new List<GameObject>();
	private BuffItem currentBuffItem = null;

	private void Start()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Subscribe(this);
		}

		CreateEffect();
	}

	private void OnDestroy()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Unsubscribe(this);
		}

		DestroyEffect();
	}

	public void OnStartTurn()
	{
		DestroyEffect();
	}

	private void DestroyEffect()
	{
		foreach (var obj in goalEffectObjs)
		{
			ObjectManager.Instance?.Destroy(obj);
		}
		currentBuffItem?.DestroyEffect();
	}

	public void OnEndTurn()
	{
		CreateEffect();
	}

	private void CreateEffect()
	{
		DestroyEffect();

		if (playerDataContainer.IsMeEnded)
		{
			foreach (var data in goalEffectDataList)
			{
				ResourceManager.Instance.TryInstantiateAsync<GameObject>(data.path, transform, true, (obj) =>
				{
					obj.transform.SetLocalPositionAndRotation(data.offset, Quaternion.identity);

					if (goalEffectObjs.Contains(obj) == false)
					{
						goalEffectObjs.Add(obj);
					}
				});
			}
		}
		else
		{
			string buffItemCode = inventory.GetUsableBuffItemCode();

			currentBuffItem = buffItemFactory.Make(buffItemCode);
			if (currentBuffItem != null)
			{
				currentBuffItem.CreateEffect(characterView.originCharacterTransform);
			}
		}
	}

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, float nextBonusMultiplyCount)
	{
		yield break;
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount)
	{
		yield break;
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		yield break;
	}

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield break;
	}
}
