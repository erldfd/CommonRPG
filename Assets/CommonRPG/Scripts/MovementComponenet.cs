using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementComponenet : MonoBehaviour
{
    public Vector3 MoveDirection { get; set; }

    [SerializeField]
    private float rotationSpeed = 1;

    [SerializeField]
    private float moveSpeed = 1;

    [SerializeField]
    private Rigidbody rigid = null;

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (MoveDirection == Vector3.zero) 
        {
            return;
        }

        Vector3 forward = transform.forward;
        float fixedDeltaTime = Time.fixedDeltaTime;

        transform.forward = Vector3.Lerp(forward, MoveDirection, rotationSpeed * fixedDeltaTime);
        rigid.MovePosition(/*gameObject.*/transform.position + MoveDirection * moveSpeed * fixedDeltaTime);
    }

}
