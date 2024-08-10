using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjectView : ObjectView
{
	private FieldItem fieldItem = null;

	// 아이템은 기존 생성 로직을 타지 않음
	protected override void InitializeTarget()
	{
		
	}

	public void SetItem(FieldItem fieldItem)
	{
		if (this.fieldItem != null)
		{
			if (this.fieldItem.fieldItemCode != fieldItem.fieldItemCode)
			{
				this.fieldItem.Destroy();
			}
			else
			{
				return;
			}
		}
			
		this.fieldItem = fieldItem;

		originObj = fieldItem.Create(cameraArm, spawnLocalPos, spawnLocalRot);
		if (originObj != null)
		{
			originObj.gameObject.SetActive(true);
		}
	}
}
