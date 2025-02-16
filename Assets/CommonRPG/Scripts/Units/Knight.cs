using UnityEngine;
using UnityEngine.InputSystem;

namespace CommonRPG
{
    public class Knight : ACharacter
    {
        private enum EAudioClipList
        {
            DragonBitesTarget,
            Hit1, Hit2, Hit3, Hit4, Hit5
        }

        private bool canCombo = false;

        protected override void Awake()
        {
            base.Awake();

            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            GameManager.InGameUI.SetPlayerHealthBarFillRatio(currentHpRatio);
            GameManager.InGameUI.SetPlayerLevelText(statComponent.Level);
            GameManager.InGameUI.SetPlayerManaBarFillRatio(statComponent.CurrentManaPoint / statComponent.TotalMana);
            GameManager.InGameUI.SetPlayerNameText(base.unitName);

            float currentExpRatio = statComponent.CurrentExp / statComponent.MaxExpOfCurrentLevel;
            GameManager.InGameUI.SetPlayerExpBarFillRatio(currentExpRatio);

            //if (GameManager.UnitManager.Player == null)
            //{
            //    GameManager.UnitManager.Player = this;
            //}
        }

        protected override void Start()
        {
            base.Start();
            //GameManager.QuestManager.UnlockQuest("First Hunt Quest");
            //GameManager.QuestManager.TryReceiveQuest("First Hunt Quest");
           // GameManager.QuestManager.UnlockQuest("Second Hunt Quest");
        }

        protected override void Update()
        {
            base.Update();
            MovementComp.CanMove = IsMovable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            knightAnimController.OnAttackCheckDelegate += OnAttackCheck;
            knightAnimController.OnComboCheckDelegate += OnCheckCombo;
            knightAnimController.OnStartPlayingComboAttackDelegate += OnStartComboAttack;
        }

        protected override void OnDisable()
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            knightAnimController.OnAttackCheckDelegate -= OnAttackCheck;
            knightAnimController.OnComboCheckDelegate -= OnCheckCombo;
            knightAnimController.OnStartPlayingComboAttackDelegate -= OnStartComboAttack;
        }

        public override float TakeDamage(float DamageAmount, AUnit DamageCauser = null, ADamageEventInfo damageEventInfo = null)
        {
            float actualDamageAmount = DamageAmount - statComponent.TotalDefense;
            if (actualDamageAmount < 1)
            {
                actualDamageAmount = 1;
            }

            statComponent.CurrentHealthPoint -= actualDamageAmount;
            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            GameManager.InGameUI.SetPlayerHealthBarFillRatio(currentHpRatio);

            //Debug.Log($"Damage is Taked : {DamageAmount}, CurrentHp : {statComponent.CurrentHealthPoint}");

            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            if (currentHpRatio > 0) 
            {
                knightAnimController.PlayHitAnim();
            }
            else
            {
                knightAnimController.PlayDeathAnim();
                isDead = true;
            }

            if (DamageCauser is DragonUsurper && damageEventInfo != null)  
            {
                DragonUserperDamageEventInfo dragonUserperDamageEventInfo = (DragonUserperDamageEventInfo)damageEventInfo;

                switch (dragonUserperDamageEventInfo.damageType)
                {
                    case DragonUserperDamageEventInfo.EAttackType.MouthAttack:
                    {
                        GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[(int)EAudioClipList.DragonBitesTarget], 1, transform.position);
                        break;
                    }
                    case DragonUserperDamageEventInfo.EAttackType.HandAttack:
                    {
                        GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[(int)EAudioClipList.DragonBitesTarget], 1, transform.position);
                        break;
                    }
                    case DragonUserperDamageEventInfo.EAttackType.FlameAttack:
                    {
                        int audioIndex = Random.Range((int)EAudioClipList.Hit1, (int)EAudioClipList.Hit5 + 1);
                        GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioIndex], 1, transform.position);

                        break;
                    }
                }
            }
            else
            {
                int audioIndex = Random.Range((int)EAudioClipList.Hit1, (int)EAudioClipList.Hit5 + 1);
                GameManager.AudioManager.PlayAudio3D(audioContainer.AudioClipList[audioIndex], 1, transform.position);
            }

            return DamageAmount;
        }

        protected override void OnMove(InputAction.CallbackContext context)
        {
            base.OnMove(context);
        }

        protected override void OnPauseAndResume(InputAction.CallbackContext context)
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            //knightAnimController.PlayHitAnim();
            //animController.ani.Play("Drinking", 0);
            //GameManager.PrintAllQuests();
            //GameManager.QuestManager.UnlockQuest("Hunt Quest 3");
        }

        protected override void OnNormalAttack(InputAction.CallbackContext context)
        {
            base.OnNormalAttack(context);
        }

        protected override void OnNormalAttackInternal()
        {
            base.OnNormalAttackInternal();

            if (IsAttackPossible() == false)
            {
                return;
            }

            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            if (knightAnimController.IsBeginningAttackAnim == false && knightAnimController.ShouldPlayNextComboAttackAnim == false)
            {
                knightAnimController.ComboCount = 0;
                knightAnimController.PlayComboAttackAnim(knightAnimController.ComboCount++);
            }
            else if (canCombo)
            {
                knightAnimController.ShouldPlayNextComboAttackAnim = true;
            }
        }

        private void OnStartComboAttack(int playIndex)
        {
            if (movementInput == Vector2.zero) 
            {
                return;
            }

            Transform cameraTransform = characterCamera.transform;
            Vector3 moveDirection = cameraTransform.right * movementInput.x + cameraTransform.forward * movementInput.y;
            moveDirection.y = 0;
            transform.forward = moveDirection;
        }

        private void OnAttackCheck(bool isStarted)
        {
            if (base.characterWeapon) 
            {
                base.characterWeapon.EnableCollider(isStarted);
            }

            if (isStarted) 
            {
                base.characterWeapon.PlaySwingSound();
            }
        }

        /// <summary>
        /// check if combo is possible now
        /// </summary>
        private void OnCheckCombo(bool canCombo)
        {
            this.canCombo = canCombo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true means movable state</returns>
        private bool IsMovable()
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            bool isMovable = (isDead == false && knightAnimController.IsHit == false && knightAnimController.IsBeginningAttackAnim == false);
            return isMovable;
        }

        private bool IsAttackPossible()
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            bool isAttackPossible = (isDead == false && knightAnimController.IsHit == false && characterWeapon != null && Time.timeScale != 0 &&
                                        knightAnimController.IsDrinking == false);
            return isAttackPossible;
        }
    }

}
