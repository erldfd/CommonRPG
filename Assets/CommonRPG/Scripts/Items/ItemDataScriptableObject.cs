using CommonRPG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EItemName
{
    None = -1,
    TheFirstSword = 0,
    TheSecondSword,
    Weapon3,
}

[Flags]
public enum EItemType
{
    None = 0,
    Weapon = 1,
    Shield = 2,
    Misc = 4,
}

public enum EItemGrade
{
    E,
    D,
    C,
    B,
    A,
    S
}

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemDataScriptableObject", order = 1)]
public class ItemDataScriptableObject : ScriptableObject
{
    [SerializeField]
    private List<ItemData> itemDataList;
    public List<ItemData> ItemDataList { get { return itemDataList; } }

    public void Awake()
    {
        itemDataList.Sort();
    }
}

[Serializable]
public class ItemData : IComparable<ItemData>
{
    [SerializeField]
    private string name = null;

    [SerializeField]
    private SItemData data;
    public SItemData Data { get { return data; } }

    [SerializeField]
    private AItem itemPrefab = null;
    public AItem ItemPrefab { get { return itemPrefab; } }

    public int CompareTo(ItemData other)
    {
        if (other == null || (int)data.ItemName > (int)other.data.ItemName) 
        {
            return 1;
        }
        else if ((int)data.ItemName == (int)other.data.ItemName)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
}

[Serializable]
public struct SItemData
{
    public EItemName ItemName;
    public EItemType ItemType;
    public EItemGrade ItemGrade;
    public Sprite SlotSprite;
    public int MaxItemCount;
    public float Damage;
    public float Defense;
    public float HPBonus;
    public float MPBonus;
}