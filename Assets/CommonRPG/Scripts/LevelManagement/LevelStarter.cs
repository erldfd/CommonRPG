using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class LevelStarter : MonoBehaviour
    {
        private LevelStartingScript levelStartingScript = null;

        private void Awake()
        {
            levelStartingScript = GetComponent<LevelStartingScript>();
            Debug.AssertFormat(levelStartingScript, "Level Starter Must Have ALevelStartingScript Component");
        }

        private void Start()
        {
            if (levelStartingScript == null)
            {
                return;
            }

            levelStartingScript.StartScript();
        }
    }
}
