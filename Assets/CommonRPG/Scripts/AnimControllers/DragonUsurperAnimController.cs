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

        [SerializeField]
        protected string takeOffAnimName;

        [SerializeField]
        private string landingAnimName;

        protected override void Awake()
        {
            base.Awake();

            GameManager.TimerManager.SetTimer(2, 16, -1, () =>
            {
                TakeOff();

            }, true);

            GameManager.TimerManager.SetTimer(10, 16, -1, () =>
            {
                Land();

            }, true);
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
    }
}
