using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjectView : ObjectView
{
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

	public void SetFieldItem(FieldItem newFieldItem)
	{
		if (currentFieldItem != null)
		{
			if (newFieldItem != null && currentFieldItem.fieldItemCode == newFieldItem.fieldItemCode)
			{
				return;
			}

			currentFieldItem.Destroy();
		}
			
		currentFieldItem = newFieldItem;
		if (currentFieldItem != null)
		{
			originObj = newFieldItem.Create(cameraArm, spawnLocalPos, spawnLocalRot);
			if (originObj != null)
			{
				originObj.SetActive(true);
			}
		}
	}
}
