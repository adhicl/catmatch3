using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Game.Ads
{
    public class NativeAdController : MonoBehaviour
    {   
        #region singleton

        public static NativeAdController Instance { get; set; }

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
        private string _adUnitId = "a-app-pub-8590881680208951/9382425139"; //"ca-app-pub-3940256099942544/2247696110";    //
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/3986624511";
#else
  private string _adUnitId = "unused";
#endif

        private NativeOverlayAd _nativeOverlayAd;

        private void Start()
        {
            AdMediationController.Instance.dMobileAdInitialized += LoadAd;
        }

        /// <summary>
        /// Loads the ad.
        /// </summary>
        private void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_nativeOverlayAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading Native overlay ad.");

            // Create a request used to load the ad.
            var adRequest = new AdRequest();

            // Optional: Define native ad options.
            var options = new NativeAdOptions
            {
                AdChoicesPlacement = AdChoicesPlacement.TopRightCorner,
                MediaAspectRatio = MediaAspectRatio.Portrait,
            };

            // Send the request to load the ad.
            NativeOverlayAd.Load(_adUnitId, adRequest, options,
                (NativeOverlayAd ad, LoadAdError error) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Native Overlay ad failed to load an ad " +
                                       " with error: " + error);
                        return;
                    }

                    // The ad should always be non-null if the error is null, but
                    // double-check to avoid a crash.
                    if (ad == null)
                    {
                        Debug.LogError("Unexpected error: Native Overlay ad load event " +
                                       " fired with null ad and null error.");
                        return;
                    }

                    // The operation completed successfully.
                    Debug.Log("Native Overlay ad loaded with response : " +
                              ad.GetResponseInfo());
                    _nativeOverlayAd = ad;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers();
                });
        }
        
        /// <summary>
        /// Renders the ad.
        /// </summary>
        private void RenderAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Rendering Native Overlay ad.");

                // Define a native template style with a custom style.
                var style = new NativeTemplateStyle
                {
                    MainBackgroundColor = Color.white,
                    CallToActionText = new NativeTemplateTextStyle
                    {
                        BackgroundColor = Color.green,
                        TextColor = Color.white,
                        FontSize = 9,
                        Style = NativeTemplateFontStyle.Bold
                    }
                };
                style.TemplateId = NativeTemplateId.Medium;

                // Renders a native overlay ad at the default size
                // and anchored to the bottom of the screne.
                _nativeOverlayAd.RenderTemplate(style, AdPosition.Center);
            }
        }
        
        /// <summary>
        /// listen to events the banner view may raise.
        /// </summary>
        private void RegisterEventHandlers()
        {
            // Raised when the ad is estimated to have earned money.
            _nativeOverlayAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Native view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _nativeOverlayAd.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Native view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _nativeOverlayAd.OnAdClicked += () =>
            {
                Debug.Log("Native view was clicked.");
            };
            // Raised when an ad opened full screen content.
            _nativeOverlayAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Native view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _nativeOverlayAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Native view full screen content closed.");
            };
        }
        
        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Showing Native Overlay ad.");
                _nativeOverlayAd.Show();
                RenderAd();
            }
        }
        
        /// <summary>
        /// Hides the ad.
        /// </summary>
        public void HideAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Hiding Native Overlay ad.");
                _nativeOverlayAd.Hide();
            }
        }
        
        /// <summary>
        /// Destroys the native overlay ad.
        /// </summary>
        private void DestroyAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Destroying native overlay ad.");
                _nativeOverlayAd.Destroy();
                _nativeOverlayAd = null;
            }
        }
    }
}