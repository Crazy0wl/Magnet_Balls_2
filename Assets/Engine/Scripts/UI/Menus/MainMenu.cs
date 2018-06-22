using UnityEngine;

namespace MB_Engine
{
    public class MainMenu : Menu
    {
        #region fields
        public GameTitle Title;
        public UILabel DailyRankLabel;
        public GameObject LeaderboardIcon;
        public UIButton FortuneWheelBtn;
        public GameObject ShareBtn;
        #endregion

        private void Start()
        {
            ShareBtn.ScaleFrom(Vector3.zero, 0.5f, 0.5f, EaseType.spring);
            FortuneWheelBtn.isEnabled = false;
            GameData.DailyRankChanged += GameData_DailyRankChanged;
            GameData.BallStyleChanged += GameData_BallStyleChanged;
            AdManager.RewardedAdLoaded += AdManager_RewardedAdLoaded;
            AdManager.RewardedAdFailedToLoad += AdManager_RewardedAdFailedToLoad;
        }

        public void OnGetCoinsPressed()
        {

        }

        private void GameData_BallStyleChanged(BallStyle ballStyle)
        {
        }

        private void AdManager_RewardedAdFailedToLoad()
        {
            FortuneWheelBtn.isEnabled = false;
        }

        private void AdManager_RewardedAdLoaded()
        {
            FortuneWheelBtn.isEnabled = true;
        }

        private void GameData_DailyRankChanged(int oldValue, int newValue)
        {
            if (newValue > 0)
            {
                LeaderboardIcon.SetActive(false);
                DailyRankLabel.gameObject.SetActive(true);
                DailyRankLabel.text = newValue.ToString();
            }
            else
            {
                LeaderboardIcon.SetActive(true);
                DailyRankLabel.gameObject.SetActive(false);
            }
        }

        private void Main_RankInited(int obj)
        {
            LeaderboardIcon.SetActive(false);
            DailyRankLabel.gameObject.SetActive(true);
            DailyRankLabel.text = obj.ToString();
        }

        public void OnPlayPressed()
        {
            if (Hide())
            {
            }
        }

        public override bool Show(float delay = 0f)
        {
            UIManager.ClearMessage();
            if (base.Show(delay))
            {
                AdManager.RequestRewardedAd();
                Title.gameObject.SetActive(true);
                Title.Show(ShowDuration);
                return true;
            }
            return false;
        }

        public override bool Hide()
        {
            Title.Hide(HideDuration);
            return base.Hide();
        }

        public void OnPlayAdventureMode()
        {
            if (!UIManager.DailyBonus.Active && Hide())
            {
                Balls.EndlessMode = false;
                UIManager.LevelList.Show(0f);
            }
        }

        public void OnPlayEndlessPressed()
        {
            if (!UIManager.DailyBonus.Active && Hide())
            {
                Balls.EndlessMode = true;
                UIManager.Gameplay.Show();
            }
        }

        public void OnRemoveAdsPressed()
        {
            Purchaser.main.PurchaseProduct(GameManager.REMOVE_ADS_PRODUCT_ID, false);
        }

        public void OnSettingsPressed()
        {
            UIManager.Settings.Show();
        }

        public void OnFortuneWheelPressed()
        {
            UIManager.FortuneWheel.Show();
            FortuneWheelBtn.isEnabled = false;
        }

        public void OnLeaderboardPressed()
        {
            PlayGameServiceManager.ShowLeaderboard();
        }
       
        public void OnAboutPressed()
        {
            UIManager.Info.Show();
        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }

        public void OnLikePressed()
        {
            GameManager.Like();
        }

        public void OnSharePressed()
        {
            GameManager.main.ShareGame();
        }

        public void OnFacebookPressed()
        {
            Application.OpenURL("https://www.facebook.com/MagnetBallsGame");
        }

        protected override void OnBackPressed()
        {
            OnQuitPressed();
        }

        public void OnMoreGamesPressed()
        {
            Application.OpenURL("market://dev?id=8626125173664403696");
        }
    }
}