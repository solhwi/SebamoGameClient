using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("ItemTable.asset")]
public partial class ItemTable : ScriptParser
{
	public override void Parser()
	{
		shopItemDataDictionary.Clear();
		foreach(var value in shopItemDataList)
		{
			shopItemDataDictionary.Add(value.key, value);
		}
		partsItemDataDictionary.Clear();
		foreach(var value in partsItemDataList)
		{
			partsItemDataDictionary.Add(value.key, value);
		}
		propItemDataDictionary.Clear();
		foreach(var value in propItemDataList)
		{
			propItemDataDictionary.Add(value.key, value);
		}
		itemIconDataDictionary.Clear();
		foreach(var value in itemIconDataList)
		{
			itemIconDataDictionary.Add(value.key, value);
		}
		dropItemDataDictionary.Clear();
		foreach(var value in dropItemDataList)
		{
			dropItemDataDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class ShopItemData
	{
		public string key;
		public int isRandom;
		public int price;
	}

	public List<ShopItemData> shopItemDataList = new List<ShopItemData>();
	[System.Serializable]
	public class ShopItemDataDictionary : SerializableDictionary<string, ShopItemData> {}
	public ShopItemDataDictionary shopItemDataDictionary = new ShopItemDataDictionary();

	[Serializable]
	public class PartsItemData
	{
		public string key;
		public CharacterPartsType partsType;
		public CharacterType characterType;
	}

	public List<PartsItemData> partsItemDataList = new List<PartsItemData>();
	[System.Serializable]
	public class PartsItemDataDictionary : SerializableDictionary<string, PartsItemData> {}
	public PartsItemDataDictionary partsItemDataDictionary = new PartsItemDataDictionary();

	[Serializable]
	public class PropItemData
	{
		public string key;
		public PropType propType;
	}

	public List<PropItemData> propItemDataList = new List<PropItemData>();
	[System.Serializable]
	public class PropItemDataDictionary : SerializableDictionary<string, PropItemData> {}
	public PropItemDataDictionary propItemDataDictionary = new PropItemDataDictionary();

	[Serializable]
	public class ItemIconData
	{
		public string key;
		public string iconAssetPath;
		public string itemName;
	}

	public List<ItemIconData> itemIconDataList = new List<ItemIconData>();
	[System.Serializable]
	public class ItemIconDataDictionary : SerializableDictionary<string, ItemIconData> {}
	public ItemIconDataDictionary itemIconDataDictionary = new ItemIconDataDictionary();

	[Serializable]
	public class DropItemData
	{
		public string key;
		public string fieldIconAssetPath;
		public DropActionType dropActionType;
		public string actionParameter;
	}

	public List<DropItemData> dropItemDataList = new List<DropItemData>();
	[System.Serializable]
	public class DropItemDataDictionary : SerializableDictionary<string, DropItemData> {}
	public DropItemDataDictionary dropItemDataDictionary = new DropItemDataDictionary();


}
