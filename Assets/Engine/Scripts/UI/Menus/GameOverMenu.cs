using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class GameOverMenu : Menu
    {
        #region fields
        public GameObject MenuBtn;
        public GameObject RestartBtn;
        public GameObject ShareBtn;
        public GameObject NewBestStamp;
        public UILabel ScoreText;
        public UILabel BestScoreText;

        public UILabel DailyRankLabel;
        public UILabel WeeklyRankLabel;
        public UILabel OverallRankLabel;

        public GameObject DailyRankObject;
        public GameObject WeeklyRankObject;
        public GameObject OverallRankObject;

        public GameObject DailyRankLoading;
        public GameObject WeeklyRankLoading;
        public GameObject OverallRankLoading;
        #endregion

        private void Start()
        {
            GameData.DailyRankChanged += GameData_DailyRankChanged;
            GameData.WeeklyRankChanged += GameData_WeeklyRankChanged;
            GameData.OverallRankChanged += GameData_OverallRankChanged;
            GameData.OverallBestScoreChanged += GameData_OverallBestScoreChanged;
        }

        private void GameData_OverallBestScoreChanged(int obj)
        {
            BestScoreText.text = string.Format("{0}", obj);
        }

        private void GameData_OverallRankChanged(int oldValue, int newValue)
        {
            StartCoroutine(ChangeOverallRankRoutine(oldValue, newValue));
        }

        private void GameData_WeeklyRankChanged(int oldValue, int newValue)
        {
            StartCoroutine(ChangeWeeklyRankRoutine(oldValue, newValue));
        }

        private void GameData_DailyRankChanged(int oldValue, int newValue)
        {
            StartCoroutine(ChangeDailyRankRoutine(oldValue, newValue));
        }

        private IEnumerator ChangeDailyRankRoutine(int oldValue, int newValue)
        {
            if (newValue > 0)
            {
                DailyRankLabel.text = oldValue.ToString();
                DailyRankLoading.SetActive(false);

                float currValue = 0f;
                for (float f = 0; f <= 1f; f += Time.deltaTime)
                {
                    currValue = Mathf.Lerp(oldValue, newValue, f);
                    DailyRankLabel.text = string.Format("{0}", (int)currValue);
                    yield return null;
                }
                DailyRankLabel.text = string.Format("{0}", newValue);
                yield return null;
            }
        }

        private IEnumerator ChangeWeeklyRankRoutine(int oldValue, int newValue)
        {
            if (newValue > 0)
            {
                WeeklyRankLabel.text = oldValue.ToString();
                WeeklyRankLoading.SetActive(false);

                float currValue = 0f;
                for (float f = 0; f <= 1f; f += Time.deltaTime)
                {
                    currValue = Mathf.Lerp(oldValue, newValue, f);
                    WeeklyRankLabel.text = string.Format("{0}", (int)currValue);
                    yield return null;
                }
                WeeklyRankLabel.text = string.Format("{0}", newValue);
                yield return null;
            }
        }

        private IEnumerator ChangeOverallRankRoutine(int oldValue, int newValue)
        {
            if (newValue > 0)
            {
                OverallRankLabel.text = oldValue.ToString();
                OverallRankLoading.SetActive(false);

                float currValue = 0f;
                for (float f = 0; f <= 1f; f += Time.deltaTime)
                {
                    currValue = Mathf.Lerp(oldValue, newValue, f);
                    OverallRankLabel.text = string.Format("{0}", (int)currValue);
                    yield return null;
                }
                OverallRankLabel.text = string.Format("{0}", newValue);
                yield return null;
            }
        }

        public override bool Show(float delay = 0f)
        {
            if (base.Show(delay))
            {
                Balls.WasGameOver = true;
                DailyRankObject.SetActive(false);
                WeeklyRankObject.SetActive(false);
                OverallRankObject.SetActive(false);
                NewBestStamp.transform.localScale = Vector3.zero;
                StartCoroutine(ShowGameOverRoutine());
                return true;
            }
            return false;
        }

        private IEnumerator ShowRanksRoutine()
        {
            yield return new WaitForSeconds(0.1f);
            DailyRankObject.SetActive(true);
            DailyRankLabel.text = "";
            DailyRankObject.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
            yield return new WaitForSeconds(0.1f);
            WeeklyRankObject.SetActive(true);
            WeeklyRankLabel.text = "";
            WeeklyRankObject.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
            yield return new WaitForSeconds(0.1f);
            OverallRankObject.SetActive(true);
            OverallRankLabel.text = "";
            OverallRankObject.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
        }

        public void OnSharePressed()
        {
            GameManager.main.ShareText("Try to beat my record", string.Format("I scored {0} points in Magnet Balls Pro\n https://play.google.com/store/apps/details?id=com.crazyowl.MagnetBallsPro", GameData.OverallBest));
        }

        private IEnumerator ShowGameOverRoutine()
        {
            Balls.Active = false;
            OverallRankLoading.SetActive(false);
            DailyRankLoading.SetActive(false);
            WeeklyRankLoading.SetActive(false);
            UIManager.Gameplay.Hide();
            MenuBtn.transform.position = new Vector3(-400f, -150f, 0f) * GameManager.NGUI_SCALE;
            RestartBtn.transform.position = new Vector3(400f, -150f, 0f) * GameManager.NGUI_SCALE;
            ShareBtn.transform.position = new Vector3(0f, -500f, 0f) * GameManager.NGUI_SCALE;
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(ShowRanksRoutine());
          //  if (Balls.Score > GameData.DailyBest)
            {
                OverallRankLoading.SetActive(true);
                DailyRankLoading.SetActive(true);
                WeeklyRankLoading.SetActive(true);
                PlayGameServiceManager.ReportScore(Balls.Score);
            }
            yield return new WaitForEndOfFrame();
            ScoreText.text = string.Format("{0}", Balls.Score);
            BestScoreText.text = string.Format("{0}", GameData.OverallBest);
            yield return new WaitForEndOfFrame();
            if (Balls.Score > GameData.OverallBest)
            {
                GameData.OverallBest = Balls.Score;
                yield return new WaitForSecondsRealtime(0.5f);
                BestScoreText.text = string.Format("{0}", GameData.OverallBest);
                yield return new WaitForSecondsRealtime(0.5f);
                NewBestStamp.SetActive(true);
                NewBestStamp.ScaleTo(Vector3.one, 0.5f, 0f, EaseType.spring);
            }
            yield return new WaitForSeconds(0.5f);
            MenuBtn.MoveTo(new Vector3(-150f, -150f, 0f) * GameManager.NGUI_SCALE, 0.5f, 0f, EaseType.spring);
            ShareBtn.MoveTo(new Vector3(0f, -200f, 0f) * GameManager.NGUI_SCALE, 0.5f, 0f, EaseType.spring);
            RestartBtn.MoveTo(new Vector3(150f, -150f, 0f) * GameManager.NGUI_SCALE, 0.5f, 0f, EaseType.spring);
            yield return new WaitForSeconds(0.5f);
        }

        public void MainMenuPressed()
        {
            if (Hide())
            {
                Balls.Active = false;
                Balls.DestroyBalls();
                GameManager.main.Gameplay.SetActive(false);
                UIManager.Gameplay.Hide();
                UIManager.Main.Show();
            }
        }

        public void RestartPressed()
        {
            if (Hide())
            {
                UIManager.Gameplay.Show();
                AdManager.ShowInterstitial();
                Balls.Restart();
            }
        }

        protected override void OnBackPressed()
        {
            MainMenuPressed();
        }
    }
}