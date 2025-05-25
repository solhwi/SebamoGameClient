
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "TeleportSpecialTile", menuName = "2D/Tiles/Teleport Special Tile")]
public class TeleportSpecialTile : SpecialTileBase
{
	[SerializeField] private AssetReference teleportPrefab = null;
	[SerializeField] private float teleportStartDelay = 1.0f;
	[SerializeField] private float teleportCharacterDelay = 0.25f;
	[SerializeField] private float effectWaitTime = 2.0f;
	[SerializeField] private int count;

	private GameObject effectObj;

	protected override void Reset()
	{
		base.Reset();

		specialTileType = SpecialTileType.Teleport;
	}

	public override void DoAction()
	{
		PlayerDataContainer.Instance.AddCurrentOrder(count);
	}

	public override IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		yield return null;
	}

	public IEnumerator DepartEffect(Transform owner)
	{
		yield return new WaitForSeconds(teleportStartDelay);

		yield return ResourceManager.Instance.InstantiateAsync<GameObject>(teleportPrefab, null, true, (obj) =>
		{
			effectObj = obj;
			effectObj.transform.position = owner.transform.position;
		});

		yield return new WaitForSeconds(teleportCharacterDelay);

		owner.gameObject.SetActive(false);

		yield return new WaitForSeconds(effectWaitTime);
	}

	public IEnumerator ArriveEffect(Transform owner)
	{
		owner.gameObject.SetActive(true);

		yield return ResourceManager.Instance.InstantiateAsync<GameObject>(teleportPrefab, null, true, (obj) =>
		{
			effectObj = obj;
			effectObj.transform.position = owner.transform.position;
		});

		yield return new WaitForSeconds(effectWaitTime);
	}

	public void DestroyEffect()
	{
		if (effectObj != null)
		{
			ObjectManager.Instance.Destroy(effectObj);
		}
	}
}
