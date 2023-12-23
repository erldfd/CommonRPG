using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class MonsterSpawner : MonoBehaviour
    {
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

        protected void Start()
        {
            GameManager.SetTimer(startTime, interval, spawnCount - 1, () =>
            {
                GameManager.SpawnMonster(spawningMonster, transform.position, transform.rotation);
            }, true);
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

