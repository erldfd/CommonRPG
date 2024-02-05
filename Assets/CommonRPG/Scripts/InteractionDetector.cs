using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace CommonRPG
{
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField]
        private CapsuleCollider interactionCollider = null;

        //private HashSet<AItem> interactionFieldItemSet = new HashSet<AItem>();
        //private HashSet<Merchant> interactionMerchantSet = new HashSet<Merchant>();
        private HashSet<MonoBehaviour> interactionSet = new HashSet<MonoBehaviour>();

        private void Awake()
        {
            Debug.Assert(interactionCollider);
        }

        private void OnTriggerEnter(Collider other)
        {
            AItem item = other.GetComponent<AItem>();
            if (item) 
            {
                if (interactionSet.Contains(item))
                {
                    return;
                }

                interactionSet.Add(item);
                GameManager.SetActiveInteractioUI(true);

                return;
            }

            Merchant merchant = other.GetComponent<Merchant>();
            if (merchant)
            {
                if (interactionSet.Contains(merchant))
                {
                    return;
                }

                interactionSet.Add(merchant);
                GameManager.SetActiveInteractioUI(true);

                return;
            }

            CraftingStation craftingStation = other.GetComponent<CraftingStation>();
            if (craftingStation) 
            {
                if (interactionSet.Contains(craftingStation))
                {
                    return;
                }

                interactionSet.Add(craftingStation);
                GameManager.SetActiveInteractioUI(true);

                return;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            AItem item = other.GetComponent<AItem>();
            if (item) 
            {
                if (interactionSet.Contains(item) == false)
                {
                    Debug.LogAssertion("Why is this item not contained in set?");
                    return;
                }

                interactionSet.Remove(item);

                if (interactionSet.Count == 0)
                {
                    GameManager.SetActiveInteractioUI(false);
                }

                return;
            }

            Merchant merchant = other.GetComponent<Merchant>();
            if (merchant) 
            {
                if (interactionSet.Contains(merchant) == false)
                {
                    Debug.LogAssertion("Wrong access");
                    return;
                }

                interactionSet.Remove(merchant);
                GameManager.InventoryManager.OpenAndCloseMerchantInventory(false);

                if (interactionSet.Count == 0)
                {
                    GameManager.SetActiveInteractioUI(false);
                }

                return;
            }  
            
            CraftingStation craftingStation = other.GetComponent<CraftingStation>();
            if (craftingStation) 
            {
                if (interactionSet.Contains(craftingStation) == false)
                {
                    Debug.LogAssertion("Wrong access");
                    return;
                }

                interactionSet.Remove(craftingStation);
                GameManager.InventoryManager.OpenAndCloseCraftInventory(false);

                if (interactionSet.Count == 0)
                {
                    GameManager.SetActiveInteractioUI(false);
                }

                return;
            }
            
        }

        public void Interact()
        {
            if (interactionSet.Count == 0) 
            {
                return;
            }

            foreach (MonoBehaviour someObject in interactionSet) 
            {
                if (someObject is AItem)
                {
                    AItem item = (AItem)someObject;

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
                        interactionSet.Remove(item);
                        Destroy(item.gameObject);
                    }

                    break;
                }
                else if (someObject is Merchant) 
                {
                    Merchant merchant = (Merchant)someObject;

                    List<InventorySlotItemData> merchantGoodsDataList = merchant.MerchantGoodsDataList;

                    GameManager.InventoryManager.DisplayMerchantGoods(merchantGoodsDataList);
                    GameManager.InventoryManager.OpenAndCloseMerchantInventory(true);
                    GameManager.SetActiveInteractioUI(false);

                    break;
                }
                else if (someObject is CraftingStation)
                {
                    CraftingStation craftingStation = (CraftingStation)someObject;
                    //GameManager.InventoryManager.OpenAndCloseCraftInventory(true);

                    GameManager.InGameUI.ReadyToConversate(craftingStation.ConversationData);
                    GameManager.SetActiveInteractioUI(false);

                    break;
                }
            }

            if (interactionSet.Count == 0)
            {
                GameManager.SetActiveInteractioUI(false);
            }
        }

        public void InitInteraction()
        {
            interactionSet.Clear();
        }
    }
}
