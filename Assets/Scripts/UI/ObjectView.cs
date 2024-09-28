using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ObjectView : MonoBehaviour
{
	[SerializeField] private AssetReferenceGameObject originPrefab = null;

	[SerializeField] protected Vector3 spawnLocalPos = new Vector3(0, -0.5f, 0);
	[SerializeField] protected Vector3 spawnLocalRot = new Vector3(0, 180, 0);

	[SerializeField] private bool isFixedPosition = false;
	[SerializeField] private bool isFixedRotation = false;

	[SerializeField] private bool isOrthoSize = false;
	[SerializeField] private float FOV = 10.0f;

	[SerializeField] private int height = 512;
	[SerializeField] private int width = 512;
	[SerializeField] private int depth = 32;

	protected Camera cam = null;
	protected Transform cameraArm = null;

	protected Vector3 initialCameraArmRot = Vector3.zero;

	protected GameObject originObj;

	protected SpriteRenderer spriteView = null;
	protected MeshRenderer2D meshView = null;
	protected RawImage textureView = null;

	private RenderTexture renderTexture = null;
	private Texture2D texture = null;
	private Rect rect;

	private bool isInitialized = false;
	private Coroutine prepareRoutine = null;

	public virtual void Initialize()
	{
		if (!isInitialized)
		{
			spriteView = GetComponent<SpriteRenderer>();
			textureView = GetComponent<RawImage>();
			meshView = GetComponent<MeshRenderer2D>();

			renderTexture = new RenderTexture(width, height, depth);
			rect = new Rect(0, 0, width, height);
			texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

			cam = ObjectCameraManager.Instance.MakeCamera(isOrthoSize, FOV);
			cameraArm = cam.transform.GetChild(0);
			cam.targetTexture = renderTexture;

			initialCameraArmRot = cameraArm.transform.localEulerAngles;

			isInitialized = true;
		}

		gameObject.SetActive(true);

		if (originObj == null)
		{
			if (prepareRoutine != null)
			{
				StopCoroutine(prepareRoutine);
			}

			prepareRoutine = StartCoroutine(OnPrepareRendering());
		}
	}

	protected virtual void OnCreateObject(GameObject obj)
	{
		originObj = obj;

		originObj.transform.localPosition = spawnLocalPos;
		originObj.transform.localEulerAngles = spawnLocalRot;
	}

	protected virtual void OnEnable()
	{
		if (originObj == null)
			return;

		cam.gameObject.SetActive(true);
	}

	protected virtual void OnDisable()
	{
		if (originObj == null)
			return;

		if (cam != null)
		{
			cam.gameObject.SetActive(false);
		}
	}

	protected virtual IEnumerator OnPrepareRendering()
	{
		yield return ResourceManager.Instance.InstantiateAsync<GameObject>(originPrefab, cameraArm, OnCreateObject);
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
		else if (textureView != null)
		{
			textureView.color = Color.white;
			textureView.texture = GetScreenShotTexture();
		}
		else if (meshView != null)
		{
			meshView.SetColor(Color.white);

			var texture = GetScreenShotTexture();
			meshView.SetTexture(texture);
		}
	}

	protected virtual void OnBecameVisible()
	{
		if (originObj == null)
			return;

		if (cam != null)
		{
			cam.gameObject.SetActive(true);
		}
	}

	protected virtual void OnBecameInvisible()
	{
		if (originObj == null)
			return;

		if (cam != null)
		{
			cam.gameObject.SetActive(false);
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
		RenderTexture.active = renderTexture;

		texture.ReadPixels(rect, 0, 0);
		texture.Apply();

		return texture;
	}

	/// <summary>
	/// 성능 때문에 사용 지양
	/// </summary>
	/// <returns></returns>
	private Sprite GetScreenShotSprite()
	{
		RenderTexture.active = renderTexture;

		texture.ReadPixels(rect, 0, 0);
		texture.Apply();

		return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
	}
}