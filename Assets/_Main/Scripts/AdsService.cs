using GoogleMobileAds.Api;
using UnityEngine;

public class AdsService
{
    private BannerView _bannerView;
    private InterstitialAd _interstitial;

    private const string BannerId = "ca-app-pub-4344619008612907/7237419271";
    private const string InterId = "ca-app-pub-4344619008612907/6017800390";
    
    public void Init()
    {
        MobileAds.Initialize(status =>{});
        LoadInterstitialAd();
    }
    
    private void CreateBanner()
    {
        if (_bannerView != null)
        {
            DestroyAd();
        }

        _bannerView = new BannerView(BannerId, AdSize.Banner, AdPosition.Top);
    }

    public void LoadAd()
    {
        if (_bannerView == null)
        {
            CreateBanner();
        }

        var adRequest = new AdRequest();
            
        _bannerView?.LoadAd(adRequest);
    }

    public void HideAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Hide();
        }
    }
        
    private void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    
    public void LoadInterstitialAd()
    {
        if (_interstitial != null)
        {
            _interstitial.Destroy();
            _interstitial = null;
        }


        var adRequest = new AdRequest();

        InterstitialAd.Load(InterId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    return;
                }

                _interstitial = ad;
                    
                RegisterReloadHandler(ad);
            });
    }
        
        
    public void ShowInterstitialAd()
    {
        if (_interstitial != null && _interstitial.CanShowAd())
        {
            _interstitial.Show();
        }
    }
        
    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd();
        };
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadInterstitialAd();
        };
    }
}
