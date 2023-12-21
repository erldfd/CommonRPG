using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public enum EInventoryType
    {
        None = -1,
        Equipment = 0,
        EquipmentScreen = 1,
        MiscItemInventory,
    }

    public abstract class AInventory : MonoBehaviour, IComparable<AInventory>
    {
        [SerializeField]
        protected EInventoryType inventoryType = EInventoryType.None;
        public EInventoryType InventoryType { get { return inventoryType; } }

        [SerializeField]
        protected EItemType allowedItemType = EItemType.None;

        [SerializeField]
        protected List<InventorySlotUI> slotUiList = null;
        public List<InventorySlotUI> SlotUiList { get { return slotUiList; } }

        protected List<InventorySlotItemData> inventoryItemDataList = null;
        public List<InventorySlotItemData> InventoryItemDataList { get { return inventoryItemDataList; } }

        protected virtual void Awake()
        {
            Debug.Assert(slotUiList != null && slotUiList.Count > 0);
        }

        public virtual void InitInventory()
        {
            int slotUIsLength = slotUiList.Count;
            for (int i = 0; i < slotUIsLength; i++)
            {
                slotUiList[i].SlotIndex = i;
                slotUiList[i].IsEmpty = true;
            }

            inventoryItemDataList = new List<InventorySlotItemData>(slotUIsLength);
            for (int i = 0; i < slotUIsLength; i++)
            {
                inventoryItemDataList.Add(new InventorySlotItemData());
            }
        }

        public virtual void SetItemInSlot(int slotIndex, InventorySlotItemData slotItemData)
        {
            SetItemInSlot(slotIndex, slotItemData.CurrentItemCount, slotItemData.ItemData);
        }

        public virtual void SetItemInSlot(int slotIndex, int newItemCount, in SItemData itemData)
        {
            inventoryItemDataList[slotIndex].ItemData = itemData;

            Sprite slotSprite = (newItemCount == 0) ? null : itemData.SlotSprite;

            slotUiList[slotIndex].SetSlotImageSprite(slotSprite);
            SetSlotItemCount(slotIndex, newItemCount);
        }

        public virtual void SetSlotItemCount(int slotIndex, int newItemCount)
        {
            if (newItemCount == 0)
            {
                slotUiList[slotIndex].SetSlotImageSprite(null);
            }

            inventoryItemDataList[slotIndex].CurrentItemCount = newItemCount;
            slotUiList[slotIndex].SetSlotItemCountText(newItemCount);
        }

        public virtual void ObtainItem(int slotIndex, int itemAddCount, in SItemData itemData)
        {
            Debug.AssertFormat(slotIndex < inventoryItemDataList.Count, $"slotIndex : {slotIndex}, equipmentItemDataList.Count : {inventoryItemDataList.Count}");

            int totalItemCount = inventoryItemDataList[slotIndex].CurrentItemCount + itemAddCount;
            if (totalItemCount > itemData.MaxItemCount)
            {
                Debug.LogWarning($"Too many items... cant obtain items.. CurrentItemCount : {inventoryItemDataList[slotIndex].CurrentItemCount}, itemAddCount : {itemAddCount}, MaxItemCount : {itemData.MaxItemCount}");
                return;
            }

            SetItemInSlot(slotIndex, totalItemCount, itemData);
        }
        /// <summary>
        ///  add item to inventory automatically as much as itemAddcount until inventory has no space.
        /// </summary>
        /// <returns> remaining item count after adding items until inventory is filled.</returns>
        public virtual int ObtainItem(int itemAddCount, in SItemData itemData)
        {
            int TotalItemAddCount = itemAddCount;

            int inventoryItemdataListCount = inventoryItemDataList.Count;
            for (int i = 0; i < inventoryItemdataListCount; ++i)
            {
                InventorySlotItemData data = inventoryItemDataList[i];

                if (data.ItemData.ItemName != itemData.ItemName && data.ItemData.ItemName != EItemName.None)
                {
                    continue;
                }

                if (data.ItemData.MaxItemCount == data.CurrentItemCount && data.ItemData.ItemName != EItemName.None)
                {
                    continue;
                }

                if (data.ItemData.ItemName == EItemName.None)
                {
                    data.ItemData.MaxItemCount = itemData.MaxItemCount;
                }

                int addableItemCount = data.ItemData.MaxItemCount - data.CurrentItemCount;

                int spareItemCount = TotalItemAddCount - addableItemCount;
                if (spareItemCount > 0)
                {
                    data.CurrentItemCount = itemData.MaxItemCount;
                    TotalItemAddCount = spareItemCount;
                }
                else
                {
                    data.CurrentItemCount += TotalItemAddCount;
                    TotalItemAddCount = 0;
                }

                SetItemInSlot(i, data.CurrentItemCount, itemData);

                if (TotalItemAddCount == 0)
                {
                    return 0;
                }
            }

            return TotalItemAddCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>if inventory is full, return -1</returns>
        public int GetEmptySlotIndex()
        {
            int invetoryItemDtatListCount = InventoryItemDataList.Count;
            for (int i = 0; i < invetoryItemDtatListCount; ++i) 
            {
                if (InventoryItemDataList[i].CurrentItemCount == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public virtual void DeleteItem(int slotIndex, int deleteCount)
        {
            if (deleteCount >= inventoryItemDataList[slotIndex].CurrentItemCount)
            {
                inventoryItemDataList[slotIndex].ItemData.ItemName = EItemName.None;
                SetSlotItemCount(slotIndex, 0);
            }
            else
            {
                SetSlotItemCount(slotIndex, inventoryItemDataList[slotIndex].CurrentItemCount - deleteCount);
            }
        }

        public virtual void AbandonItem(int slotIndex)
        {
            if (inventoryItemDataList[slotIndex].ItemData.ItemName == EItemName.None) 
            {
                return;
            }

            GameManager.SpawnItem(inventoryItemDataList[slotIndex].ItemData.ItemName, GameManager.GetPlayer().transform.position, GameManager.GetPlayer().transform.rotation, true);
            DeleteItem(slotIndex, 1);
        }

        public virtual void UseSlotItem(int slotIndex)
        {

        }

        public virtual void PaintSlotImage(int slotIndex, Sprite sprite)
        {
            slotUiList[slotIndex].SetSlotImageSprite(sprite);
        }

        public virtual void SetItemCountText(int slotIndex, int itemCount)
        {
            slotUiList[slotIndex].SetSlotItemCountText(itemCount);
        }

        public virtual bool CheckAllowedItemInInventory(EItemType checkingItemType)
        {
            return ((allowedItemType & checkingItemType) != 0);
        }

        public virtual bool CheckAllowedItemInSlot(int slotIndex, EItemType checkingItemType)
        {
            return ((slotUiList[slotIndex].AllowedItemType & checkingItemType) != 0);
        }

        public virtual void SortSlotItem()
        {
        }

        public int CompareTo(AInventory other)
        {
            if (other == null || (int)InventoryType > (int)other.InventoryType)
            {
                return 1;
            }
            else if ((int)InventoryType == (int)other.InventoryType)
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
    public class InventorySlotItemData : IComparable<InventorySlotItemData>
    {
        [SerializeField]
        private SItemData itemData = new SItemData();
        public ref SItemData ItemData { get { return ref itemData; } }

        private int currnentItemCount = 0;
        public int CurrentItemCount
        {
            get { return currnentItemCount; }
            set
            {
                currnentItemCount = value;
            }
        }

        public void CopyFrom(InventorySlotItemData previousData)
        {
            CopyFrom(previousData.ItemData, previousData.CurrentItemCount);
        }

        public void CopyFrom(in SItemData previousData, int previousItemCount)
        {
            itemData = previousData;
            currnentItemCount = previousItemCount;
        }

        public void UseSlotItem()
        {
            if (CurrentItemCount <= 0) 
            {
                return;
            }
        }

        public int CompareTo(InventorySlotItemData other)
        {
            if (other == null || other.CurrentItemCount == 0) 
            {
                return -1;
            }

            if (itemData.ItemGrade < other.itemData.ItemGrade) 
            {
                return 1;
            }    
            else if (itemData.ItemGrade == other.itemData.ItemGrade)
            {
                if (itemData.ItemName < other.itemData.ItemName) 
                {
                    return 1;
                }
                else if(itemData.ItemName == other.itemData.ItemName)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
