using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    Transform parentTransform = null;

    [SerializeField]
    float rotateSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(parentTransform);
    }

    // Update is called once per frame
    void Update()
    {
       // transform.Rotate(parentTransform.up, Time.deltaTime * rotateSpeed);
        transform.RotateAround(parentTransform.position, parentTransform.up, Time.deltaTime * rotateSpeed);
    }
}
