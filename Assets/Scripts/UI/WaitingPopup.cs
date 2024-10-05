using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPopup : BoardGamePopup
{
	public enum Type
	{
		Scene,
		Network,
		Max
	}

	public class Parameter : UIParameter
	{
		public readonly Type type;

		public Parameter(Type type)
		{
			this.type = type;
		}

		public bool IsUsingBackground()
		{
			switch (type)
			{
				case Type.Scene:
					return true;
			}

			return false;
		}

		public string GetWaitingText()
		{
			switch (type)
			{
				case Type.Scene:
					return "Now Loading";

				case Type.Network:
					return "서버에 접속 중";
			}

			return string.Empty;
		}
	}

	[SerializeField] private Image backgroundImage;

	[SerializeField] private GameObject[] spinnerObjs = new GameObject[(int)Type.Max];
	[SerializeField] private WaitingText[] waitingTexts = new WaitingText[(int)Type.Max];

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Wait;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		backgroundImage.color = backgroundImage.color.Alpha(0);

		for (int i = 0; i < (int)Type.Max; i++)
		{
			waitingTexts[i].gameObject.SetActive(false);
			spinnerObjs[i].SetActive(false);
		}

		if (parameter is Parameter p)
		{
			backgroundImage.color = backgroundImage.color.Alpha(p.IsUsingBackground() ? 1 : 0);

			waitingTexts[(int)p.type].gameObject.SetActive(true);
			waitingTexts[(int)p.type].StartWaiting(p.GetWaitingText());

			spinnerObjs[(int)p.type].SetActive(true);
		}
	}

	protected override void OnClose()
	{
		base.OnClose();

		backgroundImage.color = backgroundImage.color.Alpha(0);

		foreach (var obj in spinnerObjs)
		{
			obj.SetActive(false);
		}

		foreach (var t in waitingTexts)
		{
			t.gameObject.SetActive(false);
		}
	}

}
