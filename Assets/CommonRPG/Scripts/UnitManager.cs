using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField]
        private ACharacter player = null;
        public ACharacter Player { get { return player; } }

        [SerializeField]
        private HashSet<Slime> activatedSlimeSet = new HashSet<Slime>();
        [SerializeField]
        private Queue<Slime> deactivatedSlimeQueue = new Queue<Slime>();

        [SerializeField]
        private HashSet<Turtle> activatedTurtleSet = new HashSet<Turtle>();
        [SerializeField]
        private Queue<Turtle> deactivatedTurtleQueue = new Queue<Turtle>();

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
                if (deactivatedSlimeQueue.Count > 0)
                {
                    monster = deactivatedSlimeQueue.Dequeue();
                }
                else
                {
                    monster = Instantiate(monster, position, rotation);
                }

                activatedSlimeSet.Add((Slime)monster);
            }
            else if (monster is Turtle)
            {
                if (deactivatedTurtleQueue.Count > 0)
                {
                    monster = deactivatedTurtleQueue.Dequeue();
                }
                else
                {
                    monster = Instantiate(monster, position, rotation);
                }

                activatedTurtleSet.Add((Turtle)monster);
            }

            monster.gameObject.SetActive(true);
            monster.transform.SetPositionAndRotation(position, rotation);
            monster.IsDead = false;
            monster.ActivateAI(true);

            StatComponent monsterStat = monster.StatComponent;

            monsterStat.BaseAttackPower = data.Data.Damage;
            monsterStat.BaseDefense = data.Data.Defense;
            monsterStat.BaseHealthPoint = data.Data.Hp;
            monsterStat.CurrentHealthPoint = data.Data.Hp;
            monsterStat.BaseManaPoint = data.Data.Mp;

            return monster;
        }

        public void DeactiveMonster(MonsterBase monster)
        {
            monster.IsDead = true;

            if (monster is Slime)
            {
                deactivatedSlimeQueue.Enqueue((Slime)monster);
                activatedSlimeSet.Remove((Slime)monster);
            }
            else if (monster is Turtle)
            {
                deactivatedTurtleQueue.Enqueue((Turtle)monster);
                activatedTurtleSet.Remove((Turtle)monster);
            }

            monster.ActivateAI(false);
            monster.gameObject.SetActive(false);
        }
    }

}
