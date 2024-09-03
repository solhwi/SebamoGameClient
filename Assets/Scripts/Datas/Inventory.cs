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

	[Header("[적용 중인 프로필 아이템]")]
	[Header("[(0) 이미지 / (1) 프레임")]
	[Space]
	public string[] appliedProfileItems = new string[2];

	private const int MaxBuffItemCount = 10;

	public CharacterType GetEquippedBodyType(int index)
	{
		if (index < 0 || index >= equippedItems.Length)
			return CharacterType.Max;

		string itemCode = equippedItems[index];
		return itemTable.GetPartsCharacterType(itemCode);
	}

	public IEnumerable<PropType> GetEquippedPropType()
	{
		int index = GetIndexPropType();
		string itemCode = equippedItems[index];

		var propType = itemTable.GetItemPropType(itemCode);
		if (propType == PropType.TwinDagger_L)
		{
			yield return PropType.TwinDagger_L;
			yield return PropType.TwinDagger_R;
		}
		else
		{
			yield return propType;
		}
	}

	public bool TryAddItem(string itemCode, int count = 1)
	{
		bool bResult = AddItem(itemCode, count);
		if (bResult == false)
			return false;

		return true;
	}

	public bool TryRemoveItem(string itemCode, int count = 1)
	{
		bool bResult = RemoveItem(itemCode, count);
		if (bResult == false)
			return false;

		return true;
	}

	public async Task<bool> TryEquipOn(string itemCode)
	{
		bool bResult = EquipOnItem(itemCode);
		if (bResult == false)
			return false;

		return await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task<bool> TryEquipOff(int index)
	{
		EquipOffItem(index);
		return await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task<bool> TryEquipOff(string itemCode)
	{
		bool bResult = EquipOffItem(itemCode);
		if (bResult == false)
			return false;

		return await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public string GetUsableBuffItemCode()
	{
		if (appliedBuffItems != null && appliedBuffItems.Count > 0)
		{
			return appliedBuffItems[0];
		}

		return string.Empty;
	}

	public async Task<bool> TryApplyBuff(string itemCode)
	{
		bool bResult = ApplyBuff(itemCode);
		if (bResult == false)
			return false;

		return await HttpNetworkManager.Instance.TryPostMyPlayerData();
	}

	public async Task<bool> TryUseBuff(string itemCode)
	{
		bool bResult = UseBuff(itemCode);
		if (bResult == false)
			return false;

		return await HttpNetworkManager.Instance.TryPostMyPlayerData();
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
		else if (hasCount == 0)
		{
			hasItems.Remove(itemCode);
		}
		else
		{
			return false;
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
		if (appliedBuffItems.Count >= MaxBuffItemCount)
			return false;

		appliedBuffItems.Add(buffItemCode);
		return true;
	}

	private bool UseBuff(string buffItemCode)
	{
		if (appliedBuffItems.Contains(buffItemCode) == false)
			return false;

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

	public void EquipOffItem(int index)
	{
		if (index >= 0 && index < equippedItems.Length)
		{
			equippedItems[index] = string.Empty;
		}
	}

	public bool EquipOffItem(string itemCode)
	{
		if (itemTable.IsValidItem(itemCode) == false)
			return false;

		if (hasItems.ContainsKey(itemCode) == false)
			return false;

		for (int i = 0; i < equippedItems.Length; i++)
		{
			if (equippedItems[i] == itemCode)
			{
				equippedItems[i] = string.Empty;
			}
		}

		for (int i = 0; i < appliedProfileItems.Length; i++)
		{
			if (appliedProfileItems[i] == itemCode)
			{
				appliedProfileItems[i] = string.Empty;
			}
		}

		return true;
	}

	private bool EquipOnItem(string itemCode)
	{
		if (itemTable.IsValidItem(itemCode) == false)
			return false;

		if (hasItems.ContainsKey(itemCode) == false)
			return false;

		if (itemTable.IsProfileItem(itemCode))
		{
			if (itemTable.profileItemDataDictionary.TryGetValue(itemCode, out var profileItemData))
			{
				int index = profileItemData.isFrame;
				appliedProfileItems[index] = itemCode;
			}
		}
		else if (itemTable.IsPropItem(itemCode))
		{
			int index = GetIndexPropType();
			if (index >= 0 && index < equippedItems.Length)
			{
				equippedItems[index] = itemCode;
			}
		}
		else
		{
			var partsType = itemTable.GetItemPartsType(itemCode);
			int index = GetIndexFromPartsType(partsType);

			if (index >= 0 && index < GetIndexPropType())
			{
				equippedItems[index] = itemCode;
			}
		}

		return true;
	}

	public bool IsEquippedItem(string itemCode)
	{
		bool b = equippedItems.Any(i => i == itemCode);
		b |= appliedProfileItems.Any(i => i == itemCode);

		return b;
	}

	public int GetHasCoinCount()
	{
		if(hasItems.ContainsKey(ItemTable.Coin) == false)
			return 0;

		return hasItems[ItemTable.Coin];
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
		int index = -1;

		if (partsType == CharacterPartsType.Accessory)
			index = 4;

		if (partsType == CharacterPartsType.Face)
			index = 3;

		if (partsType == CharacterPartsType.RightEye || partsType == CharacterPartsType.Eye)
			index = 2;

		if (partsType == CharacterPartsType.Hair || partsType == CharacterPartsType.FrontHair)
			index = 1;

		if (partsType == CharacterPartsType.Body)
			index = 0;

		return GetEquippedBodyType(index);
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

		equippedItems = new string[6];

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
