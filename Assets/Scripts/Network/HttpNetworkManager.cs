using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

public class HttpNetworkManager : MonoBehaviour
{
	public void TryGet()
	{
		StartCoroutine(GetRoutine(""));
	}

	public void TryPost()
	{
		StartCoroutine(PostRoutine("", ""));
	}

	public void TryPut()
	{
		StartCoroutine(PutRoutine("", ""));
	}

	private IEnumerator PutRoutine(string url, string data)
	{
		UnityWebRequest www = UnityWebRequest.Put(url, data);
		yield return OnRequest(www);
	}

	private IEnumerator PostRoutine(string url, string data)
	{
		UnityWebRequest www = UnityWebRequest.PostWwwForm(url, data);
		yield return OnRequest(www);
	}

	private IEnumerator GetRoutine(string url)
	{
		UnityWebRequest www = UnityWebRequest.Get(url);
		yield return OnRequest(www);
	}

	private IEnumerator OnRequest(UnityWebRequest request)
	{
		yield return request.SendWebRequest();

		if (request.error == null)
		{
			Debug.Log(request.downloadHandler.text);
		}
		else
		{
			Debug.Log(request.error);
		}
	}

}
