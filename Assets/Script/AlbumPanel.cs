using UnityEngine;
using UnityEngine.UI;

public class AlbumPanel : MonoBehaviour
{
    public static AlbumPanel Instance;

    public GameObject panel;
    public Button closeButton;
    public Transform contentParent; // Content � ScrollView
    public GameObject cardPrefab;   // ������ �������� (�������� ����)

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        panel.SetActive(true);
        RefreshCards();

        if (LevelPlayManager.Instance != null)
        {
            LevelPlayManager.Instance.ShowInterstitial();
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void RefreshCards()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"RefreshCards. AlbumManager.Instance = {AlbumManager.Instance != null}");

        for (int level = 2; level <= 10; level++)
        {
            bool unlocked = AlbumManager.Instance.IsUnlocked(level);
            Debug.Log($"  ������� {level}: {unlocked}");

            GameObject card = Instantiate(cardPrefab, contentParent);
            AlbumCard cardScript = card.GetComponent<AlbumCard>();
            cardScript.Setup(level, unlocked);
        }
    }
}
