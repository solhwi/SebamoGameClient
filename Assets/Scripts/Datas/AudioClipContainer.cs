using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AudioClipContainer")]
public class AudioClipContainer : DataContainer<AudioClipContainer>
{
	[System.Serializable]
	public class BGMAudioDictionary : SerializableDictionary<BGMType, AssetReferenceT<AudioClip>> { }
	[SerializeField] private BGMAudioDictionary bgmAudioClipDictionary = new BGMAudioDictionary();

	[System.Serializable]
	public class SFXAudioDictionary : SerializableDictionary<SFXType, AssetReferenceT<AudioClip>> { }
	[SerializeField] private SFXAudioDictionary sfxAudioClipDictionary = new SFXAudioDictionary();


	public override IEnumerator Preload()
	{
		foreach (var clipRef in bgmAudioClipDictionary.Values)
		{
			yield return ResourceManager.Instance.LoadAsync<AudioClip>(clipRef);
		}

		foreach (var clipRef in sfxAudioClipDictionary.Values)
		{
			yield return ResourceManager.Instance.LoadAsync<AudioClip>(clipRef);
		}
	}

	public AudioClip GetAudioClip(BGMType bgmType)
	{
		if (bgmAudioClipDictionary.TryGetValue(bgmType, out var asset))
		{
			return ResourceManager.Instance.Load<AudioClip>(asset);
		}

		return null;
	}

	public AudioClip GetAudioClip(SFXType sfxType)
	{
		if (sfxAudioClipDictionary.TryGetValue(sfxType, out var asset))
		{
			return ResourceManager.Instance.Load<AudioClip>(asset);
		}

		return null;
	}
}
