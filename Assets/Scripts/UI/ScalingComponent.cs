using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingComponent : MonoBehaviour
{
	[SerializeField] private Transform tr;
	[SerializeField] private float scalingTime = 1.0f;
	[SerializeField] private float maxScale = 1.0f;
	[SerializeField] private float minScale = 0.5f;
	[SerializeField] private bool isMaxStart = false;
	[SerializeField] private int scalingLoopCount = -1;

	private float currentTime = 0.0f;

	private void Start()
	{
		if (isMaxStart)
		{
			tr.localScale = new Vector3(maxScale, maxScale, maxScale);
		}
		else
		{
			tr.localScale = new Vector3(minScale, minScale, minScale);
		}
	}

	private void Update()
	{
		currentTime += Time.deltaTime / scalingTime;

		int loopCount = (int)currentTime;

		if (scalingLoopCount < 0 || loopCount < scalingLoopCount)
		{
			tr.localScale = GetNextScale(currentTime, loopCount);
		}
	}

	private bool IsUpScaleTime(int loopCount)
	{
		if (isMaxStart && loopCount % 2 == 1)
			return true;

		if (isMaxStart == false && loopCount % 2 == 0)
			return true;

		return false;
	}

	private Vector3 GetNextScale(float currentTime, int loopCount)
	{
		float nextScale = 0.0f;
		float normalizedTime = currentTime - loopCount;

		if (IsUpScaleTime(loopCount))
		{
			nextScale = Mathf.Lerp(minScale, maxScale, normalizedTime);
		}
		else
		{
			nextScale = Mathf.Lerp(maxScale, minScale, normalizedTime);
		}

		return new Vector3(nextScale, nextScale, nextScale);
	}
}
