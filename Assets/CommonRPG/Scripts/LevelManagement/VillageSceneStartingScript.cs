using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class VillageSceneStartingScript : ALevelStartingScript
    {
        [SerializeField]
        private Vector3 itemSpawnPosition;

        private EquipmentScreen equipmentScreen = null;
        public override void StartScript()
        {
            GameManager.SpawnItem(EItemName.TheFirstSword, itemSpawnPosition, Quaternion.identity, true);

            if (equipmentScreen == null)
            {
                equipmentScreen = (EquipmentScreen)GameManager.InventoryManager.InventoryList[(int)EInventoryType.EquipmentScreen];
            }

            if (equipmentScreen.WeaponEquipmentTransform == null) 
            {
                equipmentScreen.WeaponEquipmentTransform = GameManager.GetPlayer().WeaponEquipmentTransform;
            }

            if (equipmentScreen.ShieldEquipmentTransform = null) 
            {
                equipmentScreen.ShieldEquipmentTransform = GameManager.GetPlayer().ShieldEquipmentTransform;
            }

            GameManager.InGameUI.SetPlayerInfoUIVisible(true);
        }
    }
}
