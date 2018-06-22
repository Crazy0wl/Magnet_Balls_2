using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class LevelCompletedMenu : Menu
    {
        #region fields
        public UILabel LevelNumText;
        public UILabel ScoreText;
        public UILabel BestScoreText;
        private Level completedLevelProfile;
        public GameObject NewBest;
        public LevelStars Stars;
        private int levelBestScore;
        public bool processing;
        public GameObject MainMenuBtn;
        public GameObject RestartBtn;
        public GameObject NextLevelBtn;
        #endregion

        public override bool Show(float delay = 0f)
        {
            if (base.Show(delay))
            {
                StartCoroutine(ShowScoreRoutine());
            }
            return true;
        }

        //private int GetTotalScore()
        //{
        //    int result = 0;
        //    for (int i = 0; i < GameData.MaxLevel; i++)
        //    {
        //        result += GameData.GetLevelBestScore(i);
        //    }
        //    return result;
        //}

        //private int GetTotalScore(int exeptLevel)
        //{
        //    int result = 0;
        //    for (int i = 0; i < GameData.MaxLevel; i++)
        //    {
        //        if (i != exeptLevel)
        //        {
        //            result += GameData.GetLevelBestScore(i);
        //        }
        //    }
        //    return result;
        //}

        public void MainMenuPressed()
        {
            if (Hide() && !processing)
            {
                Balls.Active = false;
                GameManager.main.Gameplay.SetActive(false);
                UIManager.Gameplay.Hide();
                UIManager.Main.Show();
            }
        }

        public void RestartPressed()
        {
            if (Hide() && !processing) 
            {
                UIManager.Gameplay.Show(completedLevelProfile);
            }
        }

        public void NextLevelPressed()
        {
            if (!processing && Hide())
            {
                Level nextLevelProfile = Levels.main.levels[completedLevelProfile.Num];
                if (nextLevelProfile != null)
                {
                    UIManager.Gameplay.Show(nextLevelProfile);
                }
            }
        }

        private IEnumerator AddScoreRoutine(int levelNum, int score, int bestScore)
        {
            processing = true;
            int newBestScore = bestScore;
            bool newLevel = false;
            float currScore = 0f;
            for (float f = 0f; f <= 1f; f += 0.01f)
            {
                currScore = Mathf.Lerp(0f, score, f);
                ScoreText.text = string.Format("{0}", currScore);
                yield return new WaitForEndOfFrame();
            }
            if (newBestScore > levelBestScore)
            {
                NewBest.gameObject.SetActive(true);
                NewBest.ScaleTo(Vector3.one, 0.5f, 0f, EaseType.spring);
                GameData.SetLevelBestScore(levelNum, newBestScore);
            }
            if (newLevel)
            {
               // UIManager.NewLevelUpMenu.Show();
            }
            yield return new WaitForSecondsRealtime(0f);

            MainMenuBtn.SetActive(true);
            MainMenuBtn.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
            yield return new WaitForSecondsRealtime(0.2f);

            RestartBtn.SetActive(true);
            RestartBtn.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
            yield return new WaitForSecondsRealtime(0.2f);

            NextLevelBtn.SetActive(true);
            NextLevelBtn.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
            processing = false;
        }

        private IEnumerator ShowScoreRoutine()
        {
           
            processing = true;
            NewBest.gameObject.SetActive(false);
            completedLevelProfile = Balls.main.CurrLevelProfile;
            
            levelBestScore = GameData.GetLevelBestScore(completedLevelProfile.Num);
            MainMenuBtn.SetActive(false);
            RestartBtn.SetActive(false);
            NextLevelBtn.SetActive(false);
            LevelNumText.text = string.Format("LEVEL: {0}", completedLevelProfile.Num);
            BestScoreText.text = string.Format("{0}", levelBestScore);
            GameData.MaxLevel = completedLevelProfile.Num + 1;
            int currScore = 0;
            float targetScore = Balls.Score;
            Balls.Active = false;
            Stars.Show(completedLevelProfile.FirstStarScore, completedLevelProfile.SecondStarScore, completedLevelProfile.ThirdStarScore);
            int starsCount = Stars.GetStarsCount((int)targetScore);
            GameData.SetLevelStars(completedLevelProfile.Num, starsCount);

            for (float f = 0f; f <= 1f; f += 0.01f)
            {
                currScore = (int)Mathf.Lerp(0f, targetScore, f);
                Stars.UpdateState(currScore);
                ScoreText.text = string.Format("{0}", currScore);
                yield return null;
            }
            ScoreText.text = string.Format("{0}", (int)targetScore);
            if (Balls.Score > levelBestScore)
            {
                yield return new WaitForSeconds(0.5f);
                GameData.SetLevelBestScore(completedLevelProfile.Num, Balls.Score);
                NewBest.gameObject.SetActive(true);
                NewBest.gameObject.ScaleFrom(Vector3.zero, 0.5f, 0f);
                BestScoreText.text = string.Format("{0}", Balls.Score);
            }
            yield return new WaitForSeconds(0.1f);
            MainMenuBtn.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            RestartBtn.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            NextLevelBtn.SetActive(true);
            processing = false;
            yield return null;
        }
    }
}