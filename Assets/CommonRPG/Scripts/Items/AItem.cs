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

        protected AudioContainer audioContainer = null;

        protected virtual void Awake()
        {
            audioContainer = GetComponent<AudioContainer>();
            Debug.Assert(audioContainer);
        }

        public virtual void EnableCollider(bool ShouldEnable)
        {

        }
    }
}

