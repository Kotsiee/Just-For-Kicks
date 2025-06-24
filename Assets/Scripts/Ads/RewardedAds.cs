using GoogleMobileAds.Api;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RewardedAds : MonoBehaviour
{
        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-9726053065426778/1243674180";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        private const string _adUnitId = "unused";
#endif

        private RewardedAd _rewardedAd;
    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) => {});
    }

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd(UnityAction function, UnityAction onFailed, UnityAction onLoad)
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading rewarded ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                _rewardedAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad, onFailed, onLoad);
                ShowAd(function, onFailed);
            });
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd(UnityAction function, UnityAction onFailed)
        {
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                Debug.Log("Showing rewarded ad.");
                _rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log(String.Format("Rewarded ad granted a reward: {0} {1}",
                                            reward.Amount,
                                            reward.Type));

                    function.Invoke();
                });
            }
            else
            {
                onFailed.Invoke();
                DestroyAd();
            }
        }


    /// <summary>
    /// Destroys the ad.
    /// </summary>
        public void DestroyAd()
        {
            if (_rewardedAd != null)
            {
                Debug.Log("Destroying rewarded ad.");
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_rewardedAd != null)
            {
                var responseInfo = _rewardedAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        private void RegisterEventHandlers(RewardedAd ad, UnityAction onFailed, UnityAction onLoad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when the ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded ad full screen content opened.");
                onLoad?.Invoke();
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad full screen content closed.");
                DestroyAd();
                onFailed?.Invoke();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content with error : "
                    + error);
                DestroyAd();
                onFailed?.Invoke();
            };
        }
}
