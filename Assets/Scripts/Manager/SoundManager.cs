using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
	BGM,
	SFX,
	Max
}

public enum BGMType
{
	None = -1,
	Login,
	Game
}

public enum SFXType
{
	Start,
}

public class SoundManager : Singleton<SoundManager>
{
	[SerializeField] private float bgmFadeTime = 1.0f;

	[SerializeField] private AudioClipContainer clipContainer = null;

	[SerializeField] private AudioSource bgmAudioSource = null;
	[SerializeField] private AudioSource sfxAudioSource = null;

	private Coroutine bgmCoroutine = null;
	private BGMType currentBgmType = BGMType.None;

	public IEnumerator PreLoadSound()
	{
		yield return clipContainer.PreLoadSound();
	}

	protected override void OnAwakeInstance()
	{
		base.OnAwakeInstance();

		bgmAudioSource.loop = true;
		bgmAudioSource.volume = PlayerConfig.bgmSoundValue;
		bgmAudioSource.Play();

		sfxAudioSource.loop = false;
		sfxAudioSource.volume = PlayerConfig.sfxSoundValue;
	}

	public void PlaySFX(SFXType type)
	{
		sfxAudioSource.clip = clipContainer.GetAudioClip(type);
		sfxAudioSource.Play();
	}

	public void PlayBGM(BGMType type, bool useFade)
	{
		if (currentBgmType == type)
			return;

		currentBgmType = type;

		if (bgmCoroutine != null)
		{
			StopCoroutine(bgmCoroutine);
			bgmCoroutine = null;
		}

		if (useFade)
		{	
			bgmCoroutine = StartCoroutine(PlayFadeBGM(type));
		}
		else
		{
			PlayBGM(type);
		}
	}

	private void PlayBGM(BGMType type)
	{
		bgmAudioSource.clip = clipContainer.GetAudioClip(type);
		bgmAudioSource.Play();
	}

	private void Update()
	{
		if (bgmCoroutine != null)
			return;

		bgmAudioSource.volume = PlayerConfig.bgmSoundValue;
		sfxAudioSource.volume = PlayerConfig.sfxSoundValue;
	}

	private IEnumerator PlayFadeBGM(BGMType type)
	{
		yield return FadeOutBGM();

		PlayBGM(type);

		yield return FadeInBGM();

		bgmCoroutine = null;
	}

	private IEnumerator FadeOutBGM()
	{
		float t = bgmFadeTime;
		bgmAudioSource.volume = PlayerConfig.bgmSoundValue;

		while (t >= 0.0f)
		{
			yield return null;

			t -= Time.deltaTime;
			bgmAudioSource.volume = t / bgmFadeTime * PlayerConfig.bgmSoundValue;
		}

		bgmAudioSource.volume = 0.0f;
	}

	private IEnumerator FadeInBGM()
	{
		float t = 0.0f;
		bgmAudioSource.volume = 0.0f;

		while (t <= bgmFadeTime)
		{
			yield return null;

			t += Time.deltaTime;
			bgmAudioSource.volume = t / bgmFadeTime * PlayerConfig.bgmSoundValue;
		}

		bgmAudioSource.volume = PlayerConfig.bgmSoundValue;
	}
}
