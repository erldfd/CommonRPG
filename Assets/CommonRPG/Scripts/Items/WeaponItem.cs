using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CommonRPG
{
    public class WeaponItem : AItem
    {

        private BoxCollider boxCollider = null;
        private HashSet<IDamageable> hitMonsterSet = new HashSet<IDamageable>();

        protected override void Awake()
        {
            base.Awake();

            boxCollider = GetComponent<BoxCollider>();
            Debug.Assert(boxCollider);
            boxCollider.enabled = false;
        }

        public override void EnableCollider(bool ShouldEnable)
        {
            if (boxCollider.IsDestroyed()) 
            {
                return;
            }

            boxCollider.enabled = ShouldEnable;
            if (ShouldEnable == false) 
            {
                hitMonsterSet.Clear();
            }
        }

        public void PlaySwingSound()
        {
            int audioClipCount = audioContainer.AudioClipList.Count;

            if (audioClipCount == 0) 
            {
                return;
            }

            int audioClipIndex = Random.Range(0, audioClipCount);
            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (base.IsFieldItem) 
            {
                return;
            }

            IDamageable monster = other.transform.root.GetComponent<IDamageable>();
            if (monster == null) 
            {
                return;
            }

            if (hitMonsterSet.Contains(monster)) 
            {
                return;
            }

            hitMonsterSet.Add(monster);

            monster.TakeDamage(GameManager.GetPlayerCharacter().StatComponent.TotalAttackPower, GameManager.GetPlayerCharacter());
        }
    }

}
