using Cinemachine.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class EquipmentScreen : AInventory
    {
        public enum EEquipmentSlot
        {
            Weapon,
            Shield
        }

        [SerializeField]
        private Transform weaponEquipmentTransform;
        public Transform WeaponEquipmentTransform { get { return weaponEquipmentTransform; } set { weaponEquipmentTransform = value; } }

        [SerializeField]
        private Transform shieldEquipmentTransform;
        public Transform ShieldEquipmentTransform { get { return shieldEquipmentTransform; } set { shieldEquipmentTransform = value; } }

        protected override void Awake()
        {
            base.Awake();
        }

        public override void InitInventory()
        {
            base.InitInventory();

            int slotUIsLength = base.slotUiList.Count;
            for (int i = 0; i < slotUIsLength; i++)
            {
                base.slotUiList[i].CurrentSlotInventoryType = inventoryType;
            }

            base.slotUiList[(int)EEquipmentSlot.Weapon].AllowedItemType = EItemType.Weapon;
            base.slotUiList[(int)EEquipmentSlot.Shield].AllowedItemType = EItemType.Shield;
        }

        public override void SetItemInSlot(int slotIndex, int newItemCount, in SItemData itemData)
        {
            base.SetItemInSlot(slotIndex, newItemCount, itemData);
            SetItemInSlot((EEquipmentSlot)slotIndex, newItemCount, itemData);
        }

        public void SetItemInSlot(EEquipmentSlot equipmentSlot, int newItemCount, in SItemData itemData)
        {
            bool WillMakeSlotEmpty = (newItemCount == 0);

            RemoveEquipment();

            if (WillMakeSlotEmpty)
            {
                GameManager.UpdateStatWindow();
                return;
            }

            if (equipmentSlot == EEquipmentSlot.Weapon && itemData.ItemType == EItemType.Weapon)
            {
                GameManager.GetPlayerCharacter().CharacterWeapon = (WeaponItem)GameManager.SpawnItem(itemData.ItemName, weaponEquipmentTransform, false);
            }
            else if (equipmentSlot == EEquipmentSlot.Shield && itemData.ItemType == EItemType.Shield)
            {
                GameManager.SpawnItem(itemData.ItemName, shieldEquipmentTransform, false);
            }
            else
            {
                Debug.LogAssertion("Weird equiment is detected");
            }

            GameManager.UpdateStatWindow();

            void RemoveEquipment()
            {
                if (equipmentSlot == EEquipmentSlot.Weapon)
                {
                    if (weaponEquipmentTransform.childCount > 0)
                    {
                        for (int i = 0; i < weaponEquipmentTransform.childCount; i++)
                        {
                            Destroy(weaponEquipmentTransform.GetChild(0).gameObject);
                        }
                    }

                    GameManager.GetPlayerCharacter().CharacterWeapon = null;
                }
                else if (equipmentSlot == EEquipmentSlot.Shield)
                {
                    if (shieldEquipmentTransform.childCount > 0)
                    {
                        for (int i = 0; i < shieldEquipmentTransform.childCount; i++)
                        {
                            Destroy(shieldEquipmentTransform.GetChild(0).gameObject);
                        }
                    }
                }
            }
        }

        public override void SetSlotItemCount(int slotIndex, int newItemCount)
        {
            base.SetSlotItemCount(slotIndex, newItemCount);
            slotUiList[slotIndex].SetSlotItemCountText("");
        }

        /// <summary>
        /// This method is overridden. Actual function is Unequipping item
        /// </summary>
        public override void AbandonItem(int slotIndex)
        {
            int emptySlotIndex = GameManager.InventoryManager.GetEmptySlotInex(EInventoryType.Equipment);
            if (emptySlotIndex == -1) 
            {
                Debug.Log("Inventory is full");
                return;
            }

            GameManager.InventoryManager.ExchangeOrMoveOrMergeItem(slotIndex, emptySlotIndex, InventoryType, EInventoryType.Equipment);
        }
    }
}

