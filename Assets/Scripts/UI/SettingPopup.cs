using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : BoardGamePopup
{
	[SerializeField] private Slider bgmSlider;
	[SerializeField] private Slider sfxSlider;

	[SerializeField] private Toggle userNameToggle = null;

	protected override void Reset()
	{
		base.Reset();

		popupType = PopupType.Setting;
	}

	public override void OnOpen(UIParameter parameter = null)
	{
		base.OnOpen(parameter);

		PlayerConfig.Load();

		bgmSlider.value = PlayerConfig.bgmSoundValue;
		sfxSlider.value = PlayerConfig.sfxSoundValue;
		userNameToggle.isOn = PlayerConfig.useOptionUserName;
	}

	public void OnValueChangedBGM()
	{
		PlayerConfig.bgmSoundValue = bgmSlider.value;
	}

	public void OnValueChangedSFX()
	{
		PlayerConfig.sfxSoundValue = sfxSlider.value;
	}

	protected override void OnClose()
	{
		base.OnClose();

		PlayerConfig.bgmSoundValue = bgmSlider.value;
		PlayerConfig.sfxSoundValue = sfxSlider.value;
		PlayerConfig.useOptionUserName = userNameToggle.isOn;

		PlayerConfig.Save();
	}
}
