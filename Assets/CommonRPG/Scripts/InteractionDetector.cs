using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class InteractionDetector : MonoBehaviour
    {
        private enum EAudioClipList
        {
            PickUpItem,
            Interaction
        }

        [SerializeField]
        private CapsuleCollider interactionCollider;

        [SerializeField]
        private AudioContainer interactionSoundContainer;

        private HashSet<MonoBehaviour> interactionSet = new HashSet<MonoBehaviour>();

        private void Awake()
        {
            Debug.Assert(interactionCollider);
            Debug.Assert(interactionSoundContainer);
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


            NPC nPC = other.GetComponent<NPC>();
            if (nPC) 
            {
                if (interactionSet.Contains(nPC)) 
                {
                    return;
                }

                interactionSet.Add(nPC);
                GameManager.SetActiveInteractioUI(true);
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

            NPC nPC = other.GetComponent<NPC>();
            if(nPC)
            {
                if (interactionSet.Contains(nPC) == false)
                {
                    Debug.LogAssertion("Wrong access");
                    return;
                }

                interactionSet.Remove(nPC);

                if (nPC is Merchant)
                {
                    GameManager.InventoryManager.OpenAndCloseMerchantInventory(false);
                }
                else if (nPC is CraftingStation) 
                {
                    GameManager.InventoryManager.OpenAndCloseCraftInventory(false);
                }

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
                        GameManager.AudioManager.PlayAudio2D(interactionSoundContainer.AudioClipList[(int)EAudioClipList.PickUpItem], 1);
                    }
                    else if (item.Data.ItemType == EItemType.Misc)
                    {
                        itemAddFailCount = GameManager.InventoryManager.ObtainItem(EInventoryType.MiscItemInventory, 1, item.Data);
                        GameManager.AudioManager.PlayAudio2D(interactionSoundContainer.AudioClipList[(int)EAudioClipList.PickUpItem], 1);
                    }

                    if (itemAddFailCount == 0)
                    {
                        interactionSet.Remove(item);
                        Destroy(item.gameObject);
                    }

                    break;
                }
                else if (someObject is NPC) 
                {
                    NPC npc = (NPC)someObject;
                    npc.InteractWithPlayer();
                    GameManager.AudioManager.PlayAudio2D(interactionSoundContainer.AudioClipList[(int)EAudioClipList.Interaction], 1);
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
