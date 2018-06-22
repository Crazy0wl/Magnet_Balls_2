using System;
using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class DailyBonus : Menu
    {
        #region fields
        public DailyBonusButton[] BonusButtons;
        private DailyBonusButton currentButton;
        private bool collecting;
        public int FixedDay;
        #endregion

        public override bool Show(float delay = 0f)
        {
            collecting = false;
            if (FixedDay > 0)
            {
                base.Show(delay);
                InitButtons(FixedDay);
                return true;
            }

            DateTime lastStartDate = GameData.GetDate("last_start_date");
            lastStartDate = new DateTime(lastStartDate.Year, lastStartDate.Month, lastStartDate.Day);
            DateTime nowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            TimeSpan timeSpan = nowDate - lastStartDate;

            if (timeSpan.Days == 1)
            {
                if (GameData.DailyBonusDay < 7)
                {
                    GameData.DailyBonusDay++;
                }
                else
                {
                    GameData.DailyBonusDay = 1;
                }
                if (base.Show(delay))
                {
                    GameData.SaveDate("last_start_date", DateTime.Now);
                    InitButtons(GameData.DailyBonusDay);
                    return true;
                }
            }
            else if (timeSpan.Days > 1)
            {
                GameData.DailyBonusDay = 1;
                if (base.Show())
                {
                    GameData.SaveDate("last_start_date", DateTime.Now);
                    InitButtons(GameData.DailyBonusDay);
                    return true;
                }
            }
            return false;            
        }

        private IEnumerator WaitForPress(float delay)
        {
            yield return new WaitForSeconds(delay);
            OnCollectPressed();
        }

        public override bool Hide()
        {
            UIManager.Main.Show();
            return base.Hide();
        }

        private void InitButtons(int day)
        {
            for (int i = 0; i < BonusButtons.Length; i++)
            {
                DailyBonusButton btn = BonusButtons[i];
                if (i >= day - 1)
                {
                    btn.Init(false);
                    btn.gameObject.ScaleFrom(Vector3.zero, 0.2f, i * 0.1f + 1f, EaseType.spring);
                    BonusButtons[i].Init(false);
                    int count = BonusButtons[i].Count;
                    BonusButtons[i].Count = count;
                }
                else
                {
                    btn.gameObject.SetActive(false);
                }
            }
            currentButton = BonusButtons[day - 1];
            currentButton.Init(true);
            StartCoroutine(WaitForPress(3f));
        }

        public void OnCollectPressed()
        {
            if (!collecting)
            {
                StartCoroutine(CollectRoutine());
            }
        }

        private IEnumerator CollectRoutine()
        {
            collecting = true;
            SoundManager.Play("gemAdded");
           
            while (currentButton.Count > 0)
            {
                currentButton.Spend();
                yield return new WaitForSecondsRealtime(0.01f);
            }
            yield return new WaitForSecondsRealtime(0.1f);
            currentButton.gameObject.ScaleTo(Vector3.zero, 0.5f, 0f);
            yield return new WaitForSecondsRealtime(0.2f);
            Hide();
            collecting = false;
        }
    }
}