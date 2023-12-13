using UnityEngine;
using UnityEngine.InputSystem;

namespace CommonRPG
{
    public abstract class ACharacter : AUnit, IDamageable
    {
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
        }

        protected override void OnEnable()
        {
            Debug.Log("OnEnable");
            inputActionAsset.Enable();
            inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").performed += OnMove;
            //inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").started += OnMove;
            inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").canceled += OnMove;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("PauseAndResume").performed += OnPauseAndResume;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("MoveMouseVertical").performed += OnMoveMouseVertical;
            inputActionAsset.FindActionMap("PlayerInput").FindAction("MoveMouseHorizontal").performed += OnMoveMouseHorizontal;

            inputActionAsset.FindActionMap("PlayerInput").FindAction("NormalAttack").performed += OnNormalAttack;
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
            float inValue = context.ReadValue<float>();

            if (springArm)
            {
                springArm.RotateWithVerticalAxis(inValue * cameraMoveSensitivity);
            }

            SetMovementDirection(movementInput);
        }

        protected virtual void OnMoveMouseHorizontal(InputAction.CallbackContext context)
        {
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
            LayerMask layerMask = LayerMask.GetMask("Monster");

            bool isRayHit = Physics.Raycast(transform.position + transform.forward * 2, transform.forward, out RaycastHit hit, 5, layerMask);
            if (isRayHit)
            {
                IDamageable damageableUnit = hit.transform.GetComponent<IDamageable>();
                if (damageableUnit != null)
                {
                    damageableUnit.TakeDamage(1);
                }
            }
        }
    }

}
