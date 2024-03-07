using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class BossAIController : AAIController
    {
        public enum EAIPhase
        {
            None,
            Sleep,
            Phase1,
            Phase2,
            Phase3,
            Dead
        }

        [SerializeField]
        protected EAIPhase currentPhase = EAIPhase.None;
        public EAIPhase CurrentPhase { get { return currentPhase; }  set { currentPhase = value; } }
    }
}
