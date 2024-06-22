
using System.IO;
using Unity.VisualScripting;

public static class ItemTableExtension
{
	public static string GetAssetPathWithoutResources(this ItemTable.DropItemData dropItemData)
	{
		string path = dropItemData.fieldIconAssetPath.Replace("Assets/Resources/", "");
		string noExtensionFileName = Path.GetFileNameWithoutExtension(path);
		string fileName = Path.GetFileName(path);

		return path.Replace(fileName, noExtensionFileName);
	}
}

public partial class ItemTable
{
	public bool IsValidItem(string itemCode)
	{
		if (dropItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (partsItemDataDictionary.ContainsKey(itemCode))
			return true;

		if(propItemDataDictionary.ContainsKey(itemCode))
			return true;

		if (shopItemDataDictionary.ContainsKey(itemCode))
			return true;

		return false;
	}
}