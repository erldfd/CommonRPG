using CommonRPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private ACharacter player = null;
    public ACharacter Player { get { return player; } }

    [SerializeField]
    private HashSet<MonsterBase> monsterSet = new HashSet<MonsterBase>();

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ACharacter>();
        Debug.Assert(player);
    }

}
