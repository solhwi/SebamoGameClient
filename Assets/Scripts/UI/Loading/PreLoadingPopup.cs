using System;
using System.Collections;
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
	All,
	Max
}

[System.Serializable]
public class LogoData
{
	[SerializeField] private CanvasGroup logoImage;
	[SerializeField] private float fadeInTime = 1.0f;
	[SerializeField] private float fadeWaitingTime = 1.0f;
	[SerializeField] private float fadeOutTime = 1.0f;

	public IEnumerator FadeIn(float deltaTime)
	{
		float t = 0.0f;

		logoImage.alpha = t;

		while (t < fadeInTime)
		{
			yield return null;
			t += deltaTime;

			logoImage.alpha = t / fadeInTime;
		}

		logoImage.alpha = 1.0f;
	}

	public IEnumerator FadeWaiting(float deltaTime)
	{
		float t = 0.0f;

		while (t < fadeWaitingTime)
		{
			yield return null;
			t += deltaTime;
		}
	}

	public IEnumerator FadeOut(float deltaTime)
	{
		float t = fadeOutTime;

		logoImage.alpha = 1.0f;

		while (t > 0.0f)
		{
			yield return null;
			t -= deltaTime;

			logoImage.alpha = t / fadeOutTime;
		}

		logoImage.alpha = 0f;
	}
}

public class PreLoadingPopup : BoardGamePopup
{
	[SerializeField] private LogoData[] logoDatas = new LogoData[(int)LogoType.Max];
	[SerializeField] private WaitingText descriptionText = null;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.PreLoading;
	}
	public IEnumerator FadeRoutine(LogoType logoType)
	{
		yield return FadeInRoutine(logoType);
		yield return FadeWaitingRoutine(logoType);
		yield return FadeOutRoutine(logoType);
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

	public IEnumerator FadeInRoutine(LogoType logoType)
	{
		var logoData = logoDatas[(int)logoType];
		yield return logoData.FadeIn(Time.deltaTime);
	}

	public IEnumerator FadeWaitingRoutine(LogoType logoType)
	{
		var logoData = logoDatas[(int)logoType];
		yield return logoData.FadeWaiting(Time.deltaTime);
	}

	public IEnumerator FadeOutRoutine(LogoType logoType)
	{
		var logoData = logoDatas[(int)logoType];
		yield return logoData.FadeOut(Time.deltaTime);
	}
}
