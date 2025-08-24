using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;
    [SerializeField] private BannerAds bannerAds;
    private string adUnitId;
    private bool isAdLoaded = false;
    private void Awake()
    {
        #if UNITY_IOS
            adUnitId = iosAdUnitId;
        #elif UNITY_ANDROID
            adUnitId = androidAdUnitId;
        #elif UNITY_EDITOR
            adUnitId = androidAdUnitId;
        #endif
    }

    public void LoadInterstitialAd()
    {
        isAdLoaded = false;
        Advertisement.Load(adUnitId, this);
    }

    public void ShowInterstitialAd()
    {
        if (isAdLoaded)
        {
            Advertisement.Show(adUnitId, this);
            LoadInterstitialAd();
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial ad loaded: " + placementId);
        isAdLoaded = true;
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message){ }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (bannerAds != null)
        {
            bannerAds.ShowBannerAd();
        }
        LoadInterstitialAd();
    }
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message){ }

    public void OnUnityAdsShowStart(string placementId){ }
    public void OnUnityAdsShowClick(string placementId){ }
}
