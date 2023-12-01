using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ACharacter : AUnit, IDamageable
{
    [SerializeField]
    Camera characterCamera = null;

    [SerializeField]
    float cameraMoveSensitivity = 1;

    [SerializeField]
    protected MovementComponenet movementComponent = null;
    public MovementComponenet MovementComp
    {
        get
        {
            return movementComponent;
        }
        private set
        {
            movementComponent = value;
        }
    }

    protected Vector2 movementInput = Vector2.zero;

    [SerializeField]
    private InputActionAsset inputActionAsset = null;

    [SerializeField]
    protected SpringArm springArm = null;

    public abstract float TakeDamage(float DamageAmount, IDamageable DamageCauser = null);

    private void Awake()
    {
        Debug.Assert(MovementComp);
        Debug.Assert(characterCamera);
        Debug.Assert(inputActionAsset);
    }

    private void OnEnable()
    {
        inputActionAsset.Enable();
        inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").performed += OnMove;
        //inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").started += OnMove;
        inputActionAsset.FindActionMap("PlayerInput").FindAction("Move").canceled += OnMove;

        inputActionAsset.FindActionMap("PlayerInput").FindAction("PauseAndResume").performed += OnPauseAndResume;

        inputActionAsset.FindActionMap("PlayerInput").FindAction("MoveMouseVertical").performed += OnMoveMouseVertical;
        inputActionAsset.FindActionMap("PlayerInput").FindAction("MoveMouseHorizontal").performed += OnMoveMouseHorizontal;

        inputActionAsset.FindActionMap("PlayerInput").FindAction("NormalAttack").performed += OnNormalAttack;
    }

    private void OnDisable()
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
        Debug.Log("OnMove");
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
                damageableUnit.TakeDamage(10);
            }
        }
    }
}
