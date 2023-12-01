using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AUnit : MonoBehaviour
{
    [SerializeField]
    protected bool isDead = false;

    [SerializeField]
    protected StatComponenet statComponenet = null;

    private void Awake()
    {
        Debug.Assert(statComponenet);
    }
}
