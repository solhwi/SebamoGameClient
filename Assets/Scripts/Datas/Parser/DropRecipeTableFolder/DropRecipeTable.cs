using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("DropRecipeTable.asset")]
public partial class DropRecipeTable : ScriptParser
{
	public override void Parser()
	{
		recipeDataDictionary.Clear();
		foreach(var value in recipeDataList)
		{
			recipeDataDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class RecipeData
	{
		public int key;
		public string recipe;
	}

	public List<RecipeData> recipeDataList = new List<RecipeData>();
	[System.Serializable]
	public class RecipeDataDictionary : SerializableDictionary<int, RecipeData> {}
	public RecipeDataDictionary recipeDataDictionary = new RecipeDataDictionary();


}
