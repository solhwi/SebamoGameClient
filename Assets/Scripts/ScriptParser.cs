using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAttribute : Attribute
{
	public readonly string resourcePath = string.Empty;
	public readonly ResourceType resourceType = ResourceType.UnityAsset;

	public ResourceAttribute(string path, ResourceType type)
	{
		this.resourcePath = path;
		this.resourceType = type;
	}
}

public enum ResourceType
{
	UnityAsset = 0,
	Prefab = 1,
}

public class ScriptParserAttribute : ResourceAttribute
{
	public ScriptParserAttribute(string path) : base("Datas/Parser/" + path, ResourceType.UnityAsset)
	{

	}
}

public class ScriptAssetInfo
{
	public Type AssetType { get; set; }
	public ScriptParserAttribute Attribute { get; set; }
}

public interface IScriptParser
{
	// 기본 Parser, List, Dictionary를 만들어 줌
	public void Parser();
	public void RuntimeParser();
}

public abstract class ScriptParser<T> : DataContainer<T>, IScriptParser where T : DataContainer
{
	// 기본 Parser, List, Dictionary를 만들어 줌
	public abstract void Parser();
	public virtual void RuntimeParser() { }
}