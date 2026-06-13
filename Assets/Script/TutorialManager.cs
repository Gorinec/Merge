using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public Image handPointer;
    public Button spawnButton;
    public Transform itemsContainer;

    private void Awake()
    {
        Instance = this;
        tutorialPanel.SetActive(false);
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("TutorialDone"))
        {
            StartCoroutine(TutorialRoutine());
        }
    }

    private IEnumerator TutorialRoutine()
    {
        tutorialPanel.SetActive(true);

        // Шаг 1: Создать 4 предмета
        tutorialText.text = "Нажми кнопку 4 раза, чтобы создать предметы";
        handPointer.gameObject.SetActive(true);
        
        yield return new WaitUntil(() => itemsContainer.childCount >= 4);
        yield return new WaitForSeconds(0.3f);

        // Шаг 2: Перетаскивание
        tutorialText.text = "Перетащи один предмет на другой";
        handPointer.gameObject.SetActive(false);
        yield return new WaitUntil(() =>
        {
            Item[] items = FindObjectsByType<Item>(FindObjectsSortMode.None);
            foreach (Item item in items)
            {
                if (item.level == 2) return true;
            }
            return false;
        });
        yield return new WaitForSeconds(0.3f);

        // Шаг 3: Добавить в альбом
        tutorialText.text = "Нажми + на предмете, чтобы добавить в альбом";
        yield return new WaitUntil(() => AlbumManager.Instance.IsUnlocked(2));
        yield return new WaitForSeconds(0.3f);

        // Шаг 4: Открыть альбом
        tutorialText.text = "Открой альбом и посмотри историю";
        yield return new WaitForSeconds(3f);

        // Шаг 5: Провести линию
        tutorialText.text = "Зажми предмет и проведи линию к такому же";
        yield return new WaitUntil(() =>
        {
            Item[] items = FindObjectsByType<Item>(FindObjectsSortMode.None);
            foreach (Item item in items)
            {
                if (item.level >= 2) return true;
            }
            return false;
        });
        yield return new WaitForSeconds(0.3f);

        // Шаг 6: Финал
        tutorialText.text = "Отлично! Собирай все части медведя и узнай его историю!";
        yield return new WaitForSeconds(3f);

        tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt("TutorialDone", 1);
    }
}
