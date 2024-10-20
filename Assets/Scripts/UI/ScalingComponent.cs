using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingComponent : MonoBehaviour
{
	[SerializeField] private Transform tr;
	[SerializeField] private float scalingTime = 1.0f;
	[SerializeField] private float maxScale = 1.0f;
	[SerializeField] private float minScale = 0.5f;

	private float t = 0.0f;

	private void Start()
	{
		tr.localScale = new Vector3(maxScale, maxScale, maxScale);
	}

	private void Update()
	{
		t += Time.deltaTime / scalingTime;

		int phase = (int)t;

		float nextScale = 0.0f;

		// 올라감
		if (phase % 2 == 1)
		{
			nextScale = Mathf.Lerp(minScale, maxScale, t - phase);
		}
		else
		{
			nextScale = Mathf.Lerp(maxScale, minScale, t - phase);
		}

		tr.localScale = new Vector3(nextScale, nextScale, nextScale);
	}
}
