
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class AuthData
{
	public readonly string address;
	public readonly string group;
	public readonly string name;

	public AuthData(string address, string group, string name)
	{
		this.address = address;
		this.group = group;
		this.name = name;
	}
}

public partial class AuthDataTable
{
	public IEnumerable<AuthData> GetAllAuthData(string address)
	{
		if (kahluaAuthDataDictionary.TryGetValue(address, out var kData))
		{
			yield return new AuthData(address, "Kahlua", kData.name);
		}
		if (expAuthDataDictionary.TryGetValue(address, out var eData))
		{
			yield return new AuthData(address, "Exp", kData.name);
		}
	}
}