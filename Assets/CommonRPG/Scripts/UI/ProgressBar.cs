using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private float fillAmount = 0.5f;
        public float FillAmount
        {
            get
            {
                return fillAmount;
            }
            set
            {
                fillAmount = value;
                progressBarImage.fillAmount = fillAmount;
            }
        }

        [SerializeField]
        private Image progressBarImage;

        private void Awake()
        {
            Debug.Assert(progressBarImage);
        }
    }

}
