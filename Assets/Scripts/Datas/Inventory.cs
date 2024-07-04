using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory")]
public class Inventory : ScriptableObject
{
	[SerializeField] private ItemTable table;

	[System.Serializable]
	public class HasItemDataDictionary : SerializableDictionary<string, int> { }

	[Header("[보유 중인 아이템]")]
	[Space]
	public HasItemDataDictionary hasItems = new HasItemDataDictionary();

	[Header("[장착 중인 바디 파츠]")]
	[Header("[(0) 바디 / (1) 머리 / (2) 눈 / (3) 얼굴 / (4) 악세사리]")]
	[Space]
	public CharacterType[] characterMeshTypes = new CharacterType[5];

	[Header("[장착 중인 소품]")]
	[Space]
	public PropType[] characterPropTypes = null;

	public void AddItem(string itemCode)
	{
		if (table.IsValidItem(itemCode) == false)
			return;

		if (hasItems.ContainsKey(itemCode))
		{
			hasItems[itemCode]++;
		}
		else
		{
			hasItems[itemCode] = 1;
		}

#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}

	public int GetHasCoinCount()
	{
		if(hasItems.ContainsKey("Coin") == false)
			return 0;

		return hasItems["Coin"];
	}

	public CharacterType GetCharacterTypeByPartsType(CharacterPartsType partsType)
	{
		if (partsType == CharacterPartsType.Accessory)
			return characterMeshTypes[4];

		if (partsType == CharacterPartsType.Face)
			return characterMeshTypes[3];

		if (partsType == CharacterPartsType.RightEye || partsType == CharacterPartsType.LeftEye)
			return characterMeshTypes[2];

		if (partsType == CharacterPartsType.BackHair || partsType == CharacterPartsType.FrontHair)
			return characterMeshTypes[1];

		if (partsType == CharacterPartsType.Body)
			return characterMeshTypes[0];

		return CharacterType.UnityChan;
	}

	public void SetMyPacketData(MyPlayerPacketData myData)
	{
		for (int i = 0; i < myData.hasItems.Length; i++)
		{
			string itemCode = myData.hasItems[i];
			int itemCount = myData.hasItemCounts[i];

			hasItems[itemCode] = itemCount;
		}

		// 장착 중인 아이템, 적용 중인 버프 아이템도 반영
	}
}
