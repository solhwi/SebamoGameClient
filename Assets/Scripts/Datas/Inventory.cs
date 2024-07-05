using NPOI.SS.Formula.PTG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory")]
public class Inventory : ScriptableObject
{
	[SerializeField] private ItemTable itemTable;

	[System.Serializable]
	public class HasItemDataDictionary : SerializableDictionary<string, int> { }

	[Header("[보유 중인 아이템]")]
	[Space]
	public HasItemDataDictionary hasItems = new HasItemDataDictionary();

	[Header("[장착 중인 아이템]")]
	[Header("[(0) 바디 / (1) 머리 / (2) 눈 / (3) 얼굴 / (4) 악세사리 / (5) 소품]")]
	[Space]
	public string[] equippedItems = new string[6];

	[Header("[적용 중인 버프 아이템]")]
	[Space]
	public List<string> appliedBuffItems = new List<string>();

	[HideInInspector]
	public CharacterType[] equippedBodyTypes = new CharacterType[5];

	[HideInInspector]
	public List<PropType> equippedPropTypes = new List<PropType>();

	public void AddItem(string itemCode, int count = 1)
	{
		if (itemTable.IsValidItem(itemCode) == false)
			return;

		if (hasItems.ContainsKey(itemCode))
		{
			hasItems[itemCode] += count;
		}
		else
		{
			hasItems[itemCode] = count;
		}
	}

	public void ApplyBuff(IEnumerable<string> buffItems)
	{
		if (buffItems == null)
			return;

		appliedBuffItems = buffItems.ToList();
	}

	public void EquipOnItems(IEnumerable<string> equippedItems)
	{
		foreach (var item in equippedItems)
		{
			EquipOnItem(item);
		}
	}

	public void EquipOnItem(string itemCode)
	{
		if (itemTable.IsValidItem(itemCode) == false)
			return;

		if (hasItems.ContainsKey(itemCode) == false)
			return;

		if (itemTable.IsPropItem(itemCode))
		{
			int index = GetIndexPropType();
			if (index >= 0 && index < equippedItems.Length)
			{
				equippedItems[index] = itemCode;
			}

			equippedPropTypes.Clear();

			var propType = itemTable.GetItemPropType(itemCode);
			if (propType == PropType.TwinDagger_L)
			{
				equippedPropTypes.Add(PropType.TwinDagger_L);
				equippedPropTypes.Add(PropType.TwinDagger_R);
			}
			else
			{
				equippedPropTypes.Add(propType);
			}
		}
		else
		{
			var partsType = itemTable.GetItemPartsType(itemCode);
			int index = GetIndexFromPartsType(partsType);

			if (index >= 0 && index < GetIndexPropType())
			{
				equippedItems[index] = itemCode;
				equippedBodyTypes[index] = itemTable.GetPartsCharacterType(itemCode);
			}
		}
	}

	public int GetHasCoinCount()
	{
		if(hasItems.ContainsKey("Coin") == false)
			return 0;

		return hasItems["Coin"];
	}

	public CharacterType GetCurrentPartsCharacterType(CharacterPartsType partsType)
	{
		if (partsType == CharacterPartsType.Accessory)
			return equippedBodyTypes[4];

		if (partsType == CharacterPartsType.Face)
			return equippedBodyTypes[3];

		if (partsType == CharacterPartsType.RightEye || partsType == CharacterPartsType.Eye)
			return equippedBodyTypes[2];

		if (partsType == CharacterPartsType.Hair || partsType == CharacterPartsType.FrontHair)
			return equippedBodyTypes[1];

		if (partsType == CharacterPartsType.Body)
			return equippedBodyTypes[0];

		return CharacterType.UnityChan;
	}

	private int GetIndexPropType()
	{
		return 5;
	}

	private int GetIndexFromPartsType(CharacterPartsType partsType)
	{
		if (partsType == CharacterPartsType.Accessory)
			return 4;

		if (partsType == CharacterPartsType.Face)
			return 3;

		if (partsType == CharacterPartsType.RightEye || partsType == CharacterPartsType.Eye)
			return 2;

		if (partsType == CharacterPartsType.Hair || partsType == CharacterPartsType.FrontHair)
			return 1;	

		if (partsType == CharacterPartsType.Body)
			return 0;

		return -1;
	}

	public void SetMyPacketData(MyPlayerPacketData myData)
	{
		hasItems.Clear();

		for (int i = 0; i < myData.hasItems.Length; i++)
		{
			string itemCode = myData.hasItems[i];
			int itemCount = myData.hasItemCounts[i];

			AddItem(itemCode, itemCount);
		}

		EquipOnItems(myData.playerData.equippedItems);
		ApplyBuff(myData.appliedBuffItems);
	}

#if UNITY_EDITOR

	[ContextMenu("페이크 패킷 데이터로 세팅")]
	public void SetFakePacketData()
	{
		var data = new MyPlayerPacketData();
		data.playerData = new PlayerPacketData();
		data.playerData.playerName = "지현";
		data.playerData.hasDiceCount = 3;
		data.playerData.equippedItems = new string[6] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword" };
		data.hasItems = new string[6] { "YucoBody", "MisakiHair", "UnityChanEye", "UnityChanFace", "MisakiAccessory", "GreatSword" };
		data.hasItemCounts = new int[6] { 1, 1, 1, 1, 1, 1 };
		data.appliedBuffItems = new string[] { };
		SetMyPacketData(data);

		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssetIfDirty(this);
	}
#endif
}
