
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class ItemTableExtension
{
	public static string GetAssetPathWithoutResources(this ItemTable.DropItemData itemData)
	{
		return GetAssetPathWithoutResources(itemData.fieldIconAssetPath);
	}

	public static string GetAssetPathWithoutResources(this ItemTable.ItemIconData itemData)
	{
		return GetAssetPathWithoutResources(itemData.iconAssetPath);
	}

	public static string GetAssetPathWithoutResources(this string path)
	{
		path = path.Replace("Assets/Resources/", "");
		string noExtensionFileName = Path.GetFileNameWithoutExtension(path);
		string fileName = Path.GetFileName(path);

		return path.Replace(fileName, noExtensionFileName);
	}
}

public partial class ItemTable
{
	[SerializeField] public List<ShopItemData> sortedShopItemList = new List<ShopItemData>();

	public override void RuntimeParser()
	{
		base.RuntimeParser();

		sortedShopItemList = shopItemDataList
						.OrderByDescending(d => d.isRandom)
						.ToList();
	}

	public string GetItemNPCDescription(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return string.Empty;

		if (shopItemDataDictionary.TryGetValue(itemCode, out ShopItemData itemData))
		{
			return itemData.shopItemDescription.Replace("\\n", "\n");
		}

		return string.Empty;
	}

	public string GetItemDescription(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return string.Empty;

		if (itemIconDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			return iconData.itemDescription;
		}

		return string.Empty;
	}

	public string GetItemName(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return string.Empty;

		if (itemIconDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			return iconData.itemName;
		}

		return string.Empty;
	}

	public Sprite GetItemIconSprite(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return null;

		if (itemIconDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			return ResourceManager.Instance.Load<Sprite>(iconData.GetAssetPathWithoutResources());
		}

		return null;
	}

	public bool IsValidItem(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return false;

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

	public bool IsEquipmentItem(string itemCode)
	{
		bool b = IsPropItem(itemCode);
		b |= IsPartsItem(itemCode);

		return b;
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