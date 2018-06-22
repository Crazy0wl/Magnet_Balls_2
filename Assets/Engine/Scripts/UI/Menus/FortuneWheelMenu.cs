using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class FortuneWheelMenu : Menu
    {
        #region fields
        public SpinWheel Wheel;
        private AudioSource audioSource;
        //public AudioClip WinClip;
        public UIButton SpinBtn;
        public UIButton CloseBtn;
        #endregion

        private void Start()
        {
            Wheel.SpinFinished += Wheel_SpinFinished;
            AdManager.AdRewarded += AdManager_AdRewarded;
        }

        public override bool Show(float delay = 0)
        {
            SpinBtn.isEnabled = true;
            CloseBtn.isEnabled = true;
            return base.Show(delay);
        }

        private void Spin()
        {
            Wheel.Spin();
            SpinBtn.isEnabled = false;
            CloseBtn.isEnabled = false;
        }

        private void AdManager_AdRewarded()
        {
            Spin();
        }

        private void Wheel_SpinFinished(WheelBonus prize)
        {
            StartCoroutine(GetPrizeRoutine(prize));
        }

        private IEnumerator GetPrizeRoutine(WheelBonus prize)
        {
            if (prize.Count > 0)
            {
                SoundManager.Play("gemAdded");
            }
            yield return StartCoroutine(prize.GetPrizeRoutine());
            yield return new WaitForSeconds(1f);
            Hide();
        }

        public void OnSpinPressed()
        {
            if (!Wheel.Spining)
            {
#if !UNITY_EDITOR
                 AdManager.ShowRewarded();
#else
                Spin();
#endif
            }
        }

        public void OnClosePressed()
        {
            Hide();
        }
    }
}