using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CommonRPG
{
    public class WeaponItem : AItem
    {
        private float weaponDamage = 1;
        private BoxCollider boxCollider = null;
        private HashSet<IDamageable> hitMonsterSet = new HashSet<IDamageable>();

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            Debug.Assert(boxCollider);
        }

        public void EnableCollider(bool ShouldEnable)
        {
            boxCollider.enabled = ShouldEnable;
            if (ShouldEnable == false) 
            {
                hitMonsterSet.Clear();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (base.IsFieldItem) 
            {
                return;
            }

            IDamageable monster = other.GetComponent<IDamageable>();
            if (monster == null) 
            {
                return;
            }

            if (hitMonsterSet.Contains(monster)) 
            {
                return;
            }

            hitMonsterSet.Add(monster);

            monster.TakeDamage(weaponDamage);
        }
    }

}
