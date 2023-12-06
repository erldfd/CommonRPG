using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CommonRPG
{
    public class SpringArm : MonoBehaviour
    {
        [SerializeField]
        private Transform parentTransform = null;

        [SerializeField]
        private Vector3 relativePositionWithParent = Vector3.zero;

        [SerializeField]
        private Transform childTransform = null;

        [SerializeField]
        private bool ShouldUseSmoothMove = false;

        [SerializeField]
        private float SmoothMoveSpeed = 0;

        [SerializeField]
        private float springArmLength = 10;

        [SerializeField]
        private bool ShouldUseSpringArmHitTest = false;

        [SerializeField]
        private LayerMask layerMask;

        private void Awake()
        {
            childTransform.LookAt(transform);
        }

        private void Update()
        {
            if (ShouldUseSmoothMove)
            {
                transform.position = Vector3.Lerp(transform.position, parentTransform.position + relativePositionWithParent, SmoothMoveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = parentTransform.position + relativePositionWithParent;
            }

            Vector3 direction = childTransform.position - transform.position;
            direction.Normalize();

            if (ShouldUseSpringArmHitTest)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, springArmLength, layerMask))
                {
                    childTransform.position = hit.point;
                }
                else
                {
                    childTransform.position = transform.position + direction * springArmLength;
                }
            }
            else
            {
                childTransform.position = transform.position + direction * springArmLength;
            }
        }

        public void RotateWithVerticalAxis(float angle)
        {
            transform.Rotate(transform.up, angle);
            //childTransform.LookAt(transform.position);
        }

        public void RotateWithHorizontalAxis(float angle)
        {
            float angleSign = Mathf.Sign(angle);
            float includedAngle = Vector3.Angle(childTransform.forward, -angleSign * parentTransform.up);
            float toleranceMargin = 20;
            float remainingAngle = includedAngle - toleranceMargin;

            if (remainingAngle < Mathf.Abs(angle))
            {
                angle = angleSign * remainingAngle;
            }

            childTransform.RotateAround(transform.position, transform.right, angle);
        }
    }

}
