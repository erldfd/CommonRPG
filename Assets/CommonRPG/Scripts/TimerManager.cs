using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonRPG
{
    public class TimerHandler
    {
        /// <summary>
        /// it means time until starting first run. if StartTime == -1, timemr dont use first run..
        /// </summary>
        public float StartTime;
        public float Interval;
        /// <summary>
        /// it except first run. for example, RepeatNumber == 0, Active Once, RepeatNumber == 1, Active Twice.
        /// if RepeatNumber == -1, repeat infinitely until calling ClearTimer
        /// </summary>
        public int RepeatNumber;
        public float ElapsedTime;
        public Action Function;

        private bool isActive;
        private bool isStayingActive;
        /// <summary>
        /// if true, this timerHandler is always active.
        /// but when timerHanlder is done, timerHanlder is paused.
        /// </summary>
        public bool IsStayingActive
        {
            get { return isStayingActive; }
            set { isStayingActive = value; }
        }

        private bool isPaused;
        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        private float initialStartTime;
        private int initialRepeatNumber;

        public TimerHandler(float startTime, float interval, int repeatNumber, Action function, bool isActive)
        {
            StartTime = startTime;
            initialStartTime = startTime;
            Interval = interval;
            RepeatNumber = repeatNumber;
            initialRepeatNumber = repeatNumber;
            Function = function;
            this.isActive = isActive;
            isStayingActive = false;
            isPaused = false;
        }

        public void ClearTimer()
        {
            StartTime = 0;
            Interval = 0;
            RepeatNumber = 0;
            ElapsedTime = 0;

            isActive = false;
            isStayingActive = false;
            isPaused = false;
            initialRepeatNumber = 0;
            initialStartTime = 0;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void SetActive(bool newIsActive)
        {
            isActive = newIsActive;
        }

        /// <summary>
        /// this method make hanlder restart from beginning.
        /// </summary>
        public void RestartTimer()
        {
            ElapsedTime = 0;
            RepeatNumber = initialRepeatNumber;
            StartTime = initialStartTime;
            isActive = true;
            isPaused = false;
        }

        public void ResetTimer(float startTime, float interval, int repeatNumber, Action function, bool isActive)
        {
            StartTime = startTime;
            initialStartTime = startTime;
            Interval = interval;
            RepeatNumber = repeatNumber;
            initialRepeatNumber = repeatNumber;
            Function = function;
            this.isActive = isActive;
            isStayingActive = false;
            isPaused = false;
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

                if (handler.IsPaused)
                {
                    node = node.Next;
                    continue;
                }

                bool isFirstRunEnded = (handler.StartTime == -1);
                if (isFirstRunEnded == false)
                {
                    if (handler.StartTime <= handler.ElapsedTime)
                    {
                        handler.StartTime = -1;
                        handler.ElapsedTime = 0;
                        handler.Function.Invoke();
                    }
                }
                else if (handler.RepeatNumber > 0 || handler.RepeatNumber == -1)
                {
                    if (handler.Interval <= handler.ElapsedTime)
                    {
                        if (handler.RepeatNumber > 0) 
                        {
                            handler.RepeatNumber--;
                        }

                        handler.ElapsedTime = 0;
                        handler.Function.Invoke();
                    }
                }
                else
                {
                    node = node.Next;

                    if (handler.IsStayingActive)
                    {
                        handler.IsPaused = true;
                        continue;
                    }

                    handler.SetActive(false);
                    DeactivatedTimerHandlers.Enqueue(handler);
                    ActivatedTimerHandlers.Remove(node.Previous);

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
                timerHandler.ResetTimer(startTime, interval, repeatNumber, function, isActive);
            }

            ActivatedTimerHandlers.AddLast(timerHandler);
            return timerHandler;
        }
    }
}
