using System.Collections;
using UnityEditor;
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
			if (instance == null)
			{
				instance = ResourceManager.Instance.LoadData<T>();
				return instance;
			}

			return instance;
		}
	}

	private static T instance = null;
}
