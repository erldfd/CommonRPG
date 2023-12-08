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
        private int initialRepeatNumber;

        public TimerHandler(float startTime, float interval, int repeatNumber, Action function, bool isActive)
        {
            StartTime = startTime;
            Interval = interval;
            RepeatNumber = repeatNumber;
            initialRepeatNumber = repeatNumber;
            Function = function;
            this.isActive = isActive;
        }

        public void ClearTimer()
        {
            StartTime = 0;
            Interval = 0;
            RepeatNumber = 0;
            ElapsedTime = 0;

            isActive = false;
            initialRepeatNumber = 0;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void SetActive(bool newIsActive)
        {
            isActive = newIsActive;
        }

        public void RestartTimer()
        {
            ElapsedTime = 0;
            RepeatNumber = initialRepeatNumber;
            isActive = true;
        }

        public void PauseTimer()
        {
            isActive = false;
        }

        public void ResumeTimer()
        {
            isActive = true;
        }
    }

    public class TimerManager : MonoBehaviour
    {
        private TimerManager instance = null;

        private LinkedList<TimerHandler> ActivatedTimerHandlers = new LinkedList<TimerHandler>();
        private Queue<TimerHandler> DeactivatedTimerHandlers = new Queue<TimerHandler>();

        private void Awake()
        {
            instance = this;
            Debug.Assert(instance);
        }

        private void FixedUpdate()
        {
            LinkedListNode<TimerHandler> node = ActivatedTimerHandlers.First;

            while (node != null)
            {
                TimerHandler handler = node.Value;

                if (handler.IsActive() == false)
                {
                    continue;
                }

                bool isFirstRunEnded = (handler.StartTime == 0);
                if (isFirstRunEnded == false)
                {
                    if (handler.StartTime <= handler.ElapsedTime)
                    {
                        handler.StartTime = 0;
                        handler.ElapsedTime = 0;
                        handler.Function.Invoke();
                    }
                }
                else if (handler.RepeatNumber > 0)
                {
                    if (handler.Interval <= handler.ElapsedTime)
                    {
                        handler.RepeatNumber--;
                        handler.ElapsedTime = 0;
                        handler.Function.Invoke();
                    }
                }
                else
                {
                    node = node.Next;
                    DeactivatedTimerHandlers.Enqueue(handler);
                    ActivatedTimerHandlers.Remove(handler);
                    Debug.Log($"Activated TimerHander Count : {ActivatedTimerHandlers.Count}, Deactivated TimerHandler Count : {DeactivatedTimerHandlers.Count}");
                    continue;
                }

                handler.ElapsedTime += Time.fixedDeltaTime;
                node = node.Next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> return value is timerhandler which has been set. </returns>
        public TimerHandler SetTimer(float startTime, float interval, int repeatNumber, Action function, bool isActive)
        {
            TimerHandler timerHandler;

            if (DeactivatedTimerHandlers.Count == 0)
            {
                timerHandler = new TimerHandler(startTime, interval, repeatNumber, function, isActive);
            }
            else
            {
                timerHandler = DeactivatedTimerHandlers.Dequeue();
            }

            ActivatedTimerHandlers.AddLast(timerHandler);

            Debug.Log($"Activated TimerHander Count : {ActivatedTimerHandlers.Count}, Deactivated TimerHandler Count : {DeactivatedTimerHandlers.Count}");
            return timerHandler;
        }
    }
}
