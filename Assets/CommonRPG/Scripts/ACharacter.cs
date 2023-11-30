using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ACharacter : MonoBehaviour
{
    [SerializeField]
    Camera characterCamera = null;

    [SerializeField]
    float cameraMoveSensitivity = 1;

    [SerializeField]
    protected MovementComponenet movementComponent = null;

    protected Vector2 movementInput = Vector2.zero;
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

    [SerializeField]
    protected SpringArm springArm = null;

    private void Awake()
    {
        Debug.Assert(MovementComp);
        Debug.Assert(characterCamera);
    }

    protected void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
        SetMovementDirection(movementInput);
        Debug.Log("OnMove");
    }

    protected void OnPauseAndResume(InputValue value)
    {
        Debug.Log("PauseAndResume");
    }

    protected void OnMoveMouseVertical(InputValue value)
    {
        float inValue = value.Get<float>();

        if (springArm)
        {
            springArm.RotateWithVerticalAxis(inValue * cameraMoveSensitivity);
        }

        SetMovementDirection(movementInput);
    }

    

    protected void OnMoveMouseHorizontal(InputValue value)
    {
        float inValue = value.Get<float>();

        if (springArm)
        {
            springArm.RotateWithHorizontalAxis(inValue * cameraMoveSensitivity);
        }
    }

    protected void SetMovementDirection(Vector2 newMovementInput)
    {
        Transform cameraTransform = characterCamera.transform;

        Vector3 moveDirection = cameraTransform.right * movementInput.x + cameraTransform.forward * movementInput.y;
        moveDirection.y = 0;
        moveDirection.Normalize();

        movementComponent.MoveDirection = moveDirection;
    }
}
