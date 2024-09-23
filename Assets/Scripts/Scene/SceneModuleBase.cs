using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneModuleBase : MonoBehaviour
{
	[SerializeField] private bool isStartScene = false;

	private IEnumerator Start()
	{
		if (isStartScene)
		{
			yield return OnPrepareEnter();
			OnEnter();
		}
	}

	public virtual IEnumerator OnPrepareEnter()
	{
		yield return null;
	}

	public virtual IEnumerator OnPrepareExit()
	{
		yield return null;
	}

    public virtual void OnEnter()
    {
		UIManager.Instance.OpenMainCanvas(true);
	}

	public virtual void OnExit()
    {
		UIManager.Instance.CloseMainCanvas();
	}
}
