using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFunc
{
	public static IEnumerable<int> ToRange(int min, int max)
	{
		for (int i = min; i <= max; i++)
		{
			yield return i;
		}
	}

	public static bool IsEqual(float x, float y)
	{
		if (x <= y + Mathf.Epsilon &&
			x >= y - Mathf.Epsilon)
		{
			return true;
		}

		return false;
	}
}
