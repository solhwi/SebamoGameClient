using UnityEngine;
using System.Collections.Generic;

public partial class DropRecipeTable
{
	public static Dictionary<string, int> ParseDropRecipeData(string rawRecipe)
	{
		Dictionary<string, int> res = new Dictionary<string, int>();

		string[] columns = rawRecipe.Split('/');
		
		foreach (var column in columns)
		{
			string[] keyValue = column.Split(':');

			if (keyValue.Length != 2)
				continue;

			if (int.TryParse(keyValue[1], out int value) == false)
				continue;

			res.Add(keyValue[0], value);
		}

		return res;
	}

	public static string GetDropItemCode(Dictionary<string, int> recipe)
	{
		int randomNumber = Random.Range(1, 101);

		int prevRateValue = 0;

		foreach (var iter in recipe)
		{
			int rateValue = iter.Value;

			if (prevRateValue < randomNumber && prevRateValue <= prevRateValue + rateValue)
			{
				return iter.Key;
			}

			prevRateValue += rateValue;
		}

		return string.Empty;
	}
}