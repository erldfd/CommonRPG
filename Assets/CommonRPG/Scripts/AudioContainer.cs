using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class AudioContainer : MonoBehaviour
    {
        [SerializeField]
        private List<AudioClip> audioClipList;
        public List<AudioClip> AudioClipList
        {
            get
            {
                return audioClipList;
            }
        }
    }
}
