using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class ColorExtension
{
	public static Color Alpha(this Color color, float alpha)
	{
		color.a = alpha;
		return color;
	}
}

public class LoadingCanvas : BoardGameCanvasBase
{
	[SerializeField] private Image logoImage = null;
	[SerializeField] private WaitingText descriptionText = null;

	[SerializeField] private float fadeInTime = 1.0f;
	[SerializeField] private float fadeOutTime = 1.0f;

	private Coroutine fadeCoroutine = null;
	public event Func<bool> barrierFunc = null;

	protected override void OnOpen()
	{
		base.OnOpen();
		logoImage.color = new Color(1, 1, 1, 0);

		fadeCoroutine = StartCoroutine(FadeRoutine());
	}

	private IEnumerator FadeRoutine()
	{
		yield return FadeInRoutine();

		if (barrierFunc != null)
		{
			while (barrierFunc() == false)
			{
				yield return null;
			}
		}
		
		yield return FadeOutRoutine();
	}

	public void SetWaitDescription(string description)
	{
		descriptionText.StartWaiting(description);
	}

	public void SetDescription(string description)
	{
		descriptionText.SetText(description);
	}

	private IEnumerator FadeInRoutine()
	{
		float t = 0.0f;
		logoImage.color = logoImage.color.Alpha(t);

		while (t < fadeInTime)
		{
			yield return null;
			t += Time.deltaTime;

			logoImage.color = logoImage.color.Alpha(t / fadeInTime);
		}

		logoImage.color = logoImage.color.Alpha(1.0f);
	}

	private IEnumerator FadeOutRoutine()
	{
		float t = fadeOutTime;
		logoImage.color = logoImage.color.Alpha(t);

		while (t > 0.0f)
		{
			yield return null;
			t -= Time.deltaTime;

			logoImage.color = logoImage.color.Alpha(t / fadeOutTime);
		}

		logoImage.color = logoImage.color.Alpha(0f);
	}

	protected override void OnClose()
	{
		base.OnClose();

		if (fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
		}
	}
}
