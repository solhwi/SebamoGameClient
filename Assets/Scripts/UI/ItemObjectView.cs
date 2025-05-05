using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjectView : ObjectView
{
	[SerializeField] private FieldItemFactory fieldItemFactory;
	[SerializeField] private ItemIcon itemIcon = null;

	private FieldItem currentFieldItem = null;

	// 아이템은 기존 생성 로직을 타지 않음
	protected override IEnumerator OnPrepareRendering()
	{
		yield return null;
	}

	public void UnsetItem()
	{
		if (currentFieldItem != null)
		{
			currentFieldItem.Destroy();
		}

		currentFieldItem = null;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		isVisible = true;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		isVisible = false;
	}

	public void SetItem(string currentItemCode)
	{
		if (currentFieldItem != null)
		{
			if (currentItemCode != string.Empty && currentFieldItem.fieldItemCode == currentItemCode)
			{
				return;
			}

			currentFieldItem.Destroy();
		}

		currentFieldItem = fieldItemFactory.Make(currentItemCode);
		if (currentFieldItem != null)
		{
			originObj = currentFieldItem.Create(cameraArm, spawnLocalPos, spawnLocalRot);
			if (originObj != null)
			{
				originObj.SetActive(true);
			}
		}

		itemIcon.SetItemData(currentItemCode);
		itemIcon.SetActiveImage(currentFieldItem == null);
	}
}
