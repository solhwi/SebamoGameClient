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

public enum LogoType
{
	UnityChan,
	Sebamo,
	Max
}

public class LoadingCanvas : BoardGameCanvasBase
{
	[SerializeField] private Image[] logoImages = new Image[(int)LogoType.Max];
	[SerializeField] private WaitingText descriptionText = null;

	[SerializeField] private float fadeInTime = 1.0f;
	[SerializeField] private float fadeOutTime = 1.0f;

	private Coroutine fadeCoroutine = null;
	public event Func<bool> barrierFunc = null;

	protected override void OnOpen()
	{
		base.OnOpen();

		foreach (var logo in logoImages)
		{
			logo.color = logo.color.Alpha(0);
		}
	}

	public IEnumerator FadeRoutine(LogoType logoType)
	{
		yield return FadeInRoutine(logoType);

		if (barrierFunc != null)
		{
			while (barrierFunc() == false)
			{
				yield return null;
			}
		}
		
		yield return FadeOutRoutine(logoType);
	}

	public void StartFadeRoutine(LogoType logoType)
	{
		fadeCoroutine = StartCoroutine(FadeRoutine(logoType));
	}

	public void SetWaitDescription(string description, float progress)
	{
		descriptionText.StartWaiting($"{description} ({progress})");
	}

	public void SetWaitDescription(string description)
	{
		descriptionText.StartWaiting($"{description}");
	}

	public void SetDescription(string description, float progress)
	{
		descriptionText.SetText($"{description} ({progress})");
	}

	public void SetDescription(string description)
	{
		descriptionText.SetText(description);
	}

	private IEnumerator FadeInRoutine(LogoType logoType)
	{
		float t = 0.0f;

		Image logoImage = logoImages[(int)logoType];
		logoImage.color = logoImage.color.Alpha(t);

		while (t < fadeInTime)
		{
			yield return null;
			t += Time.deltaTime;

			logoImage.color = logoImage.color.Alpha(t / fadeInTime);
		}

		logoImage.color = logoImage.color.Alpha(1.0f);
	}

	private IEnumerator FadeOutRoutine(LogoType logoType)
	{
		float t = fadeOutTime;

		Image logoImage = logoImages[(int)logoType];
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
