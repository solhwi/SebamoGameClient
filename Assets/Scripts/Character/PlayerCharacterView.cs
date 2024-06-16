using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterView : MonoBehaviour
{
	[SerializeField] private SpriteRenderer view = null;
	[SerializeField] private Camera playerViewCamera = null;

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
}
