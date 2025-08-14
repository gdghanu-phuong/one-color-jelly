using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;
using Unity.Jobs;
public class AdsController : MonoBehaviour
{
    [SerializeField]
    private string bannerId;
    [SerializeField] private GameObject blackBackground;
    [SerializeField]
    private string interstitialId;

    BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            LoadBannerAd();
            LoadInterstitialAd();
        });

        if(blackBackground != null)
        {
            blackBackground.SetActive(false);
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadBannerAd()
    {
        if(_bannerView == null)
        {
            CreateBannerView();
        }
        var adRequest = new AdRequest();
        _bannerView.LoadAd(adRequest);
    }

    public void DestroyAd()
    {
        if(_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void CreateBannerView()
    {
        if(_bannerView != null)
        {
            DestroyAd();
        }
        AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(bannerId, adSize, AdPosition.Bottom);
    }

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(interstitialId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                return;
            }
            _interstitialAd = ad;
            InterstitialEvent(_interstitialAd);
            InterstitialReloadHandle(_interstitialAd);
        });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            if(blackBackground != null)
            {
                blackBackground.SetActive(true);
            }
            _interstitialAd.Show();
        }
        else
        {
            if (blackBackground != null)
            {
                blackBackground.SetActive(false);
            }
        }
    }

    private void InterstitialEvent(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Interstitial Ad Paid: {adValue.Value} {adValue.CurrencyCode}");
        };
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial Ad Impression Recorded");
        };
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial Ad Clicked");
        };
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial Ad Full Screen Content Opened");
        };
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad Full Screen Content Closed");
            if (blackBackground != null)
            {
                blackBackground.SetActive(false);
            }
            LoadInterstitialAd(); 
        };
        interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
        {
            Debug.LogError("Interstitial Ad Full Screen Content Failed: " + adError.GetMessage());
        };
    }

    private void InterstitialReloadHandle(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad Full Screen Content Closed");
            LoadInterstitialAd(); 
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
        {
            Debug.LogError("Interstitial Ad Full Screen Content Failed: " + adError.GetMessage());
            LoadInterstitialAd();
        };
    }
}
