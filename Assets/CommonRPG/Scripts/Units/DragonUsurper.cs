using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class DragonUsurper : BossMonster
    {
        private enum EAudioClipList
        {
            Walk1, Walk2, Walk3, Walk4, Walk5,
            Run1, Run2, Run3,
            Sleep1, Sleep2,
            BitesTarget,
            HandAttack,
            MouthAttack,
            BreathFire,
            Death,
            Wings1, Wings2, Wings3, Wings4,
            Landing1, Landing2,
            Awake,
            Hit1, Hit2, Hit3, Hit4, Hit5
        }

        [Header("MeleeAttack Settings")]
        [SerializeField]
        private DamageCollider mouthAttackCollider;

        [SerializeField]
        private DamageCollider handAttackCollider;

        [SerializeField]
        private float mouthAttackDamage = 5;

        [SerializeField]
        private float handAttackDamage = 4;

        [Header("FlameAttack Settings")]
        [SerializeField]
        private DamageCollider flameCollider;

        [SerializeField]
        private float flameDamage = 3;

        private DragonUserperDamageEventInfo damageEventInfo = new DragonUserperDamageEventInfo();

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(flameCollider);
        }

        protected override void Update()
        {
            base.Update();

            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;
            if (dragonUsurperAIController && dragonUsurperAIController.CurrentPhase == BossAIController.EAIPhase.Phase1) 
            {
                animController.CurrentMoveSpeed = 0.1f;
            }
            else
            {
                animController.CurrentMoveSpeed = aiController.CurrentSpeed;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;

            dragonUsurperAIController.OnWakeUpStartDelegate += OnWakeUpStart;
            dragonUsurperAIController.OnAttackWithMouthDelegate += OnAttakWithMouth;
            dragonUsurperAIController.OnAttackWithHandDelgate += OnAttackWithHand;
            dragonUsurperAIController.OnAttackFlameGroundDelgate += OnAttackFlameGround;
            dragonUsurperAIController.OnAttackAirFlameDelegate += OnAttackAirFlame;

            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.OnWakeUpEndedDelegate += OnWakeUpEnded;
            dragonUsurperAnimController.OnAttackCheckDelegate += DoDamage;
            dragonUsurperAnimController.OnBeginAttack += CheckBeginAttack;

            dragonUsurperAnimController.OnCheckMouthAttackDelegate += OnCheckMouthAttack;
            dragonUsurperAnimController.OnCheckHandAttackDelegate += OnCheckHandAttack;
            dragonUsurperAnimController.OnCheckAttackFlameDelegate += OnCheckFlameAttack;

            dragonUsurperAnimController.OnPlayingMoveSoundDelegate += PlayFootStepAudio;
            dragonUsurperAnimController.OnPlayingMoveSoundDelegate += PlayFlyingWingSound;
            dragonUsurperAnimController.OnPlayingLandSoundDelegate += PlayLandSound;
            dragonUsurperAnimController.OnPlayingHandAttackSoundDelegate += PlayHandAttackSound;
            dragonUsurperAnimController.OnPlayingMouthAttackSoundDelegate += PlayMouthAttackSound;
            dragonUsurperAnimController.OnPlayingFlameAttackSoundDelegate += PlayFlameAttackSound;
            dragonUsurperAnimController.OnPlayingSleepSoundDelegate += PlaySleepSound;
            dragonUsurperAnimController.OnPlayingDeathSoundDelegate += PlayDeathSound;
            dragonUsurperAnimController.OnPlayingHandAttackFrontLandingSoundDelegate += PlayHandAttackLandingSound;

            mouthAttackCollider.OnEnterDelgate += OnDamageMouthAttack;
            handAttackCollider.OnEnterDelgate += OnDamageHandAttack;
            flameCollider.OnStayDelegate += OnDamageFlameAttack;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;

            dragonUsurperAIController.OnWakeUpStartDelegate -= OnWakeUpStart;
            dragonUsurperAIController.OnAttackWithMouthDelegate -= OnAttakWithMouth;
            dragonUsurperAIController.OnAttackWithHandDelgate -= OnAttackWithHand;
            dragonUsurperAIController.OnAttackFlameGroundDelgate -= OnAttackFlameGround;
            dragonUsurperAIController.OnAttackAirFlameDelegate -= OnAttackAirFlame;

            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.OnWakeUpEndedDelegate -= OnWakeUpEnded;
            dragonUsurperAnimController.OnAttackCheckDelegate -= DoDamage;
            dragonUsurperAnimController.OnBeginAttack -= CheckBeginAttack;

            dragonUsurperAnimController.OnCheckMouthAttackDelegate -= OnCheckMouthAttack;
            dragonUsurperAnimController.OnCheckHandAttackDelegate -= OnCheckHandAttack;
            dragonUsurperAnimController.OnCheckAttackFlameDelegate -= OnCheckFlameAttack;

            dragonUsurperAnimController.OnPlayingMoveSoundDelegate -= PlayFootStepAudio;
            dragonUsurperAnimController.OnPlayingMoveSoundDelegate -= PlayFlyingWingSound;
            dragonUsurperAnimController.OnPlayingLandSoundDelegate -= PlayLandSound;
            dragonUsurperAnimController.OnPlayingHandAttackSoundDelegate -= PlayHandAttackSound;
            dragonUsurperAnimController.OnPlayingMouthAttackSoundDelegate -= PlayMouthAttackSound;
            dragonUsurperAnimController.OnPlayingFlameAttackSoundDelegate -= PlayFlameAttackSound;
            dragonUsurperAnimController.OnPlayingSleepSoundDelegate -= PlaySleepSound;
            dragonUsurperAnimController.OnPlayingDeathSoundDelegate -= PlayDeathSound;
            dragonUsurperAnimController.OnPlayingHandAttackFrontLandingSoundDelegate -= PlayHandAttackLandingSound;

            mouthAttackCollider.OnEnterDelgate -= OnDamageMouthAttack;
            handAttackCollider.OnEnterDelgate -= OnDamageHandAttack;
            flameCollider.OnStayDelegate -= OnDamageFlameAttack;
        }

        public override float TakeDamage(float DamageAmount, AUnit DamageCauser = null, ADamageEventInfo damageEventInfo = null)
        {
            if (IsDead)
            {
                return 0f;
            }

            float actualDamageAmount = DamageAmount - statComponent.TotalDefense;
            if (actualDamageAmount < 1)
            {
                actualDamageAmount = 1;
            }

            float beforeHpRatio = statComponent.CurrentHealthPoint / statComponent.TotalHealth;

            statComponent.CurrentHealthPoint -= actualDamageAmount;

            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            float afterimageChangeTime = 0.5f;

            GameManager.InGameUI.DisplayDecrasingMonsterHealthBar(currentHpRatio, beforeHpRatio, afterimageChangeTime, this);
            GameManager.InGameUI.SetMonsterInfoUIVisible(true);
            GameManager.InGameUI.SetMonsterNameText(base.unitName);

            float offsetY = 1;
            Vector3 damageDisplayPosition = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);

            GameManager.InGameUI.FloatDamageNumber(actualDamageAmount, damageDisplayPosition);

            if (monsterUITimerHandler == null)
            {
                monsterUITimerHandler = GameManager.TimerManager.SetTimer(3, 1, 0, () => {

                    GameManager.InGameUI.SetMonsterInfoUIVisible(false);

                }, true);

                monsterUITimerHandler.IsStayingActive = true;
            }
            else
            {
                monsterUITimerHandler.RestartTimer();
            }

            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;


            if (currentHpRatio <= 0.8f && dragonUsurperAIController.CurrentPhase < BossAIController.EAIPhase.Phase2)
            {
                dragonUsurperAIController.CurrentPhase = BossAIController.EAIPhase.Phase2;
                dragonUsurperAIController.CurrentAIState = DragonUsurperAIController.EAIState.Run;
            }
            else if (currentHpRatio <= 0.4f && dragonUsurperAIController.CurrentPhase < BossAIController.EAIPhase.Phase3) 
            {
                dragonUsurperAIController.CurrentPhase = BossAIController.EAIPhase.Phase3;
            }
            else if (currentHpRatio <= 0 && base.isDead == false)
            {
                BeKilled();

                dragonUsurperAIController.CurrentPhase = BossAIController.EAIPhase.Dead;

                if (DamageCauser is ACharacter)
                {
                    ACharacter character = (ACharacter)DamageCauser;

                    float obtainingExp = GameManager.GetMonsterData(monsterName).Data.Exp;
                    float expTolerance = GameManager.GetMonsterData(monsterName).Data.ExpTolerance;

                    float decidedExp = Random.Range(obtainingExp - expTolerance, obtainingExp + expTolerance);
                    character.ObtainExp(decidedExp);

                    int obtainingCoins = GameManager.GetMonsterData(MonsterName).Data.HoldingMoney;
                    int coinTolerance = GameManager.GetMonsterData(MonsterName).Data.MoneyTolerance;

                    GameManager.SetCoins(GameManager.GetCurrentCoins() + Random.Range(obtainingCoins - coinTolerance, obtainingCoins + coinTolerance));
                    GameManager.DropItemFromMonster(MonsterName, transform.position, transform.rotation);
                }
            }

            DragonUsurperAnimController monsterAnimController = (DragonUsurperAnimController)animController;

            if (statComponent.CurrentHealthPoint <= 0)
            {
                monsterAnimController.PlayDeathAnim();
                //OnKilled.Invoke(this, DamageCauser);
            }
            else
            {
                int audioClipIndex = Random.Range((int)EAudioClipList.Hit1, (int)EAudioClipList.Hit5 + 1);
                Debug.LogWarning(audioClipIndex);
                float hitSoundPitch = 0.7f;
                GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position, hitSoundPitch);
                //monsterAnimController.PlayHitAnim();
            }

            return actualDamageAmount;
        }

        private void OnWakeUpStart()
        {
            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.PlayWakeUpAnim();

            GameManager.AudioManager.StopAllAudio3Ds();
            PlayAwakeSound();
        }

        private void OnWakeUpEnded()
        {
            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;
            dragonUsurperAIController.CurrentPhase = BossAIController.EAIPhase.Phase1;
            dragonUsurperAIController.CurrentAIState = DragonUsurperAIController.EAIState.Walk;
        }

        private void OnAttakWithMouth()
        {
            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.PlayAttackMouth();
        }
        private void OnAttackWithHand()
        {
            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.PlayAttackHand();
        }

        private void OnAttackFlameGround(Transform lookTransform)
        {
            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            GameManager.TimerManager.SetTimer(1, 0.02f, 100, () =>
            {
                if (transform == null)
                {
                    return;
                }

                //transform.LookAt(lookTransform, transform.up);
                LookTarget(lookTransform, 10);

            }, true);

            GameManager.TimerManager.SetTimer(3, 0, 0, () =>
            {
                if (dragonUsurperAnimController == null) 
                {
                    return;
                }

                dragonUsurperAnimController.PlayAttackFlame_Ground();

            }, true);
        }

        private void OnAttackAirFlame(Transform lookTransform)
        {
            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            GameManager.TimerManager.SetTimer(0.1f, 0.02f, 100, () =>
            {
                if (transform == null) 
                {
                    return;
                }

                //transform.LookAt(lookTransform, transform.up);
                LookTarget(lookTransform, 10);

            }, true);

            GameManager.TimerManager.SetTimer(3, 0, 0, () =>
            {
                if (dragonUsurperAnimController == null)
                {
                    return;
                }

                dragonUsurperAnimController.TakeOff();

            }, true);

            GameManager.TimerManager.SetTimer(8, 5, 0, () =>
            {
                if (dragonUsurperAnimController == null)
                {
                    return;
                }

                if (base.aiController == null) 
                {
                    return;
                }

                DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;
                dragonUsurperAIController.SetFlyingDestination(lookTransform.position);

                dragonUsurperAnimController.PlayAttackFlame_Flying();

            }, true);

            GameManager.TimerManager.SetTimer(11, 0, 0, () =>
            {
                if (dragonUsurperAnimController == null)
                {
                    return;
                }

                dragonUsurperAnimController.Land();

            }, true);

            GameManager.TimerManager.SetTimer(14, 0, 0, () =>
            {
                if (base.aiController == null)
                {
                    return;
                }

                DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;

                dragonUsurperAIController.CurrentAIState = DragonUsurperAIController.EAIState.Run;
                dragonUsurperAIController.IsAttacking = false;
                
            }, true);
        }

        private void OnCheckMouthAttack(bool isAttacking)
        {
            mouthAttackCollider.SetActiveDamageCollider(isAttacking);
        }

        private void OnCheckHandAttack(bool isAttacking)
        {
            handAttackCollider.SetActiveDamageCollider(isAttacking);
        }

        private void OnCheckFlameAttack(bool isAttacking)
        {
            flameCollider.SetActiveDamageCollider(isAttacking);
        }

        private void OnDamageMouthAttack(IDamageable damagedUnit)
        {
            if (damagedUnit == null)
            {
                return;
            }

            damageEventInfo.damageType = DragonUserperDamageEventInfo.EAttackType.MouthAttack;

            damagedUnit.TakeDamage(flameDamage, this, damageEventInfo);
        }

        private void OnDamageHandAttack(IDamageable damagedUnit)
        {
            if (damagedUnit == null)
            {
                return;
            }

            damageEventInfo.damageType = DragonUserperDamageEventInfo.EAttackType.HandAttack;

            damagedUnit.TakeDamage(flameDamage, this, damageEventInfo);
        }

        private void OnDamageFlameAttack(IDamageable damagedUnit)
        {
            if (damagedUnit == null) 
            {
                return;
            }

            damageEventInfo.damageType = DragonUserperDamageEventInfo.EAttackType.FlameAttack;

            damagedUnit.TakeDamage(flameDamage, this, damageEventInfo);
        }

        private void PlayFootStepAudio(float currentSpeed, bool isFlying)
        {
            if (isFlying) 
            {
                return;
            }

            int audioClipIndex;

            if (currentSpeed < 0.5f) 
            {
                audioClipIndex = Random.Range((int)EAudioClipList.Walk1, (int)EAudioClipList.Walk5 + 1);
            }
            else
            {
                audioClipIndex = Random.Range((int)EAudioClipList.Run1, (int)EAudioClipList.Run3 + 1);
            }

            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlayFlyingWingSound(float currentSpeed, bool isFlying)
        {
            if (isFlying == false) 
            {
                return;
            }

            int audioClipIndex = Random.Range((int)EAudioClipList.Wings1, (int)EAudioClipList.Wings4 + 1);

            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlayLandSound(bool isLandingCompleted)
        {
            int audioClipIndex;

            if (isLandingCompleted) 
            {
                audioClipIndex = (int)EAudioClipList.Landing1;
            }
            else
            {
                audioClipIndex = Random.Range((int)EAudioClipList.Wings1, (int)EAudioClipList.Wings4 + 1);
            }

            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlayHandAttackSound()
        {
            int audioClipIndex = (int)EAudioClipList.HandAttack;
            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlayMouthAttackSound()
        {
            int audioClipIndex = (int)EAudioClipList.MouthAttack;
            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlayFlameAttackSound()
        {
            int audioClipIndex = (int)EAudioClipList.BreathFire;
            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlaySleepSound()
        {
            int audioClipIndex = Random.Range((int)EAudioClipList.Sleep1, (int)EAudioClipList.Sleep2 + 1);
            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlayDeathSound()
        {
            int audioClipIndex = (int)EAudioClipList.Death;
            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlayAwakeSound()
        {
            int audioClipIndex = (int)EAudioClipList.Awake;
            GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioClipIndex], 1, transform.position);
        }

        private void PlayHandAttackLandingSound(bool isFront)
        {
            if (isFront) 
            {
                GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[(int)EAudioClipList.Landing2], 1, transform.position);
            }
            else
            {
                GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[(int)EAudioClipList.Landing1], 1, transform.position);
            }
        }

        private void DoDamage(bool isStartingAttackCheck)
        {
            Debug.Log($"damage : {isStartingAttackCheck}");
        }

        private void CheckBeginAttack(bool BeganAttack)
        {
            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;
            dragonUsurperAIController.IsAttacking = BeganAttack;
        }

        private void LookTarget(Transform targetTransform, float rotationSpeed)
        {
            Vector3 actualForward = Vector3.Lerp(transform.forward, (targetTransform.position - transform.position).normalized, Time.deltaTime * rotationSpeed);
            transform.forward = actualForward;
        }
    }
}
