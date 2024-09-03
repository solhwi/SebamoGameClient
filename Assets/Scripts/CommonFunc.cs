using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MathType
{
	None = -1,
	Add,
	Sub,
	Mul,
	Div,
}

public class CommonFunc
{
	public static MathType GetMathType(string rawData)
	{
		switch(rawData)
		{
			case "+":
				return MathType.Add;

			case "-":
				return MathType.Sub;

			case "*":
				return MathType.Mul;

			case "/":
				return MathType.Div;
		}

		return MathType.None;
	}

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
