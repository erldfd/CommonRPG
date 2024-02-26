using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonRPG
{
    [DefaultExecutionOrder(-1)]
    public class UnitManager : MonoBehaviour
    {
        [SerializeField]
        private Player playerPrefab = null;

        [SerializeField]
        private ACharacter playerCharacter = null;
        public ACharacter PlayerCharacter {
            get { return playerCharacter; } 
            set 
            {
                Debug.Assert(value);
                playerCharacter = value; 
            } 
        }

        [SerializeField]
        private SpringArm springArm = null;

        [SerializeField]
        private PlayerStartPoint playerStartPoint = null;

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
            //player = GameObject.FindGameObjectWithTag("Player").GetComponent<ACharacter>();
            //Debug.Assert(player);
            Debug.Log("UnitManaherAwake");
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            monster.MonsterName = data.Data.MonsterName;

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

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            Debug.Log($"{scene.name} is loaded, {loadSceneMode}");

            GameObject startPoint = GameObject.Find("PlayerStartPoint");

            if (startPoint == null) 
            {
                return;
            }

            playerStartPoint = startPoint.GetComponent<PlayerStartPoint>();

            if (PlayerCharacter == null) 
            {
                Player player = Instantiate(playerPrefab);
                PlayerCharacter = player.PlayerCharacter;
                springArm = player.SpringArm;

                EquipmentScreen equipmentScreen = (EquipmentScreen)GameManager.InventoryManager.InventoryList[(int)EInventoryType.EquipmentScreen];
                equipmentScreen.WeaponEquipmentTransform = PlayerCharacter.WeaponEquipmentTransform;
                equipmentScreen.ShieldEquipmentTransform = PlayerCharacter.ShieldEquipmentTransform;
            }

            PlayerCharacter.transform.position = playerStartPoint.transform.position;
            PlayerCharacter.transform.rotation = playerStartPoint.transform.rotation;
        }
    }

}
