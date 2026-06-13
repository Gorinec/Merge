using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    [Header("Данные предмета")]
    public int level = 1;
    public bool isFinalLevel = false;

    [Header("UI")]
    public Image itemImage;
    public TMP_Text levelText;

    [Header("Спрайты")]
    public Sprite[] levelSprites; // Массив из 10 спрайтов (индекс 0 = уровень 1)

    [Header("Альбом")]
    public GameObject albumButton;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (itemImage == null)
            itemImage = GetComponent<Image>();

        if (levelText == null)
            levelText = GetComponentInChildren<TMP_Text>();
    }

    public void Setup(int itemLevel)
    {
        level = itemLevel;

        // Текст уровня
        if (levelText != null)
            levelText.text = level.ToString();

        // Спрайт предмета
        if (itemImage != null && levelSprites != null && level <= levelSprites.Length)
        {
            itemImage.sprite = levelSprites[level - 1];
            itemImage.color = Color.white; // Убираем старый цвет
        }

        isFinalLevel = (level >= 10);

        // Размер увеличивается с уровнем
        float scale = 1f + (level - 1) * 0.1f;
        rectTransform.sizeDelta = new Vector2(80 * scale, 80 * scale);
    }

    public string GetItemName()
    {
        string[] names = new string[]
        {
            "Клочок ткани",
            "Лоскут",
            "Выкройка лапы",
            "Лапа с когтями",
            "Выкройка головы",
            "Голова с ушами",
            "Голова с глазами",
            "Туловище",
            "Почти целый медведь",
            "Плюшевый Медведь"
        };

        if (level >= 1 && level <= names.Length)
            return names[level - 1];
        return $"Предмет {level}";
    }

    public void ShowAlbumButton(bool show)
    {
        if (albumButton != null)
            albumButton.SetActive(show);
    }

    public bool CanMergeWith(Item other)
    {
        return other != null
            && other != this
            && other.level == this.level
            && !this.isFinalLevel
            && !other.isFinalLevel;
    }
}