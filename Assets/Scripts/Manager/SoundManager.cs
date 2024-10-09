using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
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

	[SerializeField] private float bgmFadeTime = 1.0f;

	[SerializeField] private AudioSource bgmAudioSource = null;
	[SerializeField] private AudioSource sfxAudioSource = null;

	[System.Serializable]
	public class BGMAudioDictionary : SerializableDictionary<BGMType, AudioClip> { }
	[SerializeField] private BGMAudioDictionary bgmAudioClipDictionary = new BGMAudioDictionary();

	[System.Serializable]
	public class SFXAudioDictionary : SerializableDictionary<SFXType, AudioClip> { }
	[SerializeField] private SFXAudioDictionary sfxAudioClipDictionary = new SFXAudioDictionary();

	private Coroutine bgmCoroutine = null;
	private BGMType currentBgmType = BGMType.None;

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
		if (sfxAudioClipDictionary.TryGetValue(type, out var clip))
		{
			sfxAudioSource.clip = clip;
			sfxAudioSource.Play();
		}
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
		if (bgmAudioClipDictionary.TryGetValue(type, out var clip))
		{
			bgmAudioSource.clip = clip;
			bgmAudioSource.Play();
		}
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
