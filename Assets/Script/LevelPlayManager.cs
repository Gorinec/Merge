using UnityEngine;
using System;
using Unity.Services.LevelPlay;

public class LevelPlayManager : MonoBehaviour
{
    public static LevelPlayManager Instance;

    [Header("LevelPlay Settings")]
    public string appKey = "YOUR_APP_KEY"; // Replace in Inspector
    public string rewardedAdUnitId = "YOUR_REWARDED_AD_UNIT_ID"; // Replace in Inspector
    public string interstitialAdUnitId = "YOUR_INTERSTITIAL_AD_UNIT_ID"; // Replace in Inspector

    [Header("Logic Settings")]
    public float interstitialCooldown = 120f;
    private float _lastInterstitialTime = -120f;

    private LevelPlayRewardedAd _rewardedAd;
    private LevelPlayInterstitialAd _interstitialAd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize LevelPlay SDK
        LevelPlay.Init(appKey);
        LevelPlay.OnInitSuccess += OnLevelPlayInitialized;
    }

    private void OnLevelPlayInitialized(LevelPlayConfiguration config)
    {
        Debug.Log("LevelPlay SDK Initialized");
        
        // Create Ad Objects
        _rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);
        _interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

        // Subscribe to Events
        _rewardedAd.OnAdRewarded += OnAdRewarded;
        
        // Auto-reload ads when closed
        _rewardedAd.OnAdClosed += (adInfo) => _rewardedAd.LoadAd();
        _interstitialAd.OnAdClosed += (adInfo) => _interstitialAd.LoadAd();

        // Initial Load
        _rewardedAd.LoadAd();
        _interstitialAd.LoadAd();
    }

    public void ShowRewarded()
    {
        if (_rewardedAd != null && _rewardedAd.IsAdReady())
        {
            _rewardedAd.ShowAd("Rewarded_Energy");
        }
        else
        {
            Debug.LogWarning("Rewarded Ad not ready, attempting to load...");
            _rewardedAd?.LoadAd();
        }
    }

    public void ShowInterstitial()
    {
        if (Time.time - _lastInterstitialTime < interstitialCooldown)
        {
            Debug.Log("Interstitial is on cooldown");
            return;
        }

        if (_interstitialAd != null && _interstitialAd.IsAdReady())
        {
            _interstitialAd.ShowAd("Interstitial_Album");
            _lastInterstitialTime = Time.time;
        }
        else
        {
            Debug.LogWarning("Interstitial not ready, attempting to load...");
            _interstitialAd?.LoadAd();
        }
    }

    private void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddEnergy(5);
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        LevelPlay.SetPauseGame(isPaused);
    }

    private void OnDestroy()
    {
        if (_rewardedAd != null) _rewardedAd.Dispose();
        if (_interstitialAd != null) _interstitialAd.Dispose();
    }
}
