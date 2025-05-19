using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHeadOnUI : MonoBehaviour
{
	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private Canvas canvas;
	[SerializeField] private Text characterNameText;
	[SerializeField] private Image characterArrowImage;

	private string playerGroup = string.Empty;
	private string playerName = string.Empty;

	private void Reset()
	{
		canvas = GetComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
	}

	private void Awake()
	{
		canvas.sortingOrder = (int)UIManager.CanvasGroup.HeadOnUI;
	}

	public void TrySetActive(bool isActive)
	{
		bool useOption = PlayerConfig.useOptionUserName;
		bool isMine = playerDataContainer.IsMine(playerGroup, playerName);

		gameObject.SetActive(isActive && (isMine || useOption));
	}

	public void Initialize()
	{
		canvas.worldCamera = Camera.main;
	}

	public void SetPlayerData(string playerGroup, string playerName)
	{
		this.playerGroup = playerGroup;
		this.playerName = playerName;

		characterNameText.text = playerName;

		bool isMine = playerDataContainer.IsMine(playerGroup, playerName);

		characterNameText.gameObject.SetActive(!isMine);
		characterArrowImage.gameObject.SetActive(isMine);
	}
}
