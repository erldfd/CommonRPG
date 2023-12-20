using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CommonRPG
{
    public class MiscItem : AItem
    {
        private BoxCollider boxCollider = null;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            Debug.Assert(boxCollider);
            boxCollider.enabled = true;
        }

        public override void EnableCollider(bool ShouldEnable)
        {
            if (boxCollider.IsDestroyed())
            {
                return;
            }

            boxCollider.enabled = ShouldEnable;
        }
    }
}

