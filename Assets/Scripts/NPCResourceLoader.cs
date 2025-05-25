using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NPCResourceLoader")]
public class NPCResourceLoader : DataContainer<NPCResourceLoader>
{
	[SerializeField] List<AssetReferenceTexture2D> textureRefs = new List<AssetReferenceTexture2D>();
	[SerializeField] List<AssetReferenceT<Material>> materialRefs = new List<AssetReferenceT<Material>>();
	[SerializeField] List<AssetReferenceGameObject> modelRefs = new List<AssetReferenceGameObject>();

	public override IEnumerator Preload()
	{
		foreach (var t in textureRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<Texture2D>(t);
		}

		foreach (var m in materialRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<Material>(m);
		}

		foreach (var m in modelRefs)
		{
			yield return ResourceManager.Instance.LoadAsync<GameObject>(m);
		}
	}
}
