using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("UI")]
    public GameObject storyPopup;
    public TMP_Text storyText;
    public TMP_Text storyTitle;
    public Button closeButton;

    private string[] storyParts = new string[]
    {
        "В детстве у Маши был любимый плюшевый медведь. Она не расставалась с ним ни на минуту...",
        "Однажды медведь порвался. Маша спрятала его в шкаф и горько плакала.",
        "Шли годы. Маша выросла, но иногда с улыбкой вспоминала своего друга.",
        "Она решила найти все части медведя и сшить его заново — как в детстве.",
        "Поиски начались со старого шкафа. Там, под вязаными свитерами, нашлась мягкая лапка.",
        "На чердаке, среди пыльных коробок с игрушками, Маша нашла голову медведя с ушами.",
        "В подвале стоял старый сундук. В нём, завёрнутое в детское одеяло, лежало туловище.",
        "Осталась последняя деталь. Машины руки дрожали — она почти собрала друга.",
        "Медведь снова цел. Маша прижала его к себе и улыбнулась. Совсем как в детстве."
    };

    [Header("Финал")]
    public GameObject finalPanel;
    public TMP_Text finalText;

    public void ShowFinal()
    {
        finalPanel.SetActive(true);
        finalText.text = "Медведь снова цел.\nМаша прижала его к себе и улыбнулась.\n\nСовсем как в детстве.\n\nКОНЕЦ";
    }



    private void Awake()
    {
        Instance = this;
        storyPopup.SetActive(false);
        closeButton.onClick.AddListener(HideStory);
    }

    public void ShowStory(int level)
    {
        int index = level - 2;
        if (index < 0 || index >= storyParts.Length) return;

        storyPopup.SetActive(true);
        storyTitle.text = $"Часть {index + 1} из 9";
        storyText.text = storyParts[index];
    }

    public void HideStory()
    {
        storyPopup.SetActive(false);
    }

    public bool IsLastPart(int level)
    {
        return level >= 10;
    }
}