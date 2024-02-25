using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class LevelStarter : MonoBehaviour
    {
        [SerializeField]
        private ALevelStartingScript levelStartingScript = null;

        void Start()
        {
            if (levelStartingScript == null)
            {
                return;
            }

            levelStartingScript.StartScript();
        }
    }
}
