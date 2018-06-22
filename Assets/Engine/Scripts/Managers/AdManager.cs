using UnityEngine;
using System.Collections;
#if UNITY_ANDROID
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
#endif
using System;


namespace MB_Engine
{
    public class AdManager : Singleton<AdManager>
    {
        protected AdManager() { }

        public string AdMobBannerId;
        public string AdMobInterstitialId;
        public string AdMobRewardedId;
#if UNITY_ANDROID
        private InterstitialAd adMobInterstitial;
        private RewardBasedVideoAd adMobRevarded;
#endif
        private bool inited;
        public static event Action AdRewarded = delegate { };
        public static event Action RewardedAdLoaded = delegate { };
        public static event Action RewardedAdClosed = delegate { };
        public static event Action RewardedAdStarted = delegate { };
        public static event Action RewardedAdOpened = delegate { };
        public static event Action RewardedAdFailedToLoad = delegate { };
        private bool adsActive;
        public GameObject RemoveAdsBtn;
        public GameObject GameBanners;

        void Start()
        {
            if (!inited)
            {
                StartCoroutine(InitRoutine());
            }
        }

        public static void ShowRewarded()
        {
            #if UNITY_ANDROID
            main.showRewarded();
            #endif
        }

        public static void ShowInterstitial()
        {
            #if UNITY_ANDROID
            main.showInterstitial();
            #endif
        }

        public static void RequestRewardedAd()
        {
            #if UNITY_ANDROID
            main.requestRewardedVideo();
            #endif
        }

        private IEnumerator InitRoutine()
        {
            RemoveAdsBtn.SetActive(false);
            GameBanners.SetActive(false);
            inited = false;
            yield return new WaitForSeconds(1f);
#if UNITY_ANDROID
            adMobInterstitial = new InterstitialAd(AdMobInterstitialId);
            yield return new WaitForSeconds(0.1f);
            adMobRevarded = RewardBasedVideoAd.Instance;
            adMobRevarded.OnAdLoaded += RewardBasedVideo_OnAdLoaded;
            adMobRevarded.OnAdFailedToLoad += RewardBasedVideo_OnAdFailedToLoad;
            adMobRevarded.OnAdOpening += RewardBasedVideo_OnAdOpening;
            adMobRevarded.OnAdStarted += RewardBasedVideo_OnAdStarted;
            adMobRevarded.OnAdRewarded += RewardBasedVideo_OnAdRewarded;
            adMobRevarded.OnAdClosed += RewardBasedVideo_OnAdClosed;
            adMobRevarded.OnAdLeavingApplication += RewardBasedVideo_OnAdLeavingApplication;
            requestRewardedVideo();
            yield return new WaitForSeconds(0.1f);
            adsActive = Purchaser.main.ProductActive(GameManager.REMOVE_ADS_PRODUCT_ID);
            RemoveAdsBtn.SetActive(adsActive);
            GameBanners.SetActive(adsActive);
            if (adsActive)
            {
                RemoveAdsBtn.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
                GameBanners.ScaleFrom(Vector3.zero, 0.25f, 0f, EaseType.spring);
            }
            #endif
            inited = true;
        }

        #if UNITY_ANDROID
        private void RewardBasedVideo_OnAdLeavingApplication(object sender, EventArgs e)
        {
        }

        private void RewardBasedVideo_OnAdClosed(object sender, EventArgs e)
        {
            if (RewardedAdClosed != null)
            {
                RewardedAdClosed();
            }
        }

        private void RewardBasedVideo_OnAdRewarded(object sender, Reward e)
        {
            if (AdRewarded != null)
            {
                AdRewarded();
            }
        }

        private void RewardBasedVideo_OnAdStarted(object sender, EventArgs e)
        {
            if (RewardedAdStarted != null)
            {
                RewardedAdStarted();
            }
        }

        private void RewardBasedVideo_OnAdOpening(object sender, EventArgs e)
        {
            if (RewardedAdOpened != null)
            {
                RewardedAdOpened();
            }
        }

        private void RewardBasedVideo_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            if (RewardedAdFailedToLoad != null)
            {
                RewardedAdFailedToLoad();
            }
        }

        private void RewardBasedVideo_OnAdLoaded(object sender, EventArgs e)
        {
            if (RewardedAdLoaded != null)
            {
                RewardedAdLoaded();
            }
        }

        private void showRewardedUnityAd()
        {
            if (Advertisement.IsReady("rewardedVideo"))
            {
                var options = new ShowOptions { resultCallback = handleUnityAdShowResult };
                Advertisement.Show("rewardedVideo", options);
            }
        }

        private void handleUnityAdShowResult(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    if (AdRewarded != null)
                    {
                        AdRewarded();
                    }
                    break;
                case ShowResult.Skipped:
                    Debug.Log("The ad was skipped before reaching the end.");
                    break;
                case ShowResult.Failed:
                    Debug.LogError("The ad failed to be shown.");
                    break;
            }
        }

        private void requestRewardedVideo()
        {
            if (!Advertisement.IsReady("rewardedVideo"))
            {
                if (!adMobRevarded.IsLoaded())
                {
                    AdRequest request = new AdRequest.Builder().Build();
                    adMobRevarded.LoadAd(request, AdMobRewardedId);
                }
                else
                {
                    RewardedAdLoaded();
                }
            }
            else
            {
                RewardedAdLoaded();
            }
        }

        private void showInterstitial()
        {
            adsActive = Purchaser.main.ProductActive(GameManager.REMOVE_ADS_PRODUCT_ID);
            RemoveAdsBtn.SetActive(adsActive);
            GameBanners.SetActive(adsActive);
            if (inited && adsActive)
            {
                if (!ShowAdMobInterstitial())
                {
                    if (!ShowUnityInterstitial())
                    {

                    }
                }
            }
        }

        private void showRewarded()
        {
            if (inited)
            {
                if (adMobRevarded != null)
                {
                    if (adMobRevarded.IsLoaded())
                    {
                        adMobRevarded.Show();
                    }
                    else
                    {
                        showRewardedUnityAd();
                    }
                }
            }
        }

        private bool ShowAdMobInterstitial()
        {
            bool result = false;
            if (adMobInterstitial != null)
            {
                if (adMobInterstitial.IsLoaded())
                {
                    result = true;
                    adMobInterstitial.Show();
                }
                else
                {
                    adMobInterstitial.LoadAd(new AdRequest.Builder().Build());
                }
            }
            else
            {
                adMobInterstitial = new InterstitialAd(AdMobInterstitialId);
            }
            return result;
        }

        private bool ShowUnityInterstitial()
        {
            bool result = false;
            if (Advertisement.IsReady() && !Advertisement.isShowing)
            {
                Advertisement.Show();
                result = true;
            }
            return result;
        }
        #endif
    }
}