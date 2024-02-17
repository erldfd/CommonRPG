using UnityEngine;
using UnityEngine.InputSystem;

namespace CommonRPG
{
    public class Knight : ACharacter
    {
        private bool canCombo = false;

        protected override void Awake()
        {
            base.Awake();

            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            GameManager.SetPlayerHealthBarFillRatio(currentHpRatio);
            GameManager.SetPlayerLevelText(statComponent.Level);
            GameManager.SetPlayerManaBarFillRatio(statComponent.CurrentManaPoint / statComponent.TotalMana);
            GameManager.SetPlayerNameText(base.unitName);

            if (GameManager.UnitManager.Player == null)
            {
                GameManager.UnitManager.Player = this;
            }
        }

        protected override void Start()
        {
            base.Start();
            GameManager.QuestManager.UnlockQuest("First Hunt Quest");
            GameManager.QuestManager.TryReceiveQuest("First Hunt Quest");
            GameManager.QuestManager.UnlockQuest("Second Hunt Quest");

            DontDestroyOnLoad(this);
            DontDestroyOnLoad(base.springArm);
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

            knightAnimController.OnAttackCheck += EnableCollider;
            knightAnimController.OnComboCheck += CheckCombo;
            knightAnimController.OnStartPlayingComboAttack += OnStartComboAttack;
        }

        protected override void OnDisable()
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            knightAnimController.OnAttackCheck -= EnableCollider;
            knightAnimController.OnComboCheck -= CheckCombo;
            knightAnimController.OnStartPlayingComboAttack -= OnStartComboAttack;
        }

        public override float TakeDamage(float DamageAmount, AUnit DamageCauser = null)
        {
            float actualDamageAmount = DamageAmount - statComponent.TotalDefense;
            if (actualDamageAmount < 1)
            {
                actualDamageAmount = 1;
            }

            statComponent.CurrentHealthPoint -= actualDamageAmount;
            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            GameManager.SetPlayerHealthBarFillRatio(currentHpRatio);

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
            GameManager.PrintAllQuests();
            GameManager.QuestManager.UnlockQuest("Hunt Quest 3");
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

        private void EnableCollider(bool shouldEnable)
        {
            if (base.characterWeapon == null) 
            {
                return;
            }

            base.characterWeapon.EnableCollider(shouldEnable);
        }

        /// <summary>
        /// check if combo is possible now
        /// </summary>
        private void CheckCombo(bool canCombo)
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
