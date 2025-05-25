using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("ItemTable.asset")]
public partial class ItemTable : ScriptParser<ItemTable>
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
		itemToolTipDataDictionary.Clear();
		foreach(var value in itemToolTipDataList)
		{
			itemToolTipDataDictionary.Add(value.key, value);
		}
		fieldItemDataDictionary.Clear();
		foreach(var value in fieldItemDataList)
		{
			fieldItemDataDictionary.Add(value.key, value);
		}
		buffItemDataDictionary.Clear();
		foreach(var value in buffItemDataList)
		{
			buffItemDataDictionary.Add(value.key, value);
		}
		profileItemDataDictionary.Clear();
		foreach(var value in profileItemDataList)
		{
			profileItemDataDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class ShopItemData
	{
		public string key;
		public int isEquipment;
		public int price;
		public string shopItemDescription;
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
	public class ItemToolTipData
	{
		public string key;
		public string iconAssetPath;
		public string itemName;
		public string itemDescription;
		public int sellPrice;
	}

	public List<ItemToolTipData> itemToolTipDataList = new List<ItemToolTipData>();
	[System.Serializable]
	public class ItemToolTipDataDictionary : SerializableDictionary<string, ItemToolTipData> {}
	public ItemToolTipDataDictionary itemToolTipDataDictionary = new ItemToolTipDataDictionary();

	[Serializable]
	public class FieldItemData
	{
		public string key;
		public string fieldIconAssetPath;
		public FieldActionType actionType;
		public string actionParameter;
		public string effectPath;
	}

	public List<FieldItemData> fieldItemDataList = new List<FieldItemData>();
	[System.Serializable]
	public class FieldItemDataDictionary : SerializableDictionary<string, FieldItemData> {}
	public FieldItemDataDictionary fieldItemDataDictionary = new FieldItemDataDictionary();

	[Serializable]
	public class BuffItemData
	{
		public string key;
		public string buffIconAssetPath;
		public BuffActionType actionType;
		public string actionParameter;
		public string effectPath;
		public int isDeBuff;
	}

	public List<BuffItemData> buffItemDataList = new List<BuffItemData>();
	[System.Serializable]
	public class BuffItemDataDictionary : SerializableDictionary<string, BuffItemData> {}
	public BuffItemDataDictionary buffItemDataDictionary = new BuffItemDataDictionary();

	[Serializable]
	public class ProfileItemData
	{
		public string key;
		public int isFrame;
	}

	public List<ProfileItemData> profileItemDataList = new List<ProfileItemData>();
	[System.Serializable]
	public class ProfileItemDataDictionary : SerializableDictionary<string, ProfileItemData> {}
	public ProfileItemDataDictionary profileItemDataDictionary = new ProfileItemDataDictionary();


}
