using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField]
        private CapsuleCollider interactionCollider = null;

        private HashSet<AItem> itemSet = new HashSet<AItem>();

        private void Awake()
        {
            Debug.Assert(interactionCollider);
        }

        private void OnTriggerEnter(Collider other)
        {
            AItem item = other.GetComponent<AItem>();
            if (itemSet.Contains(item)) 
            {
                return;
            }

            itemSet.Add(item);
            GameManager.SetActiveInteractioUI(true);
        }

        private void OnTriggerExit(Collider other)
        {
            AItem item = other.GetComponent<AItem>();
            if (itemSet.Contains(item) == false)
            {
                Debug.LogAssertion("Why is this item not contained in set?");
                return;
            }

            itemSet.Remove(item);

            if (itemSet.Count == 0) 
            {
                GameManager.SetActiveInteractioUI(false);
            }
        }

        public void Interact()
        {
            if (itemSet.Count == 0) 
            {
                return;
            }

            foreach (AItem item in itemSet) 
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
                    itemSet.Remove(item);
                    Destroy(item.gameObject);
                }

                break;
            }

            if (itemSet.Count == 0) 
            {
                GameManager.SetActiveInteractioUI(false);
            }
        }

        public void InitInteraction()
        {
            itemSet.Clear();
        }
    }
}
