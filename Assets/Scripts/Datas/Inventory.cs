using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory")]
public class Inventory : DataContainer<Inventory>
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

	[Header("[적용 중인 버프 아이템 (임의 수정 금지)]")]
	[Space]
	public List<string> appliedBuffItemList = new List<string>();

	private LinkedList<string> appliedBuffItems = new LinkedList<string>();

	[Header("[적용 중인 프로필 아이템]")]
	[Header("[(0) 이미지 / (1) 프레임")]
	[Space]
	public string[] appliedProfileItems = new string[2];

	private const int MaxBuffItemCount = 8;

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

	public IEnumerator TryEquipOn(string itemCode, Action<MyPlayerPacketData> onSuccess)
	{
		bool bResult = EquipOnItem(itemCode);
		if (bResult == false)
			yield break;

		yield return HttpNetworkManager.Instance.TryPostMyPlayerData(onSuccess);
	}

	public IEnumerator TryEquipOff(string itemCode, Action<MyPlayerPacketData> onSuccess)
	{
		bool bResult = EquipOffItem(itemCode);
		if (bResult == false)
			yield break;

		yield return HttpNetworkManager.Instance.TryPostMyPlayerData(onSuccess);
	}

	public string GetUsableBuffItemCode()
	{
		if (appliedBuffItems != null && appliedBuffItems.Count > 0)
		{
			return appliedBuffItems.First();
		}

		return string.Empty;
	}


	public IEnumerator TryApplyBuff(string itemCode, Action<MyPlayerPacketData> onSuccess)
	{
		bool bResult = TryRemoveItem(itemCode);
		if (bResult == false)
			yield break;

		bResult = ApplyBuffLast(itemCode);
		if (bResult == false)
			yield break;

		yield return HttpNetworkManager.Instance.TryPostMyPlayerData(onSuccess);
	}

	public bool TryUseBuff(string itemCode)
	{
		bool bResult = UseBuff(itemCode);
		if (bResult == false)
			return false;

		return true;
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

	// 사용 시 서버 동기화 챙길 것
	public bool ApplyBuffLast(string buffItemCode)
	{
		if (appliedBuffItems.Count >= MaxBuffItemCount)
			return false;

		appliedBuffItems.AddLast(buffItemCode);
		appliedBuffItemList = appliedBuffItems.ToList();
		return true;
	}

	public bool ApplyBuffFirst(string buffItemCode)
	{
		if (appliedBuffItems.Count >= MaxBuffItemCount)
			return false;

		appliedBuffItems.AddFirst(buffItemCode);
		appliedBuffItemList = appliedBuffItems.ToList();
		return true;
	}

	private bool UseBuff(string buffItemCode)
	{
		if (appliedBuffItems.Contains(buffItemCode) == false)
			return false;

		appliedBuffItems.Remove(buffItemCode);
		appliedBuffItemList = appliedBuffItems.ToList();

		return true;
	}

	private void InitializeBuffs(IEnumerable<string> buffItems)
	{
		if (buffItems == null)
			return;

		appliedBuffItems.Clear();

		foreach (var buffItem in buffItems)
		{
			ApplyBuffLast(buffItem);
		}
	}

	private void InitializeEquippedItems(IEnumerable<string> equippedItems, IEnumerable<string> appliedProfileItems)
	{
		this.appliedProfileItems = new string[2];
		this.equippedItems = new string[6];

		if (appliedProfileItems != null)
		{
			foreach (var item in appliedProfileItems)
			{
				EquipOnItem(item);
			}
		}

		foreach (var item in equippedItems)
		{
			EquipOnItem(item);
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
			int index = 5;
			if (index >= 0 && index < equippedItems.Length)
			{
				equippedItems[index] = itemCode;
			}
		}
		else
		{
			var partsType = itemTable.GetItemPartsType(itemCode);
			int index = GetIndexFromPartsType(partsType);

			if (index >= 0 && index < 5)
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

		InitializeEquippedItems(myData.playerData.equippedItems, myData.playerData.appliedProfileItems);
		InitializeBuffs(myData.appliedBuffItems);
	}
}
