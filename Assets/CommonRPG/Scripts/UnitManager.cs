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
    private HashSet<Slime> activatedSlimeSet = new HashSet<Slime>();
    [SerializeField]
    private Queue<Slime> deactivatedSlimeSet = new Queue<Slime>();
 
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ACharacter>();
        Debug.Assert(player);
    }

    public MonsterBase SpawnMonster(MonsterData data, Vector3 position, Quaternion rotation)
    {
        MonsterBase monster = data.MonsterPrefab;

        if (monster is Slime) 
        {
            if (deactivatedSlimeSet.Count > 0)
            {
                monster = deactivatedSlimeSet.Dequeue();
                
            }
            else
            {
                monster = Instantiate(monster, position, rotation);
            }

            activatedSlimeSet.Add((Slime)monster);
        }

        monster.gameObject.SetActive(true);
        monster.transform.SetPositionAndRotation(position, rotation);
        
        //monster.StatComponent.
        // TODO : Set Monster Stat according to MonsterData
        return monster;
    }
}
