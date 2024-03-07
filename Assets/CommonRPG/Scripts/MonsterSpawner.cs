using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        protected float spawnRadius = 0;

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

        private float elapsedTime = 0;
        private int currentspawnedNumber = 0;
        private bool isStarted = false;

        protected void Update()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= startTime && isStarted == false) 
            {
                isStarted = true;
            }

            if (isStarted == false) 
            {
                return;
            }

            switch (monsterNumberCheck)
            {
                case EConditionCheck.Deactivated:
                {

                    if (elapsedTime >= interval && spawnCount > currentspawnedNumber) 
                    {
                        SpawnMonster();
                        elapsedTime = 0;
                        currentspawnedNumber++;
                    }

                    break;
                }
                case EConditionCheck.Over:
                {
                    LayerMask layerMask = LayerMask.GetMask("Monster");
                    Collider[] overlaps = Physics.OverlapSphere(transform.position, radius, layerMask);

                    if (overlaps.Length > monsterCountInArea && interval <= elapsedTime)
                    {
                        SpawnMonster();
                        elapsedTime = 0;
                    }

                    break;
                }
                case EConditionCheck.Equal:
                {
                    LayerMask layerMask = LayerMask.GetMask("Monster");
                    Collider[] overlaps = Physics.OverlapSphere(transform.position, radius, layerMask);

                    if (overlaps.Length == monsterCountInArea && interval <= elapsedTime)
                    {
                        SpawnMonster();
                        elapsedTime = 0;
                    }

                    break;
                }
                case EConditionCheck.Under:
                {
                    LayerMask layerMask = LayerMask.GetMask("Monster");
                    Collider[] overlaps = Physics.OverlapSphere(transform.position, radius, layerMask);

                    if (overlaps.Length < monsterCountInArea && interval <= elapsedTime)
                    {
                        SpawnMonster();
                        elapsedTime = 0;
                    }

                    break;
                }
            }
        }

        protected void SpawnMonster()
        {
            Vector3 spawnPosition = transform.position;

            float randomPosX = Random.Range(-spawnRadius, spawnRadius);
            float randomPosZ = Mathf.Sqrt((spawnRadius + randomPosX) * (spawnRadius - randomPosX));

            spawnPosition.x = spawnPosition.x + randomPosX;
            spawnPosition.z = spawnPosition.z + Random.Range(-randomPosZ, randomPosZ);

            GameManager.SpawnMonster(spawningMonster, spawnPosition, transform.rotation);
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

