using System;
using UnityEngine;
using UnityEngine.Advertisements;
public class BannerAds : MonoBehaviour
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;
    private string adUnitId;
    private bool isAdLoaded = false;
    void Start()
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        LoadBannerAd();
    }
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

    public void LoadBannerAd()
    {
        isAdLoaded = false;
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        Advertisement.Banner.Load(adUnitId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner ad loaded successfully.");
        isAdLoaded = true;
        LoadBannerAd();
    }

    private void OnBannerError(string message)
    {
        Debug.LogError("Banner ad failed to load: " + message);
    }

    public void ShowBannerAd()
    {
        if (!isAdLoaded)
        {
            Debug.Log("Banner not loaded yet. Waiting...");
            return;
        }

        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };
        Advertisement.Banner.Show(adUnitId, options);
    }

    private void OnBannerShown(){ }

    private void OnBannerHidden(){ }

    private void OnBannerClicked(){ }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }
}

