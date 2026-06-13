using UnityEngine;
using UnityEngine.UI;

public class AdEnergyButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ShowAd);
    }

    private void ShowAd()
    {
        AdsManager.Instance.ShowRewarded();
    }
}