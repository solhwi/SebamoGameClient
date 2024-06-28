using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecyclingType
{
	DropItem
}

/// <summary>
/// 재사용 오브젝트에 붙임
/// </summary>
public class RecyclingObject : MonoBehaviour
{
	public RecyclingType type;
}
