using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager Instance;

    [Header("Unity Ads")]
    private string gameId = "6120822";
    private string rewardedAdUnitId = "Rewarded_Android";
    private string interstitialAdUnitId = "Interstitial_Android";

    private bool rewardedLoaded = false;
    private bool interstitialLoaded = false;
    private float lastInterstitialTime = -999f;
    private float interstitialCooldown = 60f;

    private void Awake()
    {
        Instance = this;
        Advertisement.Initialize(gameId, false, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Ads инициализированы");
        LoadRewarded();
        LoadInterstitial();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Ads ошибка: {error} - {message}");
    }

    // ===== REWARDED (+5 энергии) =====

    private void LoadRewarded()
    {
        Advertisement.Load(rewardedAdUnitId, this);
    }

    public void ShowRewarded()
    {
        if (rewardedLoaded)
        {
            Advertisement.Show(rewardedAdUnitId, this);
        }
        else
        {
            Debug.Log("Rewarded не готов");
        }
    }

    // ===== INTERSTITIAL (альбом, раз в 2 минуты) =====

    private void LoadInterstitial()
    {
        Advertisement.Load(interstitialAdUnitId, this);
    }

    public void ShowInterstitial()
    {
        if (Time.time - lastInterstitialTime < interstitialCooldown)
        {
            Debug.Log("Interstitial кулдаун");
            return;
        }

        if (interstitialLoaded)
        {
            Advertisement.Show(interstitialAdUnitId, this);
            lastInterstitialTime = Time.time;
        }
        else
        {
            Debug.Log("Interstitial не готов");
        }
    }

    // ===== CALLBACKS =====

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId == rewardedAdUnitId) rewardedLoaded = true;
        if (adUnitId == interstitialAdUnitId) interstitialLoaded = true;
        Debug.Log($"Ad loaded: {adUnitId}");
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Ad load failed: {adUnitId} - {error} - {message}");
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            if (adUnitId == rewardedAdUnitId)
            {
                GameManager.Instance.AddEnergy(5);
                Debug.Log("+5 энергии");
            }
        }

        if (adUnitId == rewardedAdUnitId) { rewardedLoaded = false; LoadRewarded(); }
        if (adUnitId == interstitialAdUnitId) { interstitialLoaded = false; LoadInterstitial(); }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Ad show failed: {adUnitId} - {error}");
        if (adUnitId == rewardedAdUnitId) { rewardedLoaded = false; LoadRewarded(); }
        if (adUnitId == interstitialAdUnitId) { interstitialLoaded = false; LoadInterstitial(); }
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}