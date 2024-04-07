using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace CommonRPG
{
    public class DragonUsurperAnimController : AAnimController
    {
        private enum ELayer
        {
            Land,
            Flying,
            NeckHead
        }

        private enum EMoveState
        {
            Walk,
            Run,
            Fly
        }

        public event Action OnWakeUpEndedDelegate = null;
        public event Action<bool> OnCheckMouthAttackDelegate = null;
        public event Action<bool> OnCheckHandAttackDelegate = null;
        public event Action<bool> OnCheckAttackFlameDelegate = null;
        
        /// <summary>
        /// float : currentMoveSpeed, bool : isFlying
        /// </summary>
        public event Action<float, bool> OnPlayingMoveSoundDelegate = null;

        /// <summary>
        /// bool : need LandingCompleted Sound?
        /// </summary>
        public event Action<bool> OnPlayingLandSoundDelegate = null;

        public event Action OnPlayingHandAttackSoundDelegate = null;
        public event Action OnPlayingMouthAttackSoundDelegate = null;
        public event Action OnPlayingFlameAttackSoundDelegate = null;
        public event Action OnPlayingSleepSoundDelegate = null;
        public event Action OnPlayingDeathSoundDelegate = null;

        /// <summary>
        /// bool : begunAttack..
        /// </summary>
        public event Action<bool> OnBeginAttack = null;

        [SerializeField]
        private string takeOffAnimName;

        [SerializeField]
        private string landingAnimName;

        [SerializeField]
        private string attackMouthAnimName;

        [SerializeField]
        private string attackHandAnimName;

        [SerializeField]
        private string attackFlameAnimName_Ground;

        [SerializeField]
        private string attackFlameAnimName_Air;

        [SerializeField]
        private Transform flameMouthTransform;

        /// <summary>
        ///  sleep rate 1 -> play sleep anim, sleep rate 0 -> play idle or move anim
        /// </summary>
        protected float sleepRate = 1;

        private bool isAttacking = false;

        public float SleepRate
        {
            get { return sleepRate; }
            set
            {
                sleepRate = value;
                base.animator.SetFloat("Sleep", value);
            }
        }

        /// <summary>
        /// idle rate 1 -> play idle anim, idle rate 0 -> play move(run or walk) anim
        /// </summary>
        protected float idleRate = 0;
        public float IdleRate
        {
            get { return idleRate; }
            set
            {
                idleRate = value;
                base.animator.SetFloat("Idle", value);
            }
        }

        public override float CurrentMoveSpeed
        {
            get { return currentMoveSpeed; }
            set
            {
                currentMoveSpeed = value;
                animator.SetFloat("CurrentMoveSpeed", Mathf.Clamp01(value));
                ChangeIdleToMove(currentMoveSpeed > 0);
            }
        }

        private bool isWakingUp = false;
        private float wakeUpTime = 1;
        private float elapsedTime_WakeUp = 0;

        private bool isStartingChange = false;
        private bool shouldChangeToMove = false;
        private float idleToMoveChangeTime = 1;
        private float elapsedTime_ChangeTime = 0;

        private ParticleSystem flameVFX = null;

        private bool isUsingFlyingLayer = false;

        protected override void Awake()
        {
            base.Awake();

            //GameManager.TimerManager.SetTimer(2, 16, -1, () =>
            //{
            //    TakeOff();

            //}, true);

            //GameManager.TimerManager.SetTimer(1, 16, 0, () =>
            //{
            //    PlayWakeUpAnim();

            //}, true);

            //GameManager.TimerManager.SetTimer(5, 5, -1, () =>
            //{
            //    PlayAttackFlame_Ground();

            //}, true);
        }

        protected void Update()
        {
            if (isWakingUp) 
            {
                elapsedTime_WakeUp += Time.deltaTime;

                SleepRate = 1 - elapsedTime_WakeUp / wakeUpTime;

                if (elapsedTime_WakeUp >= wakeUpTime) 
                {
                    elapsedTime_WakeUp = 0;
                    isWakingUp = false;
                    OnWakeUpEndedDelegate?.Invoke();
                }
            }
            else if (isStartingChange) 
            {
                float changeSpeed = 2;
                elapsedTime_ChangeTime += changeSpeed * Time.deltaTime;

                if (shouldChangeToMove) 
                {
                    if (IdleRate > 0) 
                    {
                        IdleRate = 1 - elapsedTime_ChangeTime / idleToMoveChangeTime;
                    }
                }
                else
                {
                    if (IdleRate < 1) 
                    {
                        IdleRate = elapsedTime_ChangeTime / idleToMoveChangeTime;
                    }
                    
                }

                if (elapsedTime_ChangeTime >= idleToMoveChangeTime)
                {
                    elapsedTime_ChangeTime = 0;
                    isStartingChange = false;
                }
            }
        }

        public override void PlayDeathAnim()
        {
            base.PlayDeathAnim();

            animator.Play(deathAnimName, (int)ELayer.Land);
        }

        public void PlayWakeUpAnim()
        {
            if (isWakingUp) 
            {
                return;
            }

            isWakingUp = true;
            IdleRate = 1;
        }

        /// <summary>
        /// <para>shouldChange == true, start change idle to move</para>
        /// shouldChange == false, start change move to idle
        /// </summary>
        public void ChangeIdleToMove(bool shouldChange)
        {
            if (isWakingUp || isStartingChange) 
            {
                return;
            }

            isStartingChange = true;
            shouldChangeToMove = shouldChange;
        }

        public void UseFlyingLayer(bool shouldUse)
        {
            isUsingFlyingLayer = shouldUse;

            if (shouldUse)
            {
                animator.SetLayerWeight((int)ELayer.Land, 0);
                animator.SetLayerWeight((int)ELayer.Flying, 1);
            }
            else
            {
                animator.SetLayerWeight((int)ELayer.Land, 1);
                animator.SetLayerWeight((int)ELayer.Flying, 0);
            }
        }

        public void TakeOff()
        {
            UseFlyingLayer(true);
            animator.Play(takeOffAnimName, (int)ELayer.Flying);
        }

        public void Land()
        {
            UseFlyingLayer(false);
            animator.Play(landingAnimName, (int)ELayer.Land);
        }

        public void PlayAttackMouth()
        {
            if (isAttacking)
            {
                return;
            }

            animator.Play(attackMouthAnimName, (int)ELayer.Land);
        }

        public void PlayAttackHand()
        {
            if (isAttacking)
            {
                return;
            }

            animator.Play(attackHandAnimName, (int)ELayer.Land);
        }

        public void PlayAttackFlame_Ground()
        {
            if (isAttacking) 
            {
                return;
            }

            animator.Play(attackFlameAnimName_Ground, (int)ELayer.Land);
        }

        public void PlayAttackFlame_Flying()
        {
            if (isAttacking)
            {
                return;
            }

            animator.Play(attackFlameAnimName_Air, (int)ELayer.NeckHead);
        }

        public void CheckAttackAnimBegin(int bIsBeginningAttackAnim)
        {
            isAttacking = (bIsBeginningAttackAnim != 0);
            OnBeginAttack.Invoke(isAttacking);
        }

        public override void StartAttackCheck(int bIsCheckingAttack)
        {
            base.StartAttackCheck(bIsCheckingAttack);
        }

        public void CheckMouthAttack(int bIsAttacking)
        {
            bool isAttacking = (bIsAttacking != 0);

            OnCheckMouthAttackDelegate.Invoke(isAttacking);
        }

        public void CheckHandAttack(int bIsAttacking)
        {
            bool isAttacking = (bIsAttacking != 0);

            OnCheckHandAttackDelegate.Invoke(isAttacking);
        }

        public void CheckFlameAttack(int bIsAttacking)
        {
            bool isAttacking = (bIsAttacking != 0);

            if (isAttacking)
            {
                flameVFX = GameManager.VFXManager.SpawnVFX(flameMouthTransform, EVFXName.DragonUsurperFlame_Ground, true);
            }
            else
            {
                GameManager.VFXManager.DespawnVFX(flameVFX);
                flameVFX = null;
            }

            OnCheckAttackFlameDelegate.Invoke(isAttacking);
        }

        public void OnPlayingMoveSound(int moveState)
        {
            if (isUsingFlyingLayer)
            {
                OnPlayingMoveSoundDelegate?.Invoke(CurrentMoveSpeed, true);
                return;
            }

            // to seperate walk state footstep and runt state footstep
            if (moveState == (int)EMoveState.Walk && CurrentMoveSpeed < 0.5f)
            {
                OnPlayingMoveSoundDelegate?.Invoke(CurrentMoveSpeed, false);
            }
            else if (moveState == (int)EMoveState.Run && currentMoveSpeed >= 0.5f)
            {
                OnPlayingMoveSoundDelegate?.Invoke(CurrentMoveSpeed, false);
            }
        }

        public void OnPlayingLandSound(int bIsLandingCompleted)
        {
            OnPlayingLandSoundDelegate?.Invoke(bIsLandingCompleted != 0);
        }

        public void OnPlayingHandAttackSound()
        {
            OnPlayingHandAttackSoundDelegate?.Invoke();
        }

        public void OnPlayingMouthAttackSound()
        {
            OnPlayingMouthAttackSoundDelegate?.Invoke();
        }

        public void OnPlayingFlameAttackSound()
        {
            OnPlayingFlameAttackSoundDelegate?.Invoke();
        }

        public void OnPlayingSleepSound()
        {
            if (SleepRate < 1) 
            {
                return;
            }

            OnPlayingSleepSoundDelegate?.Invoke();
        }

        public void OnPlayingDeathSound()
        {
            GameManager.AudioManager.StopAllAudio3Ds();
            OnPlayingDeathSoundDelegate?.Invoke();
        }
    }
}
