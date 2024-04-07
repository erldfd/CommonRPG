using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class DamageCollider : MonoBehaviour
    {
        [Flags]
        private enum ECondition
        {
            Nothing = 0,
            OnEnter = 1<<0,
            OnExit = 1<<1,
            OnStay = 1<<2
        }

        public event Action<IDamageable> OnEnterDelgate = null;
        public event Action<IDamageable> OnExitDelegate = null;
        public event Action<IDamageable> OnStayDelegate = null;

        [SerializeField]
        private Collider targetCollider;

        [SerializeField]
        private ECondition condition;

        [SerializeField]
        private float damageInterval = 0.5f;

        private bool canDamage = true;

        private HashSet<Collider> enterUnitSet = new HashSet<Collider>();
        private HashSet<Collider> exitUnitSet = new HashSet<Collider>();

        private void Awake()
        {
            Debug.Assert(targetCollider);
            targetCollider.gameObject.SetActive(false);
            targetCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (condition.HasFlag(ECondition.OnEnter) == false) 
            {
                return;
            }

            if (enterUnitSet.Contains(other)) 
            {
                return;
            }

            enterUnitSet.Add(other);

            IDamageable damageableUnit = other.GetComponent<AUnit>() as IDamageable;

            OnEnterDelgate.Invoke(damageableUnit);
        }

        private void OnTriggerExit(Collider other)
        {
            if (condition.HasFlag(ECondition.OnExit) == false)
            {
                return;
            }

            if (exitUnitSet.Contains(other))
            {
                return;
            }

            exitUnitSet.Add(other);

            Debug.LogWarning($"Exit : {other.name}");

            IDamageable damageableUnit = other.GetComponent<AUnit>() as IDamageable;

            OnExitDelegate.Invoke(damageableUnit);
        }

        private void OnTriggerStay(Collider other)
        {
            if (condition.HasFlag(ECondition.OnStay) == false)
            {
                return;
            }

            if (canDamage == false) 
            {
                return;
            }

            canDamage = false;

            GameManager.TimerManager.SetTimer(damageInterval, 0, 0, () =>
            {
                if (this == null)
                {
                    return;
                }

                canDamage = true;

            }, true);

            IDamageable damageableUnit = other.GetComponent<AUnit>() as IDamageable;

            OnStayDelegate.Invoke(damageableUnit);
        }

        public void SetActiveDamageCollider(bool isActivated)
        {
            if (targetCollider == null)
            {
                return;
            }

            targetCollider.gameObject.SetActive(isActivated);

            if (isActivated == false) 
            {
                enterUnitSet.Clear();
                exitUnitSet.Clear();
            }
        }

        public void ActivateDamageColliderForTime(float time)
        {
            targetCollider.gameObject.SetActive(true);

            GameManager.TimerManager.SetTimer(time, 0, 0, () =>
            {
                SetActiveDamageCollider(false);

            }, true);
        }
    }
}
