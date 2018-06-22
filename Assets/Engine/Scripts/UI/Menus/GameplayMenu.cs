using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class GameplayMenu : Menu
    {
        #region fields
        public UILabel ScoreText;
        public UILabel BestScoreText;
        public UILabel AddScoreText;
        //public UILabel AddCoinsText;
        public UILabel CoinsText;
        public GameObject BonusPanel;
        private int currScore;
        private int currCoins;
        #endregion

        private void Start()
        {
            GameData.CoinsChanged += GameData_CoinsChanged;
            Balls.ScoreAdded += GameManager_ScoreAdded;
            Balls.BestScoreChanged += GameManager_BestScoreChanged;
            GameData.OverallBestScoreChanged += GameData_OverallBestScoreChanged;
            currCoins = GameData.Coins;
            CoinsText.text = currCoins.ToString();
        }

        private void GameData_CoinsChanged(int oldValue, int newValue)
        {
           if (gameObject.activeSelf)
            {
                StartCoroutine(AddCoinsRoutine(newValue - oldValue));
            }
        }

        private void GameData_OverallBestScoreChanged(int bestScore)
        {
            BestScoreText.text = string.Format("BEST:{0}", GameData.OverallBest);
        }

        public void ResetScore()
        {
            currScore = 0;
            ScoreText.text = string.Format("{0}", currScore);
            BestScoreText.text = string.Format("{0}", GameData.OverallBest);
        }

        public override bool Show(float delay = 0f)
        {
            if (base.Show(delay))
            {
                if (Balls.EndlessMode)
                {
                    UIManager.ShowMessage("Endless mode");
                }
                else
                {
                    UIManager.ShowMessage("Adventure mode");
                }
                GameManager.main.Gameplay.SetActive(true);
                if (Balls.Restart())
                {
                    UIManager.Info.Show();
                    Balls.Score = 0;
                    currScore = 0;
                }
                else
                {
                    currScore = Balls.Score;
                }
                AddScoreText.text = "";
                ScoreText.text = string.Format("{0}", currScore);
                BestScoreText.text = string.Format("{0}", GameData.OverallBest);
                return true;
            }
            return false;
        }

        public bool Show(Level levelProfile)
        {
            base.Show();
            GameManager.main.Gameplay.SetActive(true);
            currScore = 0;
            AddScoreText.text = "";
            Balls.Score = 0;
            ScoreText.text = string.Format("{0}", currScore);
            BestScoreText.text = string.Format("{0}", GameData.GetLevelBestScore(levelProfile.Num));
            Balls.LoadLevel(levelProfile);
            return true;
        }

        public override bool Hide()
        {
            return base.Hide();
        }

        private void GameManager_ScoreAdded(int score, int combo)
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(AddScoreRoutine(score, combo));
            }
        }

        private void GameManager_BestScoreChanged(int prevBestScore, int nextBestScore)
        {
            BestScoreText.text = string.Format("{0}", nextBestScore);
        }

        public void OnGetBonusPressed()
        {
            if (UIManager.Info.isActiveAndEnabled)
            {
                UIManager.Info.Hide();
            }
            if (UIManager.Pause.isActiveAndEnabled)
            {
                return;
            }
            if (BonusPanel.activeSelf)
            {
                BonusPanel.SetActive(false);
                Balls.Active = true;
            }
            else
            {
                BonusPanel.SetActive(true);
                Balls.Active = false;
            }
        }

        public void OnBonusBallPressed(BonusButton btn)
        {
            if (Balls.Gun.BallForShotType == BallType.Simple)
            {
                GameData.Coins -= btn.Cost;
                Balls.Gun.SetGunBallType(btn.Bonus);
                BonusPanel.SetActive(false);
                Balls.Activate(0.1f);
            }
        }

        protected override void OnBackPressed()
        {
            if (!BonusPanel.activeSelf)
            {
                UIManager.Pause.Show();
                if (UIManager.Info.isActiveAndEnabled)
                {
                    UIManager.Info.Hide();
                }
            }
        }

        public void OnPausePressed()
        {
            if (!BonusPanel.activeSelf)
            {
                UIManager.Pause.Show();
            }
        }

        private IEnumerator AddScoreRoutine(int score, int combo)
        {
            AddScoreText.gameObject.ScaleTo(Vector3.one * 1.2f, 1f, 0f, EaseType.easeOutElastic);
            AddScoreText.text = combo > 1 ?
                string.Format("+{0} X {1}", score, combo) :
                string.Format("+{0}", score);
            yield return new WaitForEndOfFrame();
            while (currScore++ < Balls.Score)
            {
                ScoreText.text = string.Format("{0}", currScore);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSecondsRealtime(1f);
            AddScoreText.gameObject.ScaleTo(Vector3.zero, 1f, 0.2f);
        }
        
        private IEnumerator AddCoinsRoutine(int coins)
        {
            //AddCoinsText.gameObject.ScaleTo(Vector3.one * 1.2f, 1f, 0f, EaseType.easeOutElastic);
            //AddCoinsText.text = string.Format("+{0}", coins);
            yield return new WaitForEndOfFrame();
            while (currCoins++ < GameData.Coins)
            {
                CoinsText.text  = string.Format("{0}", currCoins);
                yield return new WaitForEndOfFrame();
            }
            CoinsText.text = string.Format("{0}", GameData.Coins);
            yield return new WaitForSecondsRealtime(1f);
            //AddCoinsText.gameObject.ScaleTo(Vector3.zero, 1f, 0.2f);
        }
    }
}