
public class ItemRawData
{
	public readonly string keyCode;
	public readonly string assetPath;
	public readonly string assetPathWithoutResources;

	public ItemRawData(string keyCode, string assetPath)
	{
		this.keyCode = keyCode;
		this.assetPath = assetPath;
		this.assetPathWithoutResources = assetPath.Replace("Assets/Resources/", "");
	}
}

public partial class ItemTable
{
	public ItemRawData GetItemRawData(string keyCode)
	{
		if (propItemDataDictionary.ContainsKey(keyCode))
		{
			var propItem = propItemDataDictionary[keyCode];

			return new ItemRawData(propItem.key, propItem.assetPath);
		}
		else if(partsItemDataDictionary.ContainsKey(keyCode))
		{
			var partsItem = partsItemDataDictionary[keyCode];

			return new ItemRawData(partsItem.key, partsItem.assetPath);
		}

		return null;
	}
}