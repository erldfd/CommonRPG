using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private ACharacter playerCharacter = null;
        public ACharacter PlayerCharacter {  get { return playerCharacter; } }

        [SerializeField]
        private SpringArm springArm = null;
        public SpringArm SpringArm { get { return springArm; } }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
