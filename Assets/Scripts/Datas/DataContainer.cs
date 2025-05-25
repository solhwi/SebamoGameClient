using System.Collections;
using UnityEngine;

public class DataContainer : ScriptableObject
{
	public virtual IEnumerator Preload()
	{
		yield break;
	}
}

public class DataContainer<T> : DataContainer where T : DataContainer
{
    public static T Instance
	{
		get
		{
			return ResourceManager.Instance.Load<T>($"Assets/Bundle/Datas/{typeof(T)}.asset");
		}
	}
}
