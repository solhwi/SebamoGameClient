using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("AuthDataTable.asset")]
public partial class AuthDataTable : ScriptParser
{
	public override void Parser()
	{
		kahluaAuthDataDictionary.Clear();
		foreach(var value in kahluaAuthDataList)
		{
			kahluaAuthDataDictionary.Add(value.key, value);
		}
		expAuthDataDictionary.Clear();
		foreach(var value in expAuthDataList)
		{
			expAuthDataDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class KahluaAuthData
	{
		public string key;
		public string name;
	}

	public List<KahluaAuthData> kahluaAuthDataList = new List<KahluaAuthData>();
	[System.Serializable]
	public class KahluaAuthDataDictionary : SerializableDictionary<string, KahluaAuthData> {}
	public KahluaAuthDataDictionary kahluaAuthDataDictionary = new KahluaAuthDataDictionary();

	[Serializable]
	public class ExpAuthData
	{
		public string key;
		public string name;
	}

	public List<ExpAuthData> expAuthDataList = new List<ExpAuthData>();
	[System.Serializable]
	public class ExpAuthDataDictionary : SerializableDictionary<string, ExpAuthData> {}
	public ExpAuthDataDictionary expAuthDataDictionary = new ExpAuthDataDictionary();


}
