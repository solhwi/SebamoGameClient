using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectView : MonoBehaviour
{
	[SerializeField] private GameObject originPrefab = null;

	[SerializeField] protected Vector3 spawnLocalPos = new Vector3(0, -0.5f, 0);
	[SerializeField] protected Vector3 spawnLocalRot = new Vector3(0, 180, 0);

	[SerializeField] private bool isFixedPosition = false;
	[SerializeField] private bool isFixedRotation = false;

	[SerializeField] private bool isOrthoSize = false;
	[SerializeField] private float FOV = 10.0f;

	protected Camera cam = null;
	protected Transform cameraArm = null;

	protected Vector3 initialCameraArmRot = Vector3.zero;

	protected GameObject originObj;

	protected SpriteRenderer spriteView = null;
	protected RawImage textureView = null;

	private RenderTexture renderTexture = null;
	private Texture2D texture = null;
	private Rect rect;

	int height = 512;
	int width = 512;
	int depth = 24;

	protected virtual void Awake()
	{
		spriteView = GetComponent<SpriteRenderer>();
		textureView = GetComponent<RawImage>();

		cam = ObjectCameraManager.Instance.MakeCamera(isOrthoSize, FOV);
		cameraArm = cam.transform.GetChild(0);

		initialCameraArmRot = cameraArm.transform.localEulerAngles;
	}

	protected virtual void Start()
	{
		renderTexture = new RenderTexture(width, height, depth);
		rect = new Rect(0, 0, width, height);
		texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

		cam.targetTexture = renderTexture;

		InitializeTarget();
	}

	protected virtual void OnEnable()
	{
		cam.gameObject.SetActive(true);
	}

	protected virtual void OnDisable()
	{
		cam.gameObject.SetActive(false);
	}

	protected virtual void InitializeTarget()
	{
		originObj = Instantiate(originPrefab, cameraArm);
		originObj.transform.localPosition = spawnLocalPos;
		originObj.transform.localEulerAngles = spawnLocalRot;
	}

	protected virtual void Update()
	{
		if (originObj == null)
			return;

		if (cam.gameObject.activeSelf == false)
			return;

		if (spriteView != null)
		{
			spriteView.color = Color.white;
			spriteView.sprite = GetScreenShotSprite();
		}
		else if(textureView != null)
		{
			textureView.color = Color.white;
			textureView.texture = GetScreenShotTexture();
		}
	}

	private void LateUpdate()
	{
		if (originObj == null)
			return;

		if (isFixedPosition)
		{
			originObj.transform.localPosition = spawnLocalPos;
		}
		if (isFixedRotation)
		{
			originObj.transform.localEulerAngles = spawnLocalRot;
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