using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;



public class BackGroundCanvas : MonoBehaviour, IBoardGameSubscriber
{
	[SerializeField] private RawImage backGroundFrontImage = null;
	[SerializeField] private RawImage backGroundBackImage = null;

	[SerializeField] private PlayerDataContainer playerDataContainer;
	[SerializeField] private TileDataContainer tileDataContainer;

	[SerializeField] private float fadeTime = 1.0f;

	private Coroutine fadeRoutine = null;

	private void Start()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Subscribe(this);
		}

		SetBackGround(playerDataContainer.currentTileOrder);
	}

	private void OnDestroy()
	{
		if (BoardGameManager.Instance != null)
		{
			BoardGameManager.Instance.Unsubscribe(this);
		}

		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
	}

	public IEnumerator OnRollDice(int diceCount, int nextBonusAddCount, float nextBonusMultiplyCount)
	{
		yield return null;
	}

	private void SetBackGround(int currentOrder)
	{
		backGroundFrontImage.texture = tileDataContainer.GetBackGroundResource(currentOrder);
	}

	private IEnumerator FadeBackGround(int nextOrder)
	{
		// 다음 이미지가 앞 이미지와 같다면 패스
		var nextBackGround = tileDataContainer.GetBackGroundResource(nextOrder);
		if (nextBackGround == backGroundFrontImage.texture)
			yield break;

		// 다음 이미지를 뒤에 세팅한다.
		backGroundBackImage.texture = nextBackGround;
		backGroundBackImage.color = backGroundBackImage.color.Alpha(1);

		// 현재 이미지의 알파를 서서히 줄인다. 
		backGroundFrontImage.color = backGroundFrontImage.color.Alpha(1);

		float t = 1.0f;

		while (t >= 0.0f)
		{
			yield return null;
			t -= Time.deltaTime;

			backGroundFrontImage.color = backGroundFrontImage.color.Alpha(t / fadeTime);
		}

		backGroundFrontImage.color = backGroundFrontImage.color.Alpha(0);

		yield return null;

		// 다음 이미지를 앞에 세팅한다.
		backGroundFrontImage.texture = nextBackGround;
		backGroundFrontImage.color = backGroundFrontImage.color.Alpha(1);

		// 뒤 이미지의 알파를 없앤다.
		backGroundBackImage.color = backGroundBackImage.color.Alpha(0);
	}

	public IEnumerator OnMove(int currentOrder, int nextOrder, int diceCount)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}

		SetBackGround(currentOrder);

		yield return null;

		fadeRoutine = StartCoroutine(FadeBackGround(nextOrder));
	}

	public IEnumerator OnGetItem(FieldItem fieldItem, int currentOrder, int nextOrder)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}

		SetBackGround(currentOrder);

		yield return null;

		fadeRoutine = StartCoroutine(FadeBackGround(nextOrder));
	}

	public IEnumerator OnDoTileAction(int currentOrder, int nextOrder)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}

		SetBackGround(currentOrder);

		yield return null;

		fadeRoutine = StartCoroutine(FadeBackGround(nextOrder));
	}
}
