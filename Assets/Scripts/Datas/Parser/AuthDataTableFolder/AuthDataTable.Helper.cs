
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public partial class AuthDataTable
{
	public IEnumerable<KeyValuePair<string, string>> GetAuthData(string address)
	{
		if (kahluaAuthDataDictionary.TryGetValue(address, out var kData))
		{
			yield return new KeyValuePair<string, string>("Kahlua", kData.name);
		}
		if (expAuthDataDictionary.TryGetValue(address, out var eData))
		{
			yield return new KeyValuePair<string, string>("Exp", eData.name);
		}
	}
}