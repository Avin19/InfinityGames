using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance { get; private set; }

    [Header("Test Ad Unit IDs (Replace Before Release)")]
    [SerializeField]
    private string bannerAdUnitId =
        "ca-app-pub-1779486678797596/1850340463";

    [SerializeField]
    private string interstitialAdUnitId =
        "ca-app-pub-1779486678797596/2042105678";

    [SerializeField]
    private string rewardedAdUnitId =
        "ca-app-pub-1779486678797596/8043395485";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private int loadingAttemp = 0;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MobileAds.Initialize(status =>
        {
            Debug.Log("AdMob initialized");
            LoadBanner();
            LoadInterstitial();
            LoadRewarded();
        });
    }

    // =========================
    // BANNER
    // =========================
    public void SetLoadingAttempt()
    {
        loadingAttemp = 0;
    }
    public void LoadBanner()
    {
        bannerView?.Destroy();

        bannerView = new BannerView(
            bannerAdUnitId,
            AdSize.Banner,
            AdPosition.Bottom
        );

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    public void ShowBanner()
    {
        Debug.Log(loadingAttemp);
        if (loadingAttemp > 3) return;
        if (bannerView != null)
        {
            bannerView.Show();
            loadingAttemp++;

        }
        else
        {
            LoadBanner();
        }
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    // =========================
    // INTERSTITIAL
    // =========================

    public void LoadInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        AdRequest request = new AdRequest();

        InterstitialAd.Load(interstitialAdUnitId, request,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial failed: " + error);
                    return;
                }

                interstitialAd = ad;
            });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            LoadInterstitial(); // Preload next
        }
        else
        {
            Debug.Log("Interstitial not ready");
        }
    }

    // =========================
    // REWARDED
    // =========================

    public void LoadRewarded()
    {
        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, request,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Rewarded failed: " + error);
                    return;
                }

                rewardedAd = ad;
            });
    }

    public void ShowRewarded(Action onRewardGranted)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show(reward =>
            {
                Debug.Log($"Reward: {reward.Amount} {reward.Type}");
                onRewardGranted?.Invoke();
            });

            LoadRewarded(); // Preload next
        }
        else
        {
            Debug.Log("Rewarded ad not ready");
        }
    }

    // =========================
    // CLEANUP
    // =========================

    private void OnDestroy()
    {
        bannerView?.Destroy();
        interstitialAd?.Destroy();
    }
}
