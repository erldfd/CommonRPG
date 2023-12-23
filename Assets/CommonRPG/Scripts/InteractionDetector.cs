using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField]
        private CapsuleCollider interactionCollider = null;

        private HashSet<AItem> interactionFieldItemSet = new HashSet<AItem>();
        private HashSet<Merchant> interactionMerchantSet = new HashSet<Merchant>();

        private void Awake()
        {
            Debug.Assert(interactionCollider);
        }

        private void OnTriggerEnter(Collider other)
        {
            AItem item = other.GetComponent<AItem>();
            if (item) 
            {
                if (interactionFieldItemSet.Contains(item))
                {
                    return;
                }

                interactionFieldItemSet.Add(item);
                GameManager.SetActiveInteractioUI(true);

                return;
            }

            Merchant merchant = other.GetComponent<Merchant>();
            if (merchant)
            {
                if (interactionMerchantSet.Contains(merchant))
                {
                    return;
                }

                interactionMerchantSet.Add(merchant);
                GameManager.SetActiveInteractioUI(true);

                return;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            AItem item = other.GetComponent<AItem>();
            if (item) 
            {
                if (interactionFieldItemSet.Contains(item) == false)
                {
                    Debug.LogAssertion("Why is this item not contained in set?");
                    return;
                }

                interactionFieldItemSet.Remove(item);

                if (interactionFieldItemSet.Count == 0)
                {
                    GameManager.SetActiveInteractioUI(false);
                }

                return;
            }

            Merchant merchant = other.GetComponent<Merchant>();
            if (merchant) 
            {
                if (interactionMerchantSet.Contains(merchant) == false)
                {
                    Debug.LogAssertion("Wrong access");
                    return;
                }

                interactionMerchantSet.Remove(merchant);

                if (interactionMerchantSet.Count == 0)
                {
                    GameManager.SetActiveInteractioUI(false);
                    GameManager.InventoryManager.OpenAndCloseMerchantInventory(false);
                }

                return;
            }    
            
        }

        public void Interact()
        {
            if (interactionFieldItemSet.Count > 0) 
            {
                foreach (AItem item in interactionFieldItemSet)
                {
                    int itemAddFailCount = 0;

                    if (item.Data.ItemType == EItemType.Weapon)
                    {
                        itemAddFailCount = GameManager.InventoryManager.ObtainItem(EInventoryType.Equipment, 1, item.Data);
                    }
                    else if (item.Data.ItemType == EItemType.Misc)
                    {
                        itemAddFailCount = GameManager.InventoryManager.ObtainItem(EInventoryType.MiscItemInventory, 1, item.Data);
                    }

                    if (itemAddFailCount == 0)
                    {
                        interactionFieldItemSet.Remove(item);
                        Destroy(item.gameObject);
                    }

                    break;
                }

                if (interactionFieldItemSet.Count == 0)
                {
                    GameManager.SetActiveInteractioUI(false);
                }

                return;
            }

            if (interactionMerchantSet.Count > 0) 
            {
                foreach (Merchant merchant in interactionMerchantSet)
                {
                    List<InventorySlotItemData> merchantGoodsDataList = merchant.MerchantGoodsDataList;
                    int merchantGoodsDataListCount = merchantGoodsDataList.Count;

                    GameManager.InventoryManager.DisplayMerchantGoods(merchantGoodsDataList);
                    GameManager.InventoryManager.OpenAndCloseMerchantInventory(true);
                    GameManager.SetActiveInteractioUI(false);

                    break;
                }

                return;
            }
        }

        public void InitInteraction()
        {
            interactionFieldItemSet.Clear();
            interactionMerchantSet.Clear();
        }
    }
}
