using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CommonRPG
{
    public abstract class ACharacter : AUnit, IDamageable
    {
        [Header("Equipment")]
        [SerializeField]
        protected WeaponItem characterWeapon = null;
        public WeaponItem CharacterWeapon
        {
            get { return characterWeapon; }
            set
            {
                characterWeapon = value;

                if (value == null)
                {
                    statComponent.WeaponAttackPowerBonus = 0;
                    statComponent.WeaponDefenseBonus = 0;
                    statComponent.WeaponHealthBonus = 0;
                    statComponent.WeaponManaBonus = 0;
                }
                else
                {
                    statComponent.WeaponAttackPowerBonus = value.Data.Damage;
                    statComponent.WeaponDefenseBonus = value.Data.Defense;
                    statComponent.WeaponHealthBonus = value.Data.HPBonus;
                    statComponent.WeaponManaBonus = value.Data.MPBonus;
                }

                if (statComponent.CurrentHealthPoint > statComponent.TotalHealth)
                {
                    statComponent.CurrentHealthPoint = statComponent.TotalHealth;
                }

                if (statComponent.CurrentManaPoint > statComponent.TotalMana)
                {
                    statComponent.CurrentManaPoint = statComponent.TotalMana;
                }

                GameManager.SetPlayerHealthBarFillRatio(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
                GameManager.SetPlayerManaBarFillRatio(statComponent.CurrentManaPoint / statComponent.TotalMana);
            }
        }

        [Header("Camera")]
        [SerializeField]
        protected Camera characterCamera = null;

        [SerializeField]
        protected float cameraMoveSensitivity = 1;

        [SerializeField]
        protected SpringArm springArm = null;

        [SerializeField]
        protected InputActionAsset inputActionAsset = null;

        public abstract float TakeDamage(float DamageAmount, AUnit DamageCauser = null);

        protected bool isNormalAttackPressed = false;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(MovementComp);
            Debug.Assert(characterCamera);
            Debug.Assert(inputActionAsset);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            animController.CurrentMoveSpeed = movementComponent.CurrentMoveSpeed;

            if (isNormalAttackPressed)
            {
                OnNormalAttackInternal();
            }
        }

        protected override void OnEnable()
        {
            inputActionAsset.Enable();

            inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").performed += OnMove;
            inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").canceled += OnMove;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("PauseAndResume").performed += OnPauseAndResume;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("MoveMouseVertical").performed += OnMoveMouseVertical;
            inputActionAsset.FindActionMap("PlayerInput").FindAction("MoveMouseHorizontal").performed += OnMoveMouseHorizontal;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("NormalAttack").performed += OnNormalAttack;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("OpenInventory").performed += OnOpenInventory;
        }

        protected override void OnDisable()
        {
            inputActionAsset.Disable();
            inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").performed -= OnMove;
            inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").canceled -= OnMove;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("PauseAndResume").performed -= OnPauseAndResume;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("MoveMouseVertical").performed -= OnMoveMouseVertical;
            inputActionAsset.FindActionMap("PlayerInput").FindAction("MoveMouseHorizontal").performed -= OnMoveMouseHorizontal;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("NormalAttack").performed -= OnNormalAttack;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("OpenInventory").performed -= OnOpenInventory;
        }

        public virtual void ObtainExp(float amount)
        {
            statComponent.CurrentExp += amount;
            //Debug.Log($"exp obtained : {amount}");
        }

        protected virtual void OnMove(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
            SetMovementDirection(movementInput);
        }

        protected virtual void OnPauseAndResume(InputAction.CallbackContext context)
        {
            Debug.Log("PauseAndResume");
        }

        protected virtual void OnMoveMouseVertical(InputAction.CallbackContext context)
        {
            if (GameManager.IsInventoryOpened())
            {
                return;
            }

            float inValue = context.ReadValue<float>();

            if (springArm)
            {
                springArm.RotateWithVerticalAxis(inValue * cameraMoveSensitivity);
            }

            SetMovementDirection(movementInput);
        }

        protected virtual void OnMoveMouseHorizontal(InputAction.CallbackContext context)
        {
            if (GameManager.IsInventoryOpened())
            {
                return;
            }

            float inValue = context.ReadValue<float>();

            if (springArm)
            {
                springArm.RotateWithHorizontalAxis(inValue * cameraMoveSensitivity);
            }
        }

        protected void SetMovementDirection(Vector2 newMovementInput)
        {
            Transform cameraTransform = characterCamera.transform;

            Vector3 moveDirection = cameraTransform.right * newMovementInput.x + cameraTransform.forward * newMovementInput.y;
            moveDirection.y = 0;
            moveDirection.Normalize();

            movementComponent.MoveDirection = moveDirection;
        }

        protected virtual void OnNormalAttack(InputAction.CallbackContext context)
        {
            isNormalAttackPressed = Convert.ToBoolean(context.ReadValue<float>());
        }

        protected virtual void OnNormalAttackInternal()
        {

        }

        protected virtual void OnOpenInventory(InputAction.CallbackContext context)
        {
            GameManager.OpenAndCloseInventory();
        }
    }

}
