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
		itemDataDictionary.Clear();
		foreach(var value in itemDataList)
		{
			itemDataDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class ItemData
	{
		public string key;
		public string assetPath;
	}

	public List<ItemData> itemDataList = new List<ItemData>();
	[System.Serializable]
	public class ItemDataDictionary : SerializableDictionary<string, ItemData> {}
	public ItemDataDictionary itemDataDictionary = new ItemDataDictionary();


}
