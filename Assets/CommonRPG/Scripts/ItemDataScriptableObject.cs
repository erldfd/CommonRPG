using CommonRPG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EItemName
{
    TheFirstSword,
    Weapon2,
    Weapon3,
}

public enum EItemType
{
    Weapon,
    Shield,
    Misc
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
}

[Serializable]
public class ItemData
{
    [SerializeField]
    private string name = null;

    [SerializeField]
    private SInventorySlotItemData slotData;
    public SInventorySlotItemData SlotData { get { return slotData; } }

    [SerializeField]
    private AItem item = null;
    public AItem Item { get { return item; } }
}

[Serializable]
public struct SInventorySlotItemData
{
    public EItemName ItemName;
    public EItemType ItemType;
    public EItemGrade ItemGrade;
    public Sprite SlotSprite;
    public int MaxItemCount;
    [HideInInspector]
    public int CurrentItemCount;
    public float Damage;
    public float Defense;
    public float HPBonus;
    public float MPBonus;
}