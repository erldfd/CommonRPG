using UnityEngine;

namespace CommonRPG
{
    public class Slime : NormalMonster
    {
        protected override void Awake()
        {
            base.Awake();
            SlimeAnimController slimeAnimController = (SlimeAnimController)animController;
            Debug.Assert(slimeAnimController);
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
