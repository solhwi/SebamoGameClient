using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
	[SerializeField] private SpriteRenderer view = null;
	[SerializeField] private Camera playerViewCamera = null;
	[SerializeField] private CharacterDataSetter playerOriginPrefab = null;

	private Vector3 characterSpawnLocalPos = new Vector3(0.05f, -0.5f, 10.0f);
	private Vector3 characterSpawnLocalRot = new Vector3(15, 150, -15);

	private Transform originCharacterTransform = null;
	private CharacterAnimationController characterAnimationController = null;

	private RenderTexture renderTexture = null;
	private Texture2D texture = null;
	private Rect rect;

	int height = 256;
	int width = 256;
	int depth = 24;

	private void Start()
	{
		renderTexture = new RenderTexture(width, height, depth);
		rect = new Rect(0, 0, width, height);
		texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

		playerViewCamera.targetTexture = renderTexture;

		var playerOrigin = Instantiate<CharacterDataSetter>(playerOriginPrefab, playerViewCamera.transform);
		playerOrigin.transform.localPosition = characterSpawnLocalPos;
		playerOrigin.transform.localEulerAngles = characterSpawnLocalRot;

		playerOrigin.MakeParts();
		playerOrigin.SetMeshes();
		playerOrigin.SetAvatar();

		characterAnimationController = playerOrigin.GetComponentInChildren<CharacterAnimationController>();

		// 애니메이터가 달려있는 곳이 제어할 위치
		var characterOrigin = playerOrigin.GetComponentInChildren<Animator>();
		originCharacterTransform = characterOrigin.transform;
	}

	private void Update()
	{
		view.sprite = GetScreenShot();
	}

	private Sprite GetScreenShot()
	{
		playerViewCamera.Render();

		RenderTexture.active = renderTexture;

		texture.ReadPixels(rect, 0, 0);
		texture.Apply();

		return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
	}

	public void FlipX(bool flipX)
	{
		view.flipX = flipX;
	}

	public void FlipY(bool flipY)
	{
		float yRot = flipY ? 180 : 0;

		originCharacterTransform.localEulerAngles = new Vector3(0, yRot, 0);
	}

	public void DoIdle()
	{
		characterAnimationController.DoIdle();
	}

	public void DoRun()
	{
		characterAnimationController.DoRun();
	}
}
