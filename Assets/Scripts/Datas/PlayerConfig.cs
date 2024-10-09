using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig
{
	public static float bgmSoundValue;
	public static float sfxSoundValue;
	public static bool useOptionUserName;

	public static void Save()
	{
		PlayerPrefs.SetFloat("bgmSoundValue", bgmSoundValue);
		PlayerPrefs.SetFloat("sfxSoundValue", sfxSoundValue);
		SetBool("useOptionUserName", useOptionUserName);
	}

	public static void Load()
	{
		bgmSoundValue = PlayerPrefs.GetFloat("bgmSoundValue");
		sfxSoundValue = PlayerPrefs.GetFloat("sfxSoundValue");
		useOptionUserName = GetBool("useOptionUserName");
	}

	public static bool GetBool(string key)
	{
		return PlayerPrefs.GetInt(key) > 0;
	}

	public static void SetBool(string key, bool value)
	{
		PlayerPrefs.SetInt(key, value ? 1 : 0);
	}
}
