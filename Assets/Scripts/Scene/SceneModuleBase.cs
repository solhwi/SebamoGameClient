using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneModuleBase : MonoBehaviour
{
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
		UIManager.Instance.OpenMainCanvas();
	}

	public virtual void OnExit()
    {
		UIManager.Instance.CloseMainCanvas();
	}
}
