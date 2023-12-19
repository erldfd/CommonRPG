using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class AItem : MonoBehaviour
    {

        public bool IsFieldItem { get; set; }

        [SerializeField]
        protected SItemData data;
        public SItemData Data
        {
            get { return data; }
            set { data = value; }
        }

        public virtual void EnableCollider(bool ShouldEnable)
        {
            
        }
    }
}

