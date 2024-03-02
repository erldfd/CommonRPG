using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private List<Image> progressBarImageList = new List<Image>();

        private List<float> fillAmountList = null;

        private bool isInitialized = false;

        public void Init()
        {
            Debug.Assert(progressBarImageList.Count > 0);

            fillAmountList = new List<float>(progressBarImageList.Count);

            foreach (Image image in progressBarImageList)
            {
                fillAmountList.Add(0);
            }
        }

        /// <summary>
        ///  <para> index : index of progressBarImageList. </para>
        ///  you can multi progressBar if you want.
        ///  first progressBar index is 0, second is 1, ect...
        /// </summary>
        public void SetProgressBarFillAmount(int index, float fillAmount)
        {
            if (isInitialized == false) 
            {
                Init();
            }

            if (index < 0 || index >= progressBarImageList.Count) 
            {
                Debug.LogAssertion("index is out of range");
                return;
            }
            
            progressBarImageList[index].fillAmount = fillAmount;
            fillAmountList[index] = fillAmount;
        }
    }

}
