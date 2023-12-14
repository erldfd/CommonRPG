using UnityEngine;

namespace CommonRPG
{
    public class MovementComponent : MonoBehaviour
    {
        public Vector3 MoveDirection { get; set; }
        public float CurrentMoveSpeed { get; private set; }

        [SerializeField]
        private float rotationSpeed = 1;

        [SerializeField]
        private float moveSpeed = 1;
        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }

        [SerializeField]
        private Rigidbody rigid = null;

        protected bool canMove = false;
        public bool CanMove { get { return canMove; } set { canMove = value; } }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            CurrentMoveSpeed = MoveDirection.magnitude * moveSpeed;

            if (CanMove == false) 
            {
                return;
            }

            if (MoveDirection == Vector3.zero)
            {
                return;
            }

            Vector3 forward = transform.forward;
            float fixedDeltaTime = Time.fixedDeltaTime;

            if (Mathf.Sign(forward.x) != Mathf.Sign(MoveDirection.x) || Mathf.Sign(forward.z) != Mathf.Sign(MoveDirection.z))
            {
                transform.Rotate(0, rotationSpeed, 0);
            }

            transform.forward = Vector3.Lerp(forward, MoveDirection, rotationSpeed * fixedDeltaTime);

            rigid.MovePosition(transform.position + MoveDirection * moveSpeed * fixedDeltaTime);
        }

    }

}
