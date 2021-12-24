// Workaround for https://github.com/googleads/googleads-mobile-unity/issues/1616

typedef const void *GADUTypeInterstitialRef;

typedef const void *GADUTypeRewardedAdRef;

typedef const void *GADUTypeRequestRef;

void GADURequestInterstitial(GADUTypeInterstitialRef interstitial, GADUTypeRequestRef request) { }

void GADURequestRewardedAd(GADUTypeRewardedAdRef rewardedAd, GADUTypeRequestRef request) { }
