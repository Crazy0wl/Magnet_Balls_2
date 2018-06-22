using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Video;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MB_Engine
{
    public class PlayGameServiceManager : Singleton<PlayGameServiceManager>
    {
        private bool Authenticated { get { return Social.Active.localUser.authenticated; } }
        private bool authenticating;
        public static event Action<bool> ScoreReported = delegate { };
        public static event Action SignetIn = delegate { };
        public static event Action<IScore, LeaderboardCollection, LeaderboardTimeSpan> UserScoreInited = delegate { };

        protected PlayGameServiceManager() { }

        void Start()
        {
            StartCoroutine(InitRoutine());
        }

        public static void ReportScore(int score)
        {
            main.reportScore(score);
        }

        public static void LoadFriends()
        {
            main.loadFriends();
        }

#if UNITY_ANDROID
        public static VideoCaptureState GetCurrentVieoCaptureState()
        {
            return main.getCurrentVideoCaptureState();
        }
#endif

        public static void GetUserScore(LeaderboardCollection collection, LeaderboardTimeSpan interval)
        {
            main.getUserScore(collection, interval);
        }

        public static void SignOut()
        {
            main.signOut();
        }

        public static void ShowLeaderboard()
        {
            main.showLeaderboard();
        }

        private void loadFriends()
        {
            Social.localUser.LoadFriends((ok) =>
            {
                foreach (IUserProfile p in Social.localUser.friends)
                {
                    Debug.Log(p.userName + " is a friend");
                }
            });
        }

#if UNITY_ANDROID
        private VideoCaptureState getCurrentVideoCaptureState()
        {
            VideoCaptureState result = null;
            PlayGamesPlatform.Instance.Video.GetCaptureState(
              (status, state) =>
              {
                  bool isSuccess = CommonTypesUtil.StatusIsSuccess(status);
                  if (isSuccess)
                  {
                      if (state.IsCapturing)
                      {
                          result = state;
                          Debug.Log("Currently capturing to " + state.CaptureMode.ToString() + " in " +
                                    state.QualityLevel.ToString());
                      }
                      else
                      {
                          Debug.Log("Not currently capturing.");
                      }
                  }
                  else
                  {
                      Debug.Log("Error: " + status.ToString());
                  }
              });
            return result;
        }
#endif
        private void getUserScore(LeaderboardCollection collection, LeaderboardTimeSpan interval)
        {
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.LoadScores(GPGSIds.leaderboard_magnet_balls_pro, LeaderboardStart.PlayerCentered, 1, collection, interval,
                (data) =>
                {
                    if (data.Scores.Length > 0)
                    {
                        if (UserScoreInited != null)
                        {
                            UserScoreInited(data.PlayerScore, collection, interval);
                        }
                    }
                }
            );
#endif
        }

        private void launchVideoCaptureOverlay()
        {
#if UNITY_ANDROID
            if (PlayGamesPlatform.Instance.Video.IsCaptureSupported())
            {
                PlayGamesPlatform.Instance.Video.IsCaptureAvailable(VideoCaptureMode.File,
                  (status, isAvailable) =>
                  {
                      bool isSuccess = CommonTypesUtil.StatusIsSuccess(status);
                      if (isSuccess)
                      {
                          if (isAvailable)
                          {
                              PlayGamesPlatform.Instance.Video.ShowCaptureOverlay();
                          }
                          else
                          {
                              Debug.Log("Video capture is unavailable. Is the overlay already open?");
                          }
                      }
                      else
                      {
                          Debug.Log("Error: " + status.ToString());
                      }
                  });
            }
#endif
        }

        private bool getVideoCaptureCapabilities()
        {
            bool result = false;
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.Video.GetCaptureCapabilities(
              (status, capabilities) =>
              {
                  bool isSuccess = CommonTypesUtil.StatusIsSuccess(status);
                  if (isSuccess)
                  {
                      if (capabilities.IsCameraSupported && capabilities.IsMicSupported &&
                          capabilities.IsWriteStorageSupported &&
                          capabilities.SupportsCaptureMode(VideoCaptureMode.File) &&
                          capabilities.SupportsQualityLevel(VideoQualityLevel.SD))
                      {
                          result = true;
                          Debug.Log("All requested capabilities are present.");
                      }
                      else
                      {
                          result = false;
                          Debug.Log("Not all requested capabilities are present!");
                      }
                  }
                  else
                  {
                      result = false;
                      Debug.Log("Error: " + status.ToString());
                  }
              });
#endif
            return result;
        }

        private Texture2D getScreenshot()
        {
            Texture2D screenShot = new Texture2D(1024, 700);
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, (Screen.width / 1024) * 700), 0, 0);
            return screenShot;
        }

        private IEnumerator InitRoutine()
        {
#if UNITY_ANDROID
            yield return new WaitForSeconds(0.1f);
            PlayGamesPlatform.Activate();
            yield return new WaitForSeconds(0.1f);
            ((PlayGamesPlatform)Social.Active).SetDefaultLeaderboardForUI(GPGSIds.leaderboard_magnet_balls_pro);
            yield return new WaitForSeconds(0.1f);
            authenticate();
#endif
            yield return new WaitForSeconds(0.1f);
        }

        private void getPlayerStarts()
        {
#if UNITY_ANDROID
            ((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
            {
                // -1 means cached stats, 0 is succeess
                // see  CommonStatusCodes for all values.
                if (rc <= 0 && stats.HasDaysSinceLastPlayed())
                {
                    Debug.Log("It has been " + stats.DaysSinceLastPlayed + " days");
                }
            });
#endif
        }

        private void authenticate()
        {
            if (Authenticated || authenticating)
            {
                Debug.LogWarning("Ignoring repeated call to Authenticate().");
                return;
            }
#if UNITY_ANDROID
            authenticating = true;
            Social.localUser.Authenticate((bool success) =>
            {
                authenticating = false;
                if (SignetIn != null && success)
                {
                    SignetIn();
                }
            });
#endif
        }

        private void signOut()
        {
#if UNITY_ANDROID
            ((PlayGamesPlatform)Social.Active).SignOut();
#endif
        }

        private Texture2D captureScreenshot()
        {
            Texture2D mScreenImage = new Texture2D(Screen.width, Screen.height);
            mScreenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            mScreenImage.Apply();
            Debug.Log("Captured screen: " + mScreenImage);
            return mScreenImage;
        }

        private void showLeaderboard()
        {
            if (Authenticated)
            {
                Social.ShowLeaderboardUI();
            }
            else
            {
                authenticate();
            }
        }

        private void showAchivements()
        {
#if UNITY_ANDROID
            if (Authenticated)
            {
                Social.ShowAchievementsUI();
            }
#endif
        }

        private void reportScore(int score)
        {
#if UNITY_ANDROID
            if (Authenticated && score > GameData.BestScore)
            {
                Social.ReportScore(score, GPGSIds.leaderboard_magnet_balls_pro, (bool success) =>
                {
                    ScoreReported(success);
                });
            }
#endif
        }
    }
}