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

	private BuffItem currentBuffItem = null;

	private void Start()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Subscribe(this);
		}
	}

	private void OnDestroy()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Unsubscribe(this);
		}
	}

	private void OnEnable()
	{
		CreateEffect();
	}

	private void OnDisable()
	{
		currentBuffItem?.DestroyEffect();
	}

	public void OnStartTurn()
	{
		currentBuffItem?.DestroyEffect();
	}

	public void OnEndTurn()
	{
		CreateEffect();
	}

	private void CreateEffect()
	{
		if (playerDataContainer.IsEnded)
		{
			foreach (var data in goalEffectDataList)
			{
				ResourceManager.Instance.TryInstantiateAsync<GameObject>(data.path, transform, true, (obj) =>
				{
					obj.transform.SetLocalPositionAndRotation(data.offset, Quaternion.identity);
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
