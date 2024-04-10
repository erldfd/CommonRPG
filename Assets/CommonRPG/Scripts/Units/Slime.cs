using UnityEngine;

namespace CommonRPG
{
    public class Slime : NormalMonster
    {
        private enum EAudioClipList
        {
            Hit1, Hit2, Hit3, Hit4, Hit5
        }

        protected override void Awake()
        {
            base.Awake();
            SlimeAnimController slimeAnimController = (SlimeAnimController)animController;
            Debug.Assert(slimeAnimController);
        }

        protected override void Update()
        {
            base.Update();
        }

        public override float TakeDamage(float DamageAmount, AUnit DamageCauser = null, ADamageEventInfo damageEventInfo = null)
        {
            float actualDamage = base.TakeDamage(DamageAmount, DamageCauser, damageEventInfo);

            int audioIndex = Random.Range((int)EAudioClipList.Hit1, (int)EAudioClipList.Hit5 + 1);
            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioIndex], 1, transform.position);

            return actualDamage;
        }
    }
}
