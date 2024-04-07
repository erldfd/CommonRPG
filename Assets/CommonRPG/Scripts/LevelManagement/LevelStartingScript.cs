using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class LevelStartingScript : MonoBehaviour
    {
        [SerializeField]
        private List<AudioClip> levelBGMList;

        protected void Awake()
        {
            
        }

        protected void Start()
        {
            if (levelBGMList != null && levelBGMList.Count > 0)
            {
                GameManager.AudioManager.PlayLongAudio2D(levelBGMList[0], 1, true);
            }
        }

        public virtual void StartScript()
        {

        }
    }
}
