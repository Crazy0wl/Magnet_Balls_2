using System.Collections;
using UnityEngine;
using System;
using System.IO;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

namespace MB_Engine
{
    public class GameManager : Singleton<GameManager>
    {
        #region fields
        public const float NGUI_SCALE = 0.002222222f;
        public CameraController CamController;
        public Background Background;
        private bool shareProcessing;
        public static event Action<IScore> DailyScoreInited = delegate { };
        public static event Action<IScore> WeeklyScoreInited = delegate { };
        public static event Action<IScore> OverallScoreInited = delegate { };
        public GameTitle GameTitle;
        public int FixedLevelNum;
        public GameObject Gameplay;
        public const string REMOVE_ADS_PRODUCT_ID = "magnet_balls_pro_remove_ads";
        #endregion

        protected GameManager() { }

        private void Start()
        {
            UIManager.DailyBonus.Show(0f);
            PlayGameServiceManager.SignetIn += Main_SignetIn;
            PlayGameServiceManager.UserScoreInited += Main_UserScoreInited;
            PlayGameServiceManager.ScoreReported += Main_ScoreReported;
        }

        private void Main_ScoreReported(bool obj)
        {
            PlayGameServiceManager.GetUserScore(LeaderboardCollection.Public, LeaderboardTimeSpan.Daily);
        }

        private void Main_UserScoreInited(IScore score, LeaderboardCollection collection, LeaderboardTimeSpan interval)
        {
            int best = (int)score.value;
            switch (interval)
            {
                case LeaderboardTimeSpan.Daily:
                    PlayGameServiceManager.GetUserScore(LeaderboardCollection.Public, LeaderboardTimeSpan.Weekly);
                    GameData.DailyBest = best;
                    GameData.DailyRank = score.rank;
                    if (DailyScoreInited != null)
                    {
                        DailyScoreInited(score);
                    }
                    break;
                case LeaderboardTimeSpan.Weekly:
                    PlayGameServiceManager.GetUserScore(LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime);
                    GameData.WeeklyBest = best;
                    GameData.WeeklyRank = score.rank;
                    if (WeeklyScoreInited != null)
                    {
                        WeeklyScoreInited(score);
                    }
                    break;
                case LeaderboardTimeSpan.AllTime:
                    GameData.OverallBest = best;
                    GameData.OverallRank = score.rank;
                    if (OverallScoreInited != null)
                    {
                        OverallScoreInited(score);
                    }
                    break;
            }
        }

        private void Main_SignetIn()
        {
            PlayGameServiceManager.GetUserScore(LeaderboardCollection.Public, LeaderboardTimeSpan.Daily);
        }

        public static void SetTimeScale(float scale, float duration, float delay)
        {
            main.setTimeScale(scale, duration, delay);
        }

        public void setTimeScale(float scale, float duration, float delay)
        {
            StartCoroutine(SetTimeScaleRoutine(scale, duration, delay));
        }

        private void setTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = .02f * Time.timeScale;
        }

        public void GoToFacebook()
        {
            Application.OpenURL("https://www.facebook.com/MagnetBallsGame");
        }

        private string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        private void SendEmail(string mail, string subject, string body)
        {
            subject = subject.Replace("+", "%20");
            string url = string.Format("mailto:{0}?subject={1}&body={2}", mail, subject, body);
            Application.OpenURL(url);
        }

        public static void SendEmail()
        {
            main.sendEmail();
        }

        private void sendEmail()
        {
            string email = "crazyowlgames@gmail.com";
            string subject = "Feedback";
            string body = string.Format("\n\n\n\nModel: {0}\nOS: {1}", SystemInfo.deviceModel, SystemInfo.operatingSystem);
            SendEmail(email, subject, body);
        }

        public static void Like()
        {
            main.like();
        }

        public static void GoToApp(string appId)
        {
            main.goToApp(appId);
        }

        private void like()
        {
            goToApp(Application.identifier);
        }

        private void goToApp(string appId)
        {
            string url = "market://details?id=" + appId;
            Application.OpenURL(url);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }

        public void ShareText(string subject, string body)
        {
            //execute the below lines if being run on a Android device
#if UNITY_ANDROID
            //Refernece of AndroidJavaClass class for intent
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            //Refernece of AndroidJavaObject class for intent
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            //call setAction method of the Intent object created
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            //set the type of sharing that is happening
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            //add data to be passed to the other activity i.e., the data to be sent
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
            //get the current activity
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            //start the activity by sending the intent data
            currentActivity.Call("startActivity", intentObject);
#endif
        }

        public void ShareGame()
        {
#if UNITY_ANDROID
            ShareText("Magnet Balls 2", "https://play.google.com/store/apps/details?id=com.crazyowl.MagnetBalls2");
#endif
        }

        public void ShareGameOver(string subject, string text)
        {
            if (!shareProcessing)
            {
                StartCoroutine(ShareScreenshotRoutine(subject, text));
            }
        }

        private IEnumerator ShareScreenshotRoutine(string extraSubject, string extraText)
        {
            shareProcessing = true;
            // wait for graphics to render
            yield return new WaitForEndOfFrame();
            // create the texture
            var width = Screen.width;
            var height = Screen.height;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            // Read screen contents into the texture
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            byte[] dataToSave = tex.EncodeToJPG();
            string destination = "";// Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
            File.WriteAllBytes(destination, dataToSave);
            if (!Application.isEditor)
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
                intentObject.Call<AndroidJavaObject>("setType", "text/plain");
                //add data to be passed to the other activity i.e., the data to be sent
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), extraSubject);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), extraText);
                intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", jChooser);
            }
            shareProcessing = false;
        }

        private IEnumerator SetTimeScaleRoutine(float timeScale, float duration, float delay)
        {
            yield return new WaitForSeconds(delay);
            setTimeScale(timeScale);
            Time.timeScale = timeScale;
            yield return new WaitForSeconds(duration);
            setTimeScale(1f);
        }
    }
}