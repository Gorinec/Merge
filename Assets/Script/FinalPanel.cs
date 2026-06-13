using UnityEngine;
using UnityEngine.UI;

public class FinalPanel : MonoBehaviour
{
    public static FinalPanel Instance;

    public GameObject panel;
    public Button closeButton;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}