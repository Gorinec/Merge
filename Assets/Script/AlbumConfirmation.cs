using UnityEngine;
using UnityEngine.UI;

public class AlbumConfirmation : MonoBehaviour
{
    public static AlbumConfirmation Instance;

    public GameObject panel;
    public Button yesButton;
    public Button noButton;

    private Item currentItem;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        yesButton.onClick.AddListener(OnYes);
        noButton.onClick.AddListener(OnNo);
    }

    public void Show(Item item)
    {
        currentItem = item;
        panel.SetActive(true);
    }

    private void OnYes()
    {
        panel.SetActive(false);
        if (currentItem != null)
        {
            AlbumManager.Instance.AddItem(currentItem.level);
            Destroy(currentItem.gameObject); // Вот эта строка должна быть
        }
        AudioManager.Instance.PlayAlbumAdd();
    }

    private void OnNo()
    {
        panel.SetActive(false);
        currentItem = null;
    }
}