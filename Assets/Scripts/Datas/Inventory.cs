using NPOI.SS.Formula.PTG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

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

	public async Task TryAddItem(string itemCode, int count = 1)
	{
		bool bResult = AddItem(itemCode, count);
		if (bResult == false)
			return;

		await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task TryRemoveItem(string itemCode, int count = 1)
	{
		bool bResult = RemoveItem(itemCode, count);
		if (bResult == false)
			return;

		await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task TryEquipOn(string itemCode)
	{
		bool bResult = EquipOnItem(itemCode);
		if (bResult == false)
			return;

		await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task TryApplyBuff(string itemCode)
	{
		bool bResult = ApplyBuff(itemCode);
		if (bResult == false)
			return;

		await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task TryUseBuff(string itemCode)
	{
		bool bResult = UseBuff(itemCode);
		if (bResult == false)
			return;

		await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	private bool RemoveItem(string itemCode, int count = 1)
	{
		if (itemTable.IsValidItem(itemCode) == false)
			return false;

		if (hasItems.ContainsKey(itemCode) == false)
			return false;

		int hasCount = hasItems[itemCode] - count;
		if (hasCount > 0)
		{
			hasItems[itemCode] = hasCount;
		}
		else
		{
			hasItems.Remove(itemCode);
		}

		return true;
	}

	private bool AddItem(string itemCode, int count = 1)
	{
		if (itemTable.IsValidItem(itemCode) == false)
			return false;

		if (hasItems.ContainsKey(itemCode))
		{
			hasItems[itemCode] += count;
		}
		else
		{
			hasItems[itemCode] = count;
		}

		return true;
	}

	private bool ApplyBuff(string buffItemCode)
	{
		if (appliedBuffItems.Contains(buffItemCode))
			return false;

		appliedBuffItems.Add(buffItemCode);
		return true;
	}

	private bool UseBuff(string buffItemCode)
	{
		if (appliedBuffItems.Contains(buffItemCode) == false)
			return false;

		// 버프 작동 로직

		appliedBuffItems.Remove(buffItemCode);
		return true;
	}

	private void ApplyBuffs(IEnumerable<string> buffItems)
	{
		if (buffItems == null)
			return;

		foreach(var buffItem in buffItems)
		{
			ApplyBuff(buffItem);
		}
	}

	private void EquipOnItems(IEnumerable<string> equippedItems)
	{
		foreach (var item in equippedItems)
		{
			EquipOnItem(item);
		}
	}

	private bool EquipOnItem(string itemCode)
	{
		if (itemTable.IsValidItem(itemCode) == false)
			return false;

		if (hasItems.ContainsKey(itemCode) == false)
			return false;

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

		return true;
	}

	public bool IsEquippedItem(string itemCode)
	{
		return equippedItems.Any(i => i == itemCode);
	}

	public int GetHasCoinCount()
	{
		if(hasItems.ContainsKey("Coin") == false)
			return 0;

		return hasItems["Coin"];
	}

	public int GetHasCount(string itemCode)
	{
		if (itemCode == null || itemCode == string.Empty)
			return 0;

		if (hasItems.ContainsKey(itemCode) == false)
			return 0;

		return hasItems[itemCode];
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
		appliedBuffItems.Clear();
		equippedPropTypes.Clear();

		equippedItems = new string[6];
		equippedBodyTypes = new CharacterType[5];

		if (myData == null)
			return;

		for (int i = 0; i < myData.hasItems.Length; i++)
		{
			string itemCode = myData.hasItems[i];
			int itemCount = myData.hasItemCounts[i];

			AddItem(itemCode, itemCount);
		}

		EquipOnItems(myData.playerData.equippedItems);
		ApplyBuffs(myData.appliedBuffItems);
	}
}
