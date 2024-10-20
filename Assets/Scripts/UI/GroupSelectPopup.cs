using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSelectPopup : BoardGamePopup
{
	public class Param : UIParameter
	{
		public readonly List<AuthData> authDataList = new List<AuthData>();
		public readonly Action<AuthData> onAuthSelected = null;

		public Param(List<AuthData> authDataList, Action<AuthData> onAuthSelected)
		{
			this.onAuthSelected = onAuthSelected;
			this.authDataList = authDataList;
		}
	}

	[SerializeField] private ScrollContent groupScrollContent = null;

	private List<AuthData> authDataList = new List<AuthData>();
	private Action<AuthData> onAuthSelected = null;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.GroupSelect;
	}

	public override void OnOpen(UIParameter param)
	{
		base.OnOpen(param);

		if (param is Param p)
		{
			authDataList.Clear();
			authDataList.AddRange(p.authDataList);

			onAuthSelected = p.onAuthSelected;

			groupScrollContent.onUpdateContents += OnUpdateContents;
			groupScrollContent.onGetItemCount += OnGetItemCount;

			groupScrollContent.UpdateContents();
		}
	}

	protected override void OnClose()
	{
		base.OnClose();

		groupScrollContent.onUpdateContents -= OnUpdateContents;
		groupScrollContent.onGetItemCount -= OnGetItemCount;
	}

	private void OnUpdateContents(int index, GameObject contentsObj)
	{
		if (index < 0 || authDataList.Count <= index)
			return;

		if (contentsObj == null)
			return;

		var groupSelectScrollItem = contentsObj.GetComponent<GroupSelectScrollItem>();
		if (groupSelectScrollItem == null)
			return;

		var authGroup = authDataList[index];
		if (authGroup == null)
			return;

		groupSelectScrollItem.SetData(authGroup);
		groupSelectScrollItem.SetItemClick(OnAuthSuccess);
	}

	private int OnGetItemCount(int tabType)
	{
		return authDataList.Count;
	}

	private void OnAuthSuccess(AuthData authData)
	{
		onAuthSelected?.Invoke(authData);
		OnClickClose();
	}

}
