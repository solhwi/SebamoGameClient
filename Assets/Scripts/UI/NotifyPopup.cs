using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NotifyPopup : BoardGamePopup
{
	public class Parameter : UIParameter
	{
		public readonly Action onClickConfirm = null;

		public readonly string descriptionStr = string.Empty;
		public readonly string titleStr = string.Empty;

		public Parameter(string descriptionStr, string titleStr = "알림", Action onClickConfirm = null)
		{
			this.descriptionStr = descriptionStr;
			this.titleStr = titleStr;
			this.onClickConfirm = onClickConfirm;
		}
	}

	[SerializeField] private Text titleText = null;
	[SerializeField] private Text descriptionText = null;

	private Action onClickConfirm = null;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Notify;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		if (parameter is Parameter p)
		{
			titleText.text = p.titleStr;
			descriptionText.text = p.descriptionStr;
			onClickConfirm = p.onClickConfirm;
		}
	}

	public void OnClickConfirm()
	{
		onClickConfirm?.Invoke();
		OnClickClose();
	}

}
