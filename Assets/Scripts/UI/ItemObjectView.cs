using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjectView : ObjectView
{
	[SerializeField] private FieldItemFactory fieldItemFactory;
	private FieldItem currentFieldItem = null;

	// 아이템은 기존 생성 로직을 타지 않음
	protected override void InitializeTarget()
	{
		
	}

	public void UnsetFieldItem()
	{
		if (currentFieldItem != null)
		{
			currentFieldItem.Destroy();
		}

		currentFieldItem = null;
	}

	public void SetFieldItem(string currentItemCode)
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
	}
}
