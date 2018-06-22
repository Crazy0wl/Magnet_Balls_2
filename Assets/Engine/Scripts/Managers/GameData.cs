using System;
using UnityEngine;

namespace MB_Engine
{
    public enum GameDifficult
    {
        Easy,
        Normal,
        Hard
    }

    public class GameData : Singleton<GameData>
    {
        #region events
        public static event Action<int, int> CoinsChanged = delegate { };
        public static event Action<int, int> LivesChanged = delegate { };
        public static event Action<int, BallType> BallsCountChanged = delegate { };
        public static event Action<int, int> LevelBestScoreChanged = delegate { };

        public static event Action<int> DailyBestScoreChanged = delegate { };
        public static event Action<int> WeeklyBestScoreChanged = delegate { };
        public static event Action<int> OverallBestScoreChanged = delegate { };

        public static event Action<int, int> DailyRankChanged = delegate { };
        public static event Action<int, int> WeeklyRankChanged = delegate { };
        public static event Action<int, int> OverallRankChanged = delegate { };
        public static event Action<BallStyle> BallStyleChanged = delegate { };
        #endregion

        public int StartGems = 999;

        public bool ClearData;

        public static int CurrLevelsBlock
        {
            get { return PlayerPrefs.GetInt("CurrLevelsBlock", 0); }
            set { PlayerPrefs.SetInt("CurrLevelsBlock", value); }
        }

        void Start()
        {
            if (ClearData)
            {
                PlayerPrefs.DeleteAll();
            }
            if (Coins < StartGems)
            {
                Coins = StartGems;
            }
        }

        public static BallStyle BallStyle
        {
            get { return (BallStyle)PlayerPrefs.GetInt("BallStyle", 0); }
            set
            {
                PlayerPrefs.SetInt("BallStyle", (int)value);
                if (BallStyleChanged != null)
                {
                    BallStyleChanged(value);
                }
            }
        }

        public static int OverallRank
        {
            get { return PlayerPrefs.GetInt("OverallRank", 0); }
            set
            {
                if (OverallRankChanged != null)
                {
                    OverallRankChanged(OverallRank, value);
                }
                PlayerPrefs.SetInt("OverallRank", value);
            }
        }

        public static int WeeklyRank
        {
            get { return PlayerPrefs.GetInt("WeeklyRank", 0); }
            set
            {
                if (WeeklyRankChanged != null)
                {
                    WeeklyRankChanged(WeeklyRank, value);
                }
                PlayerPrefs.SetInt("WeeklyRank", value);
            }
        }

        public static int DailyRank
        {
            get { return PlayerPrefs.GetInt("DailyRank", 0); }
            set
            {
                if (DailyRankChanged != null)
                {
                    DailyRankChanged(DailyRank, value);
                }
                PlayerPrefs.SetInt("DailyRank", value);
            }
        }

        public static int OverallBest
        {
            get { return PlayerPrefs.GetInt("OverallBest", 0); }
            set
            {
                if (value != OverallBest)
                {
                    if (OverallBestScoreChanged != null)
                    {
                        OverallBestScoreChanged(value);
                    }
                    PlayerPrefs.SetInt("OverallBest", value);
                }
            }
        }

        public static int WeeklyBest
        {
            get { return PlayerPrefs.GetInt("WeeklyBest", 0); }
            set
            {
                if (value != WeeklyBest)
                {
                    if (WeeklyBestScoreChanged != null)
                    {
                        WeeklyBestScoreChanged(value);
                    }
                    PlayerPrefs.SetInt("WeeklyBest", value);
                }
            }
        }

        public static int DailyBest
        {
            get { return PlayerPrefs.GetInt("DailyBest", 0); }
            set
            {
                if (value != DailyBest)
                {
                    if (DailyBestScoreChanged != null)
                    {
                        DailyBestScoreChanged(value);
                    }
                    PlayerPrefs.SetInt("DailyBest", value);
                }
            }
        }

        public static int BestScore
        {
            get { return PlayerPrefs.GetInt("best", 0); }
            set { PlayerPrefs.SetInt("best", value); }
        }

        protected GameData() { }

        public static int DailyBonusDay
        {
            get { return PlayerPrefs.GetInt("daily_bonus_day", 0); }
            set { PlayerPrefs.SetInt("daily_bonus_day", value); }
        }

        public static GameDifficult Difficult
        {
            get { return (GameDifficult)PlayerPrefs.GetInt("game_difficult", 0); }
            set { PlayerPrefs.SetInt("game_difficult", (int)value); }
        }

        public static void SaveDate(string key, DateTime date)
        {
            // YYYY:MM:DD:hh:mm:ss
            string dateStr = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            PlayerPrefs.SetString(key, dateStr);
        }

        public static DateTime GetDate(string key)
        {
            string dateStr = PlayerPrefs.GetString(key, string.Empty);
            if (dateStr != string.Empty)
            {
                string[] dateStrArr = dateStr.Split(':');
                if (dateStrArr.Length == 6)
                {
                    int year = 0;
                    if (int.TryParse(dateStrArr[0], out year))
                    {
                        int month = 0;
                        if (int.TryParse(dateStrArr[1], out month))
                        {
                            int day = 0;
                            if (int.TryParse(dateStrArr[2], out day))
                            {
                                int hour = 0;
                                if (int.TryParse(dateStrArr[3], out hour))
                                {
                                    int minute = 0;
                                    if (int.TryParse(dateStrArr[4], out minute))
                                    {
                                        int second = 0;
                                        if (int.TryParse(dateStrArr[5], out second))
                                        {
                                            return new DateTime(year, month, day, hour, minute, second);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return DateTime.MinValue;
        }

        public static int Coins
        {
            get { return PlayerPrefs.GetInt("Coins", 0); }
            set
            {
                int oldValue = Coins;
                PlayerPrefs.SetInt("Coins", value);
                if (CoinsChanged != null)
                {
                    CoinsChanged(oldValue, value);
                }
            }
        }

        public static int Lives
        {
            get { return PlayerPrefs.GetInt("lives", 100); }
            set
            {
                int oldValue = Lives;
                PlayerPrefs.SetInt("lives", value);
                if (LivesChanged != null)
                {
                    LivesChanged(oldValue, value);
                }
            }
        }

        public static int MaxLevel
        {
            get { return PlayerPrefs.GetInt("max_level", 1); }
            set
            {
                if (MaxLevel < value)
                {
                    PlayerPrefs.SetInt("max_level", value);
                }
            }
        }

        public static bool Muted
        {
            get { return PlayerPrefs.GetInt("muted", 0) == 1; }
            set { PlayerPrefs.SetInt("muted", value ? 1 : 0); }
        }

        public static bool AdsRemoved
        {
            get { return PlayerPrefs.GetInt("ads_removed", 0) == 1; }
            set { PlayerPrefs.SetInt("ads_removed", value ? 1 : 0); }
        }

        public static bool AccelActive
        {
            get { return PlayerPrefs.GetInt("AccelActive", 1) == 1; }
            set { PlayerPrefs.SetInt("AccelActive", value ? 1 : 0); }
        }

        public static int GetBallsCount(BallType ballType)
        {
            string key = string.Format("balls_count_{0}", ballType);
            return PlayerPrefs.GetInt(key, -1);
        }

        public static bool BallEnabled(BallType ballType)
        {
            return GetBallsCount(ballType) != -1;
        }

        public static void SpendBall(BallType ball)
        {
            if (ball != BallType.None)
            {
                int count = GetBallsCount(ball);
                if (count > 0)
                {
                    count--;
                    SetBallsCount(ball, count);
                }
            }
        }

        public static void AddBall(BallType ball, int addCount = 1)
        {
            if (ball != BallType.None)
            {
                int count = GetBallsCount(ball);
                count += addCount;
                SetBallsCount(ball, count);
            }
        }

        public static void SetBallsCount(BallType ballType, int count)
        {
            if (ballType != BallType.None)
            {
                string key = string.Format("balls_count_{0}", ballType);
                int oldValue = GetBallsCount(ballType);
                PlayerPrefs.SetInt(key, count);
                if (BallsCountChanged != null && oldValue != count)
                {
                    BallsCountChanged(count, ballType);
                }
            }
        }

        public static int GetLevelBestScore(int levelNum)
        {
            string key = string.Format("level_best_{0}", levelNum);
            return PlayerPrefs.GetInt(key, 0);
        }

        public static void SetLevelBestScore(int levelNum, int bestScore)
        {
            int prevBestScore = GetLevelBestScore(levelNum);
            if (bestScore > prevBestScore)
            {
                string key = string.Format("level_best_{0}", levelNum);
                PlayerPrefs.SetInt(key, bestScore);
                if (LevelBestScoreChanged != null)
                {
                    LevelBestScoreChanged(levelNum, bestScore);
                }
            }
        }

        public static void SetLevelStars(int levelNum, int starsCount)
        {
            if (GetLevelStars(levelNum) < starsCount)
            {
                string key = string.Format("level_stars_{0}", levelNum);
                PlayerPrefs.SetInt(key, starsCount);
            }
        }

        public static int GetLevelStars(int levelNum)
        {
            string key = string.Format("level_stars_{0}", levelNum);
            return PlayerPrefs.GetInt(key, 0);
        }
    }
}