using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneModuleBase : MonoBehaviour
{
	[SerializeField] protected BoardGameCanvasBase boardGameCanvas;

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
		UIManager.Instance.SetMainCanvas(boardGameCanvas);
		UIManager.Instance.OpenMainCanvas();

		if (boardGameCanvas != null)
		{
			boardGameCanvas.OnEnter();
		}
	}

	public virtual void OnExit()
    {
		if (boardGameCanvas != null)
		{
			boardGameCanvas.OnExit();
		}
	}
}
