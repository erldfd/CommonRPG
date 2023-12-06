using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonRPG
{
    public class TimerHandler
    {
        public float StartTime;
        public float Interval;
        /// <summary>
        /// it except first run. for example, RepeatNumber == 0, Active Once, RepeatNumber == 1, Active Twice.
        /// </summary>
        public int RepeatNumber;
        public float ElapsedTime;
        public Action Function;

        private bool isActive;

        public void ClearTimer()
        {
            StartTime = 0;
            Interval = 0;
            RepeatNumber = 0;
            ElapsedTime = 0;

            isActive = false;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void SetActive(bool newIsActive)
        {
            isActive = newIsActive;
        }
    }

    public class TimerManager : MonoBehaviour
    {
        private TimerManager instance = null;

        private List<TimerHandler> TimerHandleList = new List<TimerHandler>();
        private void Awake()
        {
            instance = this;
            Debug.Assert(instance);
        }

        private void FixedUpdate()
        {
            int timerHandleListCount = TimerHandleList.Count;
            for (int i = 0; i < timerHandleListCount; ++i)
            {
                TimerHandler timerHandler = TimerHandleList[i];
                if (timerHandler.IsActive() == false)
                {
                    continue;
                }

                bool isFirstRunEnded = (timerHandler.StartTime == 0);
                if (isFirstRunEnded == false)
                {
                    if (timerHandler.StartTime <= timerHandler.ElapsedTime)
                    {
                        TimerHandleList[i].StartTime = 0;
                        TimerHandleList[i].ElapsedTime = 0;
                        TimerHandleList[i].Function.Invoke();
                        continue;
                    }
                }
                else if (timerHandler.RepeatNumber > 0)
                {
                    if (timerHandler.Interval <= timerHandler.ElapsedTime)
                    {
                        TimerHandleList[i].ElapsedTime = 0;
                        TimerHandleList[i].RepeatNumber--;
                        TimerHandleList[i].Function.Invoke();
                        continue;
                    }
                }
                else
                {
                    //TimerHandleList.RemoveAt(i);
                    //TODO : remove or recycle used timerhandle
                    continue;
                }

                TimerHandleList[i].ElapsedTime += Time.fixedDeltaTime;
            }
        }

        public void SetTimer(TimerHandler timerHandler)
        {
            timerHandler.SetActive(true);
            TimerHandleList.Add(timerHandler);
        }
    }
}
