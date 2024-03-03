using System;
using Unity.VisualScripting;
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

                GameManager.InGameUI.SetPlayerHealthBarFillRatio(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
                GameManager.InGameUI.SetPlayerManaBarFillRatio(statComponent.CurrentManaPoint / statComponent.TotalMana);
            }
        }

        [SerializeField]
        private Transform weaponEquipmentTransform = null;
        public Transform WeaponEquipmentTransform { get { return weaponEquipmentTransform; } set { weaponEquipmentTransform = value; } }

        [SerializeField]
        private Transform shieldEquipmentTransform = null;
        public Transform ShieldEquipmentTransform { get { return shieldEquipmentTransform; } set { shieldEquipmentTransform = value; } }

        [Header("Camera")]
        [SerializeField]
        protected Camera characterCamera = null;
        public Camera CharacterCamera { get { return characterCamera; } set { characterCamera = value; } }

        [SerializeField]
        protected float cameraMoveSensitivity = 1;

        [SerializeField]
        protected SpringArm springArm = null;

        [Header("Input")]
        [SerializeField]
        protected InputActionAsset inputActionAsset = null;

        [Header("Interaction")]
        [SerializeField]
        protected InteractionDetector interactionDetector = null;

        public abstract float TakeDamage(float DamageAmount, AUnit DamageCauser = null);

        protected bool isNormalAttackPressed = false;

        protected bool IsCameraMoveAllowing 
        {
            get 
            {
                return (GameManager.IsInventoryOpened() == false && GameManager.InGameUI.IsConversationStarted == false && GameManager.QuestManager.IsQuestWindowOpened() == false); 
            } 
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(base.statComponent);
            Debug.Assert(base.animController);

            Debug.Assert(MovementComp);
            Debug.Assert(characterCamera);

            Debug.Assert(inputActionAsset);

            Debug.Assert(interactionDetector);
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

            inputActionAsset.FindActionMap("PlayerInput").FindAction("Interaction").performed += OnInteraction;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("UsingQuickSlot").performed += OnUseQuickSlot;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("OpenQuestWindow").performed += OnOpenQuestWindow;

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

            inputActionAsset.FindActionMap("PlayerInput").FindAction("Interaction").performed -= OnInteraction;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("UsingQuickSlot").performed -= OnUseQuickSlot;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("OpenQuestWindow").performed -= OnOpenQuestWindow;
        }

        public virtual void ObtainExp(float amount)
        {
            StatComponent.CurrentExp += amount;
            GameManager.InGameUI.FloatExpNumber(amount, transform.position);
            float currentExpRatio = StatComponent.CurrentExp / StatComponent.MaxExpOfCurrentLevel;
            GameManager.InGameUI.SetPlayerExpBarFillRatio(currentExpRatio);

            //Debug.Log($"exp obtained : {amount}");
        }

        protected virtual void OnMove(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
            SetMovementDirection(movementInput);
        }

        protected virtual void OnPauseAndResume(InputAction.CallbackContext context)
        {
            //Debug.Log("PauseAndResume");
        }

        protected virtual void OnMoveMouseVertical(InputAction.CallbackContext context)
        {
            if (IsCameraMoveAllowing == false)
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
            if (IsCameraMoveAllowing == false)
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

        protected virtual void OnInteraction(InputAction.CallbackContext context)
        {
            interactionDetector.Interact();
            Debug.Log($"{context.control.name}, {context.control.displayName}");
            
        }

        protected virtual void OnUseQuickSlot(InputAction.CallbackContext context)
        {
            EInputKey inputKey = context.control.displayName.ToInputKey();
            GameManager.InventoryManager.UseQuickSlot(inputKey);
        }
        
        protected virtual void OnOpenQuestWindow(InputAction.CallbackContext context)
        {
            GameManager.QuestManager.OpenAndCloseQuestWindow();
        }
    }

    public enum EInputKey
    {
        Key1,
        Key2,
        Key3,
    }

    public static class DisplayNameToKey
    {
        public static EInputKey ToInputKey(this string inputKey)
        {
            switch (inputKey)
            {
                case "1":
                {
                    return EInputKey.Key1;
                }
                case "2":
                {
                    return EInputKey.Key2;
                }
                case "3":
                {
                    return EInputKey.Key3;
                }
                default:
                {
                    Debug.LogAssertion("Not Declared input");
                    throw new ArgumentOutOfRangeException(nameof(inputKey), inputKey, null);
                }
            }
        }
    }
}


