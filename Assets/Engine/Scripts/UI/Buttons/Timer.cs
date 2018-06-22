using System;
using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    [RequireComponent(typeof(UILabel))]
    public class Timer : MonoBehaviour
    {
        public UILabel timerLabel;
        private DateTime alarmTime;
        public bool Active;
        public event Action Alarmed = delegate { };

        public void SetTimer(int minutes)
        {
            alarmTime = DateTime.Now.AddMinutes(minutes);
            GameData.SaveDate("alarm", alarmTime);
            Active = true;
            StartCoroutine(StartCountdownRoutine());
        }

        public bool IsActive()
        {
            alarmTime = GameData.GetDate("alarm");
            TimeSpan timeSpan = alarmTime - DateTime.Now;
            return timeSpan.Seconds > 1;
        }

        private void OnEnable()
        {
            alarmTime = GameData.GetDate("alarm");
            Active = true;
            StartCoroutine(StartCountdownRoutine());
        }

        public IEnumerator StartCountdownRoutine()
        {
            TimeSpan timeSpan = alarmTime - DateTime.Now;
            while (timeSpan.Seconds > 0)
            {
                timeSpan = alarmTime - DateTime.Now;
                if (!(timeSpan.Seconds > 0))
                {
                    timerLabel.text = "";
                    Active = false;
                    if (Alarmed != null)
                    {
                        Alarmed();
                    }
                    break;
                }
                else
                {
                    timerLabel.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                    yield return new WaitForSeconds(1.0f);
                }
            }
            yield return null;
        }
    }
}