using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class DragonUsurperAnimController : AAnimController
    {
        private enum ELayer
        {
            Land,
            Flying
        }

        public event Action OnWakeUpEndedDelegate = null;

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

        /// <summary>
        ///  sleep rate 1 -> play sleep anim, sleep rate 0 -> play idle or move anim
        /// </summary>
        protected float sleepRate = 1;
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

        protected override void Awake()
        {
            base.Awake();

            //GameManager.TimerManager.SetTimer(2, 16, -1, () =>
            //{
            //    TakeOff();

            //}, true);

            //GameManager.TimerManager.SetTimer(10, 16, -1, () =>
            //{
            //    Land();

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
                elapsedTime_ChangeTime += Time.deltaTime;

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
            animator.Play(attackMouthAnimName, (int)ELayer.Land);
        }

        public void PlayAttackHand()
        {
            animator.Play(attackHandAnimName, (int)ELayer.Land);
        }

        public void PlayAttackFlame_Ground()
        {
            animator.Play(attackFlameAnimName_Ground, (int)ELayer.Land);
        }

        public void PlayAttackFlame_Flying()
        {

        }

        public void CheckAttackAnimBegin(int bIsBeginningAttackAnim)
        {
            OnBeginAttack.Invoke((bIsBeginningAttackAnim != 0));
        }
    }
}
