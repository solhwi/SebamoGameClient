using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("TileTable.asset")]
public partial class TileTable : ScriptParser
{
	public override void Parser()
	{
		tileDataDictionary.Clear();
		foreach(var value in tileDataList)
		{
			tileDataDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class TileData
	{
		public string key;
		public string tileAssetPath;
		public TileActionType tileActionType;
		public string tileActionParameter;
	}

	public List<TileData> tileDataList = new List<TileData>();
	[System.Serializable]
	public class TileDataDictionary : SerializableDictionary<string, TileData> {}
	public TileDataDictionary tileDataDictionary = new TileDataDictionary();


}
