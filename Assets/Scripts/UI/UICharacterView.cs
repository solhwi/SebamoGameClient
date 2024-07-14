using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterView : MonoBehaviour
{
	[SerializeField] private RawImage view = null;
	[SerializeField] private Camera playerViewCamera = null;
	[SerializeField] private CharacterDataSetter playerOriginPrefab = null;

	private CharacterDataSetter characterDataSetter = null;

	private Vector3 characterSpawnLocalPos = new Vector3(0, -0.5f, 10);
	private Vector3 characterSpawnLocalRot = new Vector3(0, 180, 0);

	private CharacterAnimationController characterAnimationController = null;

	private RenderTexture renderTexture = null;
	private Texture2D texture = null;
	private Rect rect;

	int height = 1024;
	int width = 1024;
	int depth = 24;

	private void Start()
	{
		renderTexture = new RenderTexture(width, height, depth);
		rect = new Rect(0, 0, width, height);
		texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

		playerViewCamera.targetTexture = renderTexture;

		characterDataSetter = Instantiate(playerOriginPrefab, playerViewCamera.transform);
		characterDataSetter.transform.localPosition = characterSpawnLocalPos;
		characterDataSetter.transform.localEulerAngles = characterSpawnLocalRot;

		characterDataSetter.MakeParts();
		characterDataSetter.SetMeshes();
		characterDataSetter.SetAvatar();

		characterAnimationController = characterDataSetter.GetComponentInChildren<CharacterAnimationController>();
	}

	private void Update()
	{
		view.texture = GetScreenShot();
	}

	private Texture2D GetScreenShot()
	{
		playerViewCamera.Render();

		RenderTexture.active = renderTexture;

		texture.ReadPixels(rect, 0, 0);
		texture.Apply();

		return texture;
	}

	public void DoIdle()
	{
		characterAnimationController.DoIdle();
	}

	public void DoRun()
	{
		characterAnimationController.DoRun();
	}

	// 임의로 입혀볼 수 있는 함수 추가 예정
}
