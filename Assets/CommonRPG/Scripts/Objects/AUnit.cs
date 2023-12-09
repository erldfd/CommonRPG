using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public abstract class AUnit : MonoBehaviour
    {
        [SerializeField]
        protected string unitName = "";

        [SerializeField]
        protected bool isDead = false;

        [SerializeField]
        protected StatComponent statComponent = null;

        [SerializeField]
        protected MovementComponent movementComponent = null;
        public MovementComponent MovementComp
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
        protected AAnimController animController = null;

        protected virtual void Awake()
        {
            Debug.Assert(statComponent);
            Debug.Assert(animController);
        }

        protected virtual void Update()
        {
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();
    }
}

