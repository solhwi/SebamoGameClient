using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PressEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private float pressCheckTime;

	private bool isProgressing = false;
	private float pressingTime = 0.0f;
	public event Action<float> onEndPress = null;

	private void Update()
	{
		if (isProgressing == false)
			return;

		pressingTime += Time.deltaTime;
		if (pressingTime > pressCheckTime)
		{
			onEndPress?.Invoke(pressCheckTime);
			pressingTime = 0.0f;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isProgressing = true;
		pressingTime = 0.0f;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isProgressing = false;
		pressingTime = 0.0f;
	}
}
