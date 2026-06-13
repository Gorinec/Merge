using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlbumCard : MonoBehaviour
{
    public Image background;
    public Image itemIcon;
    public TMP_Text levelText;
    public Button cardButton;
    public GameObject lockOverlay;

    [Header("Спрайты предметов (уровни 2-10)")]
    public Sprite[] levelSprites;

    private int level;
    private bool unlocked;

    public void Setup(int lvl, bool isUnlocked)
    {
        level = lvl;
        unlocked = isUnlocked;

        levelText.text = $"Ур. {level}";
        cardButton.onClick.RemoveAllListeners();

        cardButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayAlbumOpen();
            StoryManager.Instance.ShowStory(level);
            AlbumPanel.Instance.Hide();
        });

        if (unlocked)
        {
            background.color = new Color(0.3f, 0.2f, 0.1f);

            // Спрайт предмета
            if (levelSprites != null && level - 2 >= 0 && level - 2 < levelSprites.Length)
            {
                itemIcon.sprite = levelSprites[level - 2];
                itemIcon.color = Color.white;
            }

            itemIcon.gameObject.SetActive(true);
            if (lockOverlay != null) lockOverlay.SetActive(false);
            cardButton.interactable = true;

            cardButton.onClick.AddListener(() =>
            {
                StoryManager.Instance.ShowStory(level);
                AlbumPanel.Instance.Hide();
            });
        }
        else
        {
            background.color = new Color(0.15f, 0.15f, 0.15f);
            itemIcon.gameObject.SetActive(false);
            if (lockOverlay != null) lockOverlay.SetActive(true);
            cardButton.interactable = false;
        }
    }
}