using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Game.Ads
{
    public class InterstitialAdController : MonoBehaviour
    {
        #region singleton

        public static InterstitialAdController Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        #endregion
        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-3940256099942544/1033173712"; //ca-app-pub-8590881680208951/5049262604
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
  private string _adUnitId = "unused";
#endif

        private InterstitialAd _interstitialAd;
        
        public delegate void mDelegate();
        public mDelegate dShowInterstitialAdFinish;

        private void Start()
        {
            AdMediationController.Instance.dMobileAdInitialized += SetupAd;
        }

        private void SetupAd()
        {
            LoadAd();
        }

        /// <summary>
        /// Loads the interstitial ad.
        /// </summary>
        private void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            Debug.Log("Loading the interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(_adUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Interstitial ad loaded with response : "
                              + ad.GetResponseInfo());

                    _interstitialAd = ad;
                    
                    RegisterEventHandlers();
                    RegisterReloadHandler();
                });
        }
        
        /// <summary>
        /// Shows the interstitial ad.
        /// </summary>
        public void ShowInterstitialAd()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
                if (dShowInterstitialAdFinish != null) dShowInterstitialAdFinish();
            }
        }
        
        private void RegisterEventHandlers()
        {
            // Raised when the ad is estimated to have earned money.
            _interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _interstitialAd.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _interstitialAd.OnAdClicked += () =>
            {
                Debug.Log("Interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            _interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad full screen content opened.");
            };
        }
        
        private void RegisterReloadHandler()
        {
            Debug.Log("Interstitial Ad RegisterReloadHandler");
            // Raised when the ad closed full screen content.
            _interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial Ad full screen content closed.");
                // Reload the ad so that we can show another as soon as possible.
                LoadAd();
                if (dShowInterstitialAdFinish != null) dShowInterstitialAdFinish();
            };
            // Raised when the ad failed to open full screen content.
            _interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content " +
                               "with error : " + error);

                // Reload the ad so that we can show another as soon as possible.
                LoadAd();
                if (dShowInterstitialAdFinish != null) dShowInterstitialAdFinish();
            };
        }
    }
}