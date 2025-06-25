using System;
#if (UNITY_ANDROID || UNITY_IOS)
using GoogleMobileAds.Api;
#endif
using UnityEngine;

namespace Game
{
    public class AdMediationController : MonoBehaviour
    {
        #region singleton

        public static AdMediationController Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        #endregion
        
        public delegate void mDelegate();
        public mDelegate dMobileAdInitialized;
        
        private void Start()
        {
#if (UNITY_ANDROID || UNITY_IOS)
            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                if (initstatus == null)
                {
                    Debug.LogError("Google Mobile Ads initialization failed.");
                    return;
                }

                if (dMobileAdInitialized != null) dMobileAdInitialized();
                Debug.Log("Google Mobile Ads initialization complete.");
            });
#endif

        }
    }
}