// Copyright (C) 2015 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System;
using System.Collections.Generic;

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class Utils
    {
        #region Fully-qualified class names

        #region Google Mobile Ads SDK class names

        public const string AdListenerClassName = "com.google.android.gms.ads.AdListener";

        public const string AdRequestClassName = "com.google.android.gms.ads.AdRequest";

        public const string AdRequestBuilderClassName =
                "com.google.android.gms.ads.AdRequest$Builder";

        public const string AdSizeClassName = "com.google.android.gms.ads.AdSize";

        public const string AdMobExtrasClassName =
                "com.google.android.gms.ads.mediation.admob.AdMobExtras";

        public const string AppOpenAdClassName =
                "com.google.android.gms.ads.appopen.AppOpenAd";

        public const string PlayStorePurchaseListenerClassName =
            "com.google.android.gms.ads.purchase.PlayStorePurchaseListener";

        public const string MobileAdsClassName = "com.google.android.gms.ads.MobileAds";

        public const string RequestConfigurationClassName = "com.google.android.gms.ads.RequestConfiguration";

        public const string RequestConfigurationBuilderClassName = "com.google.android.gms.ads.RequestConfiguration$Builder";

        public const string ServerSideVerificationOptionsClassName =
            "com.google.android.gms.ads.rewarded.ServerSideVerificationOptions";

        public const string ServerSideVerificationOptionsBuilderClassName =
            "com.google.android.gms.ads.rewarded.ServerSideVerificationOptions$Builder";

        #endregion

        #region Google Mobile Ads Unity Plugin class names

        public const string UnityAdSizeClassName = "com.google.unity.ads.UnityAdSize";

        public const string BannerViewClassName = "com.google.unity.ads.Banner";

        public const string InterstitialClassName = "com.google.unity.ads.Interstitial";

        public const string RewardBasedVideoClassName = "com.google.unity.ads.RewardBasedVideo";

        public const string UnityRewardedAdClassName = "com.google.unity.ads.UnityRewardedAd";

        public const string UnityAdListenerClassName = "com.google.unity.ads.UnityAdListener";

        public const string UnityRewardedAdCallbackClassName =
            "com.google.unity.ads.UnityRewardedAdCallback";

        public const string UnityInterstitialAdCallbackClassName = "com.google.unity.ads.UnityInterstitialAdCallback";

        public const string UnityFullScreenContentCallbackClassName = "com.google.unity.ads.UnityFullScreenContentCallback";

        public const string UnityAdapterStatusEnumName =
                "com.google.android.gms.ads.initialization.AdapterStatus$State";

        public const string UnityAppOpenAdClassName = "com.google.unity.ads.UnityAppOpenAd";

        public const string UnityAppOpenAdCallbackClassName =
                "com.google.unity.ads.UnityAppOpenAdCallback";

        public const string OnInitializationCompleteListenerClassName =
            "com.google.android.gms.ads.initialization.OnInitializationCompleteListener";

        public const string UnityAdLoaderListenerClassName =
            "com.google.unity.ads.UnityAdLoaderListener";

        public const string UnityAdInspectorClassName =
            "com.google.unity.ads.UnityAdInspector";

        public const string UnityAdInspectorListenerClassname =
            "com.google.unity.ads.UnityAdInspectorListener";

        public const string UnityPaidEventListenerClassName =
            "com.google.unity.ads.UnityPaidEventListener";

        public const string UnityRewardedInterstitialAdClassName =
            "com.google.unity.ads.UnityRewardedInterstitialAd";

        public const string UnityRewardedInterstitialAdCallbackClassName =
            "com.google.unity.ads.UnityRewardedInterstitialAdCallback";

        public const string PluginUtilsClassName = "com.google.unity.ads.PluginUtils";

        #endregion

        #region Unity class names

        public const string UnityActivityClassName = "com.unity3d.player.UnityPlayer";

        #endregion

        #region Android SDK class names

        public const string BundleClassName = "android.os.Bundle";
        public const string DateClassName = "java.util.Date";
        public const string DisplayMetricsClassName = "android.util.DisplayMetrics";

        #endregion

        #endregion

        #region JavaObject creators

        public static AndroidJavaObject GetAdSizeJavaObject(AdSize adSize)
        {
            AndroidJavaClass adSizeClass = new AndroidJavaClass(UnityAdSizeClassName);
            switch (adSize.AdType)
            {
                case AdSize.Type.SmartBanner:
                    return adSizeClass.CallStatic<AndroidJavaObject>("getSmartBannerAdSize");
                case AdSize.Type.AnchoredAdaptive:

                    AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
                    AndroidJavaObject activity =
                        playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                    switch (adSize.Orientation)
                    {
                        case Orientation.Landscape:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getLandscapeAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
                        case Orientation.Portrait:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getPortraitAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
                        case Orientation.Current:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getCurrentOrientationAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
                        default:
                            throw new ArgumentException("Invalid Orientation provided for ad size.");
                    }
                case AdSize.Type.Standard:
                    return new AndroidJavaObject(AdSizeClassName, adSize.Width, adSize.Height);
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided for ad size.");
            }
        }

        public static int GetAppOpenAdOrientation(ScreenOrientation orientation)
        {
            string orientationFieldName;
            switch (orientation)
            {
                case ScreenOrientation.Landscape:
                case ScreenOrientation.LandscapeRight:
                    orientationFieldName = "APP_OPEN_AD_ORIENTATION_LANDSCAPE";
                    break;
                default:
                    orientationFieldName = "APP_OPEN_AD_ORIENTATION_PORTRAIT";
                    break;
            }

            AndroidJavaClass appOpenAdClass = new AndroidJavaClass(AppOpenAdClassName);
            return appOpenAdClass.GetStatic<int>(orientationFieldName);
        }

        internal static int GetScreenWidth()
        {
            DisplayMetrics metrics = new DisplayMetrics();
            return (int)(metrics.WidthPixels / metrics.Density);
        }

        /// <summary>
        /// Converts the plugin AdRequest object to a native java proxy object for use by the sdk.
        /// </summary>
        /// <param name="AdRequest">the AdRequest from the unity plugin.</param>
        /// <param name="nativePluginVersion">the version string of the native plugin.</param>
        public static AndroidJavaObject GetAdRequestJavaObject(AdRequest request,
                                                               string nativePluginVersion = null)
        {
            AndroidJavaObject adRequestBuilder = new AndroidJavaObject(AdRequestBuilderClassName);
            foreach (string keyword in request.Keywords)
            {
                adRequestBuilder.Call<AndroidJavaObject>("addKeyword", keyword);
            }

            // Denote that the request is coming from this Unity plugin.
            adRequestBuilder.Call<AndroidJavaObject>(
                    "setRequestAgent",
                    nativePluginVersion);
            AndroidJavaObject bundle = new AndroidJavaObject(BundleClassName);
            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                bundle.Call("putString", entry.Key, entry.Value);
            }

            bundle.Call("putString", "is_unity", "1");

            // Makes ads that contain WebP ad assets ineligible.
            bundle.Call("putString", "adw", "true");

            AndroidJavaObject extras = new AndroidJavaObject(AdMobExtrasClassName, bundle);
            adRequestBuilder.Call<AndroidJavaObject>("addNetworkExtras", extras);

            foreach (MediationExtras mediationExtra in request.MediationExtras)
            {
                AndroidJavaObject mediationExtrasBundleBuilder =
                    new AndroidJavaObject(mediationExtra.AndroidMediationExtraBuilderClassName);
                AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");

                foreach (KeyValuePair<string, string> entry in mediationExtra.Extras)
                {
                    map.Call<AndroidJavaObject>("put", entry.Key, entry.Value);
                }

                AndroidJavaObject mediationExtras =
                        mediationExtrasBundleBuilder.Call<AndroidJavaObject>("buildExtras", map);

                if (mediationExtras != null)
                {
                    adRequestBuilder.Call<AndroidJavaObject>(
                        "addNetworkExtrasBundle",
                        mediationExtrasBundleBuilder.Call<AndroidJavaClass>("getAdapterClass"),
                        mediationExtras);

                    adRequestBuilder.Call<AndroidJavaObject>(
                        "addCustomEventExtrasBundle",
                        mediationExtrasBundleBuilder.Call<AndroidJavaClass>("getAdapterClass"),
                        mediationExtras);
                }
            }

            return adRequestBuilder.Call<AndroidJavaObject>("build");
        }

        public static AndroidJavaObject GetJavaListObject(List<String> csTypeList)
        {

            AndroidJavaObject javaTypeArrayList = new AndroidJavaObject("java.util.ArrayList");
            foreach (string itemList in csTypeList)
            {
                javaTypeArrayList.Call<bool>("add", itemList);
            }
            return javaTypeArrayList;
        }

        public static List<String> GetCsTypeList(AndroidJavaObject javaTypeList)
        {
            List<String> csTypeList = new List<String>();
            int length = javaTypeList.Call<int>("size");
            for (int i = 0; i < length; i++)
            {
                csTypeList.Add(javaTypeList.Call<string>("get", i));
            }

            return csTypeList;
        }

        public static AndroidJavaObject GetServerSideVerificationOptionsJavaObject(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            AndroidJavaObject serverSideVerificationOptionsBuilder = new AndroidJavaObject(ServerSideVerificationOptionsBuilderClassName);
            serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("setUserId", serverSideVerificationOptions.UserId);
            serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("setCustomData", serverSideVerificationOptions.CustomData);

            return serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("build");
        }
        #endregion
    }
}
