using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class MonsterSpawner : MonoBehaviour
    {
        protected enum EConditionCheck
        {
            Deactivated,
            Over,
            Equal,
            Under
        }

        [SerializeField]
        protected string spawnerName = null;

        [SerializeField]
        protected EMonsterName spawningMonster = EMonsterName.None;

        [SerializeField]
        protected int spawnCount = 1;

        [SerializeField]
        protected float startTime = 5;

        [SerializeField]
        protected float interval = 3;

        [Header("Conditions")]
        [SerializeField]
        protected EConditionCheck monsterNumberCheck = EConditionCheck.Deactivated;

        [SerializeField]
        protected int monsterCountInArea = 0;

        [SerializeField]
        protected float radius;

        protected void Start()
        {
            switch (monsterNumberCheck)
            {
                case EConditionCheck.Deactivated:
                {
                    GameManager.SetTimer(startTime, interval, spawnCount - 1, () =>
                    {
                        SpawnMonster();
                    }, true);

                    break;
                }
                case EConditionCheck.Over:
                {
                    LayerMask layerMask = LayerMask.GetMask("Monster");

                    GameManager.SetTimer(startTime, interval, -1, () =>
                    {
                        Collider[] overlaps = Physics.OverlapSphere(transform.position, radius, layerMask);

                        if (overlaps.Length > monsterCountInArea)
                        {
                            SpawnMonster();
                        }

                    }, true);

                    break;
                }
                case EConditionCheck.Equal:
                {
                    LayerMask layerMask = LayerMask.GetMask("Monster");

                    GameManager.SetTimer(startTime, interval, -1, () =>
                    {
                        Collider[] overlaps = Physics.OverlapSphere(transform.position, radius, layerMask);

                        if (overlaps.Length == monsterCountInArea)
                        {
                            SpawnMonster();
                        }

                    }, true);

                    break;
                }
                case EConditionCheck.Under:
                {
                    LayerMask layerMask = LayerMask.GetMask("Monster");

                    GameManager.SetTimer(startTime, interval, -1, () =>
                    {
                        Collider[] overlaps = Physics.OverlapSphere(transform.position, radius, layerMask);

                        if (overlaps.Length < monsterCountInArea)
                        {
                            SpawnMonster();
                        }

                    }, true);

                    break;
                }
            }
            
        }


        protected void SpawnMonster()
        {
            GameManager.SpawnMonster(spawningMonster, transform.position, transform.rotation);
        }

        protected void OnGUI()
        {
//            //Debug.Log("OnGUI Enabled");
//#if UNITY_EDITOR
//            //GUI.editor
//            if (UnityEditor.EditorApplication.isPlaying)
//            {
//                return;
//            }

//            Vector3 pos = Camera.current.WorldToScreenPoint(transform.position);
//            GUI.Label(new Rect(pos.x, Screen.height - pos.y, 100, 20), spawnerName);
//#endif
        }
    }
}

