using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePanel : MonoBehaviour
{
	[SerializeField] private RectTransform panel;
	[SerializeField] private bool isResetToggleOn = false;
	[SerializeField] private float offPos = 0.0f;
	[SerializeField] private float onPos = 0.0f;
	[SerializeField] private bool isY = false;

	private bool isToggleOn = false;

	private void Start()
	{
		SetToggle(isResetToggleOn);
	}

	public void SetToggle(bool isOn)
	{
		isToggleOn = isOn;

		if (isY)
		{
			if (isOn)
			{
				panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, onPos);
			}
			else
			{
				panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, offPos);
			}
		}
		else
		{
			if (isOn)
			{
				panel.anchoredPosition = new Vector2(onPos, panel.anchoredPosition.y);
			}
			else
			{
				panel.anchoredPosition = new Vector2(offPos, panel.anchoredPosition.y);
			}
		}

	}

	public void OnClickToggle()
	{
		SetToggle(!isToggleOn);
	}
}
