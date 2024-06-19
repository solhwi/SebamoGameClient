
using Unity.VisualScripting;

public static class ItemTableExtension
{
	public static string GetAssetPathWithoutResources(this ItemTable.DropItemData dropItemData)
	{
		return dropItemData.fieldIconAssetPath.Replace("Assets/Resources", "");
	}
}

public partial class ItemTable
{
	
}