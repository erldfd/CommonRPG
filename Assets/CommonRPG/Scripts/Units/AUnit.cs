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
        public bool IsDead { get { return isDead; } set { isDead = value; } }

        [SerializeField]
        protected StatComponent statComponent = null;
        public StatComponent StatComponent { get { return statComponent; } }

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
        public AAnimController AnimController { get { return animController; } }

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
        }

        protected virtual void OnEnable()
        {

        }
        protected virtual void OnDisable()
        {

        }

        protected virtual void OnDestroy()
        {
            
        }
    }
}

