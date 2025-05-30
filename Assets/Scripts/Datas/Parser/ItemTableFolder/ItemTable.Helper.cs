
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class ItemTableExtension
{
	public static string GetAssetPathWithoutResources(this ItemTable.FieldItemData itemData)
	{
		return GetAssetPathWithoutResources(itemData.fieldIconAssetPath);
	}

	public static string GetAssetPathWithoutResources(this ItemTable.ItemToolTipData itemData)
	{
		return GetAssetPathWithoutResources(itemData.iconAssetPath);
	}

	public static string GetAssetPathWithoutResources(this string path)
	{
		path = path.Replace("Assets/Bundles/", "");
		return "Assets/Bundles/" + path;
	}
}

public partial class ItemTable
{
	[SerializeField] public List<ShopItemData> sortedShopItemList = new List<ShopItemData>();
	public const string Coin = "Coin";

	public override void RuntimeParser()
	{
		base.RuntimeParser();

		sortedShopItemList = shopItemDataList
						.OrderByDescending(d => d.isEquipment)
						.ToList();
	}

	private IEnumerable<string> GetPreLoadTableDataPath()
	{
		foreach (var d in itemToolTipDataList)
		{
			yield return d.iconAssetPath.GetAssetPathWithoutResources();
		}

		foreach (var d in fieldItemDataList)
		{
			yield return d.fieldIconAssetPath.GetAssetPathWithoutResources();
		}

		foreach (var d in buffItemDataList)
		{
			yield return d.buffIconAssetPath.GetAssetPathWithoutResources();
		}
	}

	public override IEnumerator Preload()
	{
		foreach (string path in GetPreLoadTableDataPath())
		{
			string extension = Path.GetExtension(path);
			if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
			{
				yield return ResourceManager.Instance.LoadAsync<Sprite>(path);
			}
			else
			{
				yield return ResourceManager.Instance.LoadAsync<UnityEngine.Object>(path);
			}
		}
	}

	public int Compare(string itemCode1, string itemCode2)
	{
		int order1 = -1;
		int order2 = -1;

		if (propItemDataDictionary.ContainsKey(itemCode1))
		{
			order1 = (int)CharacterPartsType.Max;
		}
		else if (partsItemDataDictionary.TryGetValue(itemCode1, out var itemData))
		{
			order1 = (int)itemData.partsType;
		}
		else if (profileItemDataDictionary.TryGetValue(itemCode1, out var profileItemData))
		{
			order1 = profileItemData.isFrame + (int)CharacterPartsType.Max;
		}
		else if (fieldItemDataDictionary.TryGetValue(itemCode1, out var fieldItemData))
		{
			order1 = (int)CharacterPartsType.Max + 2;
		}
		else if (buffItemDataDictionary.TryGetValue(itemCode1, out var buffItemData))
		{
			order1 = (int)CharacterPartsType.Max + 3;
		}

		if (propItemDataDictionary.ContainsKey(itemCode2))
		{
			order2 = (int)CharacterPartsType.Max;
		}
		else if (partsItemDataDictionary.TryGetValue(itemCode2, out var itemData))
		{
			order2 = (int)itemData.partsType;
		}
		else if (profileItemDataDictionary.TryGetValue(itemCode2, out var profileItemData))
		{
			order2 = profileItemData.isFrame + (int)CharacterPartsType.Max;
		}
		else if (fieldItemDataDictionary.TryGetValue(itemCode2, out var fieldItemData))
		{
			order2 = (int)CharacterPartsType.Max + 2;
		}
		else if (buffItemDataDictionary.TryGetValue(itemCode2, out var buffItemData))
		{
			order2 = (int)CharacterPartsType.Max + 3;
		}

		if (order1 == order2)
		{
			return 0;
		}
		else
		{
			return order1 > order2 ? 1 : -1;
		}
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

		if (itemToolTipDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			return iconData.itemDescription;
		}

		return string.Empty;
	}

	public string GetItemName(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return string.Empty;

		if (itemToolTipDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			return iconData.itemName;
		}

		return string.Empty;
	}

	public Sprite GetItemToolTipIconSprite(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return null;

		if (itemToolTipDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			return ResourceManager.Instance.Load<Sprite>(iconData.GetAssetPathWithoutResources());
		}

		return null;
	}

	public Sprite GetBuffItemIconSprite(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return null;

		if (buffItemDataDictionary.TryGetValue(itemCode, out var itemData))
		{
			return ResourceManager.Instance.Load<Sprite>(itemData.buffIconAssetPath.GetAssetPathWithoutResources());
		}

		return null;
	}

	public int GetItemSellPrice(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return 0;

		if (itemToolTipDataDictionary.TryGetValue(itemCode, out var iconData))
		{
			return iconData.sellPrice;
		}

		return 0;
	}

	public int GetItemBuyPrice(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return 0;

		if (shopItemDataDictionary.TryGetValue(itemCode, out var data))
		{
			return data.price;
		}

		return 0;
	}

	public bool IsValidItem(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return false;

		if (itemCode == Coin)
			return true;

		if (fieldItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (partsItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (propItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (shopItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (buffItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (profileItemDataDictionary.ContainsKey(itemCode))
			return true;

		return false;
	}

	public bool IsEquipmentItem(string itemCode)
	{
		bool b = IsPropItem(itemCode);
		b |= IsPartsItem(itemCode);

		return b;
	}

	public bool IsBuffItem(string itemCode)
	{
		if (IsValidItem(itemCode) == false)
			return false;

		if (buffItemDataDictionary.TryGetValue(itemCode, out var data) == false)
			return false;

		return data.isDeBuff == 0;
	}

	public bool IsAvatarItem(string itemCode)
	{
		if (partsItemDataDictionary.TryGetValue(itemCode, out var partsItemData))
		{
			if (partsItemData.partsType == CharacterPartsType.Body)
				return true;

			if (partsItemData.partsType == CharacterPartsType.Accessory)
				return true;
		}
		else if (propItemDataDictionary.ContainsKey(itemCode)) 
		{
			return true;
		}

		return false;
	}

	public bool IsEnableEquipOffItem(string itemCode)
	{
		if (IsPropItem(itemCode) || IsAccessoryItem(itemCode))
		{
			return true;
		}

		return false;
	}

	public bool IsAccessoryItem(string itemCode)
	{
		if (partsItemDataDictionary.TryGetValue(itemCode, out var partsItemData))
		{
			if (partsItemData.partsType == CharacterPartsType.Accessory)
				return true;
		}

		return false;
	}

	public bool IsProfileItem(string itemCode)
	{
		if (profileItemDataDictionary.ContainsKey(itemCode))
			return true;

		return false;
	}

	public bool IsFieldItem(string itemCode)
	{
		if (IsValidItem(itemCode) == false)
			return false;

		if (fieldItemDataDictionary.ContainsKey(itemCode))
		{
			return true;
		}

		return false;
	}

	public bool IsDeBuffItem(string itemCode)
	{
		if (IsValidItem(itemCode) == false)
			return false;

		if (buffItemDataDictionary.TryGetValue(itemCode, out var data) == false)
			return false;

		return data.isDeBuff > 0;
	}

	public bool IsFrameItem(string itemCode)
	{
		if (profileItemDataDictionary.TryGetValue(itemCode, out var data))
		{
			return data.isFrame > 0;
		}

		return false;
	}

	public bool IsBeautyItem(string itemCode)
	{
		if (partsItemDataDictionary.TryGetValue(itemCode, out var itemData))
		{
			if (itemData.partsType == CharacterPartsType.Hair)
				return true;

			if (itemData.partsType == CharacterPartsType.Eye)
				return true;

			if (itemData.partsType == CharacterPartsType.FrontHair)
				return true;

			if (itemData.partsType == CharacterPartsType.RightEye)
				return true;

			if (itemData.partsType == CharacterPartsType.Face)
				return true;
		}

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
		if (itemCode == null)
			return CharacterPartsType.Max;

		if (partsItemDataDictionary.TryGetValue(itemCode, out var data) == false)
			return CharacterPartsType.Max;

		return data.partsType;
	}

	public CharacterType GetPartsCharacterType(string itemCode)
	{
		if (itemCode == null)
			return CharacterType.Max;

		if (partsItemDataDictionary.TryGetValue(itemCode, out var data) == false)
			return CharacterType.Max;

		return data.characterType;
	}

	public PropType GetItemPropType(string itemCode)
	{
		if (itemCode == null)
			return PropType.Max;

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

	public static KeyValuePair<MathType, float> ParseBuffData(string rawRecipe)
	{
		string[] columns = rawRecipe.Split(':');

		var mathTypeStr = columns[0];
		var mathType = CommonFunc.GetMathType(mathTypeStr);
			
		if (float.TryParse(columns[1], out float value) == false)
			return default;

		return new KeyValuePair<MathType, float>(mathType, value);
	}

	public static int[] ParseRangeData(string rawRecipe)
	{
		int[] ranges = new int[2];

		string[] columns = rawRecipe.Split('/');

		for (int i = 0; i < 2; i++)
		{
			string[] keyValue = columns[i].Split(':');

			if (keyValue.Length != 2)
				continue;

			if (int.TryParse(keyValue[1], out int value) == false)
				continue;

			ranges[i] = value;
		}

		return ranges;
	}

	public static int ParseCountData(string rawRecipe)
	{
		string[] columns = rawRecipe.Split('/');
		if (int.TryParse(columns[2], out int value) == false)
			return 0;

		return value;
	}

	public static string GetFieldItemCode(Dictionary<string, int> recipe)
	{
		int randomNumber = UnityEngine.Random.Range(1, 101);

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