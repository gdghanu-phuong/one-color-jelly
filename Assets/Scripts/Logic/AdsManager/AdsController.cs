using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;
using Unity.Jobs;
public class AdsController : MonoBehaviour
{
    public InitializeAds initializeAds;
    public BannerAds bannerAds;
    public InterstitialAds interstitialAds;

    public static AdsController instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadBannerAds()
    {
        bannerAds.LoadBannerAd();
    }

    public void LoadInterstitialAds()
    {
        interstitialAds.LoadInterstitialAd();
    }

    public void ShowInterstitialAd()
    {
        interstitialAds.ShowInterstitialAd();
    }
}
