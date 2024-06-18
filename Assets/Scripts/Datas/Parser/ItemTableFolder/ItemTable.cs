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
	}

	[Serializable]
	public class PartsItemData
	{
		public string key;
		public string assetPath;
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
		public string assetPath;
		public PropType propType;
	}

	public List<PropItemData> propItemDataList = new List<PropItemData>();
	[System.Serializable]
	public class PropItemDataDictionary : SerializableDictionary<string, PropItemData> {}
	public PropItemDataDictionary propItemDataDictionary = new PropItemDataDictionary();


}
