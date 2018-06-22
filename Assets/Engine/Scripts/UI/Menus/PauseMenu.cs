using UnityEngine;

namespace MB_Engine
{
    public class PauseMenu : Menu
    {
        #region fields
        public GameObject MainMenuBtn;
        public GameObject RestartBtn;
        public GameObject ResumeBtn;
        #endregion

        private void Start()
        {
            ShowCompleted += PauseMenu_ShowCompleted;
        }

        private void PauseMenu_ShowCompleted()
        {
            Time.timeScale = 0f;
        }

        public override bool Show(float delay = 0f)
        {
            if (base.Show(delay))
            {
                if (UIManager.Info.isActiveAndEnabled)
                {
                    UIManager.Info.Hide();
                }
                Balls.Active = false;
                return true;
            }
            return false;
        }

        public override bool Hide()
        {
            if (base.Hide())
            {
                Time.timeScale = 1f;
                Balls.Active = true;
                return true;
            }
            return false;
        }

        protected override void OnBackPressed()
        {
            Hide();
        }

        public void OnResumePressed()
        {
            Hide();
        }

        public void OnMainMenuPressed()
        {
            Hide();

            Balls.Active = false;
            GameManager.main.Gameplay.SetActive(false);
            UIManager.Gameplay.Hide();
            UIManager.Main.Show();
        }

        public void OnRestartPressed()
        {
            if (Hide())
            {
                AdManager.ShowInterstitial();
                Balls.Restart();
            }
        }
    }
}