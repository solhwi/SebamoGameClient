
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public static class ItemTableExtension
{
	public static string GetAssetPathWithoutResources(this ItemTable.DropItemData dropItemData)
	{
		string path = dropItemData.fieldIconAssetPath.Replace("Assets/Resources/", "");
		string noExtensionFileName = Path.GetFileNameWithoutExtension(path);
		string fileName = Path.GetFileName(path);

		return path.Replace(fileName, noExtensionFileName);
	}
}

public partial class ItemTable
{
	public bool IsValidItem(string itemCode)
	{
		if (dropItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (partsItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (propItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (shopItemDataDictionary.ContainsKey(itemCode))
			return true;

		return false;
	}

	public bool IsPropItem(string itemCode)
	{
		if (propItemDataDictionary.ContainsKey(itemCode))
			return true;

		return false;
	}

	public bool IsPartsItem(string itemCode)
	{
		if (partsItemDataDictionary.ContainsKey(itemCode))
			return true;

		return false;
	}

	public CharacterPartsType GetItemPartsType(string itemCode)
	{
		if (partsItemDataDictionary.TryGetValue(itemCode, out var data) == false)
			return CharacterPartsType.Max;

		return data.partsType;
	}

	public CharacterType GetPartsCharacterType(string itemCode)
	{
		if (partsItemDataDictionary.TryGetValue(itemCode, out var data) == false)
			return CharacterType.Max;

		return data.characterType;
	}

	public PropType GetItemPropType(string itemCode)
	{
		if (propItemDataDictionary.TryGetValue(itemCode, out var data) == false)
			return PropType.Max;

		return data.propType;
	}

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