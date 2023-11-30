using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpringArm : MonoBehaviour
{
    [SerializeField]
    private Transform parentTransform = null;

    [SerializeField]
    private Transform childTransform = null;

    [SerializeField]
    private bool isUsingSmoothMove = false;

    [SerializeField]
    private float SmoothMoveSpeed = 0;

    [SerializeField]
    private float springArmLength = 10;

    [SerializeField]
    private bool isUsingSpringArmHitTest = false;

    [SerializeField]
    private LayerMask layerMask;

    private void Awake()
    {
        childTransform.LookAt(transform);
    }

    private void Update()
    {
        if (isUsingSmoothMove) 
        {
            transform.position = Vector3.Lerp(transform.position, parentTransform.position, SmoothMoveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = parentTransform.position;
        }

        if (isUsingSpringArmHitTest) 
        {
            Vector3 direction = childTransform.position - transform.position;
            direction.Normalize();

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
