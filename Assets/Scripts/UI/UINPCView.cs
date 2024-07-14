using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectView : MonoBehaviour
{
	[SerializeField] protected Camera cam = null;
	[SerializeField] private GameObject originPrefab = null;

	protected GameObject originObj;

	protected Vector3 spawnLocalPos = new Vector3(0, -0.5f, 10);
	protected Vector3 spawnLocalRot = new Vector3(0, 180, 0);

	protected SpriteRenderer spriteView = null;
	protected RawImage textureView = null;

	private RenderTexture renderTexture = null;
	private Texture2D texture = null;
	private Rect rect;

	int height = 1024;
	int width = 1024;
	int depth = 24;

	protected virtual void Awake()
	{
		spriteView = GetComponent<SpriteRenderer>();
		textureView = GetComponent<RawImage>();
	}

	protected virtual void Start()
	{
		renderTexture = new RenderTexture(width, height, depth);
		rect = new Rect(0, 0, width, height);
		texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

		cam.targetTexture = renderTexture;

		originObj = Instantiate(originPrefab, cam.transform);
		originObj.transform.localPosition = spawnLocalPos;
		originObj.transform.localEulerAngles = spawnLocalRot;
	}

	private void Update()
	{
		if (spriteView != null)
		{
			spriteView.sprite = GetScreenShotSprite();
		}
		else if(textureView != null)
		{
			textureView.texture = GetScreenShotTexture();
		}
	}

	private Texture2D GetScreenShotTexture()
	{
		cam.Render();

		RenderTexture.active = renderTexture;

		texture.ReadPixels(rect, 0, 0);
		texture.Apply();

		return texture;
	}

	private Sprite GetScreenShotSprite()
	{
		cam.Render();

		RenderTexture.active = renderTexture;

		texture.ReadPixels(rect, 0, 0);
		texture.Apply();

		return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
	}
}

public class UINPCView : ObjectView
{
	private Animator animator = null;

	protected override void Start()
	{
		spawnLocalPos = new Vector3(0, -1f, 10);

		base.Start();

		animator = originObj.GetComponent<Animator>();
		animator.Play("POSE14");
	}
}
