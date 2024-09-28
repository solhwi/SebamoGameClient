using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;


public class UnusedAssetRemover : Editor
{
	[MenuItem("Tools/Clean Unused Assets")]
	public static void CleanUnusedAssets()
	{
		var unusedAssets = GetAllUnUsedAssets(IsExcludeAssets);

		// 사용되지 않는 에셋을 삭제합니다.
		foreach (string asset in unusedAssets)
		{
			bool success = AssetDatabase.DeleteAsset(asset);
			if (success)
			{
				Debug.Log($"Deleted unused asset: {asset}");
			}
			else
			{
				Debug.LogWarning($"Failed to delete asset: {asset}");
			}
		}

		// 변경 사항을 적용합니다.
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private static IEnumerable<string> GetAllUnUsedAssets(Func<string, bool> IsExclude)
	{
		// 모든 에셋을 가져옵니다.
		string[] allAssets = AssetDatabase.GetAllAssetPaths();

		// 모든 씬과 프리팹에서 참조되는 에셋을 추적합니다.
		HashSet<string> usedAssets = new HashSet<string>(GetAllDependencies());

		// 에셋 목록 중 사용되지 않는 에셋을 찾습니다.
		foreach (string asset in allAssets)
		{
			if (!usedAssets.Contains(asset) && !IsExclude(asset))
			{
				yield return asset;
			}
		}
	}

	private static IEnumerable<string> GetAllDependencies()
	{
		List<string> dependencies = new List<string>();

		foreach (var scene in EditorBuildSettings.scenes)
		{
			dependencies.AddRange(AssetDatabase.GetDependencies(scene.path));
		}

		string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");
		foreach (var prefabGUID in allPrefabs)
		{
			string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
			dependencies.AddRange(AssetDatabase.GetDependencies(prefabPath));
		}

		return dependencies;
	}

	private static bool IsExcludeAssets(string assetPath)
	{
		bool b = assetPath.StartsWith("Assets/Bundles/") == false;

		b |= assetPath.StartsWith("Assets/Bundles/Datas");
		b |= assetPath.StartsWith("Assets/Bundles/Prefabs");

		return b;
	}
}
