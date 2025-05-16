using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupSelectScrollItem : MonoBehaviour
{
	[SerializeField] private Text groupText;

	private AuthData authData = null;
	private Action<AuthData> onClick;

	public void SetData(AuthData authData)
	{
		this.authData = authData;

		groupText.text = authData.group;
	}

	public void SetItemClick(Action<AuthData> onClick)
	{
		this.onClick = onClick;
	}

	public void OnClick()
	{
		onClick?.Invoke(authData);
	}
}
