using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    [Header("Префаб предмета")]
    public GameObject itemPrefab;

    [Header("Контейнер для предметов")]
    public RectTransform itemsContainer;

    [Header("Кнопка создания")]
    public Button spawnButton;

    private void Start()
    {
        spawnButton.onClick.AddListener(SpawnItem);
    }

    private void Update()
    {
        spawnButton.interactable = GameManager.Instance.CanSpawn();
    }

    public void SpawnItem()
    {
        if (!GameManager.Instance.CanSpawn())
            return;

        GameManager.Instance.SpendEnergy(1);

        RectTransform itemRect = itemPrefab.GetComponent<RectTransform>();
        float itemWidth = itemRect.sizeDelta.x;
        float itemHeight = itemRect.sizeDelta.y;

        float containerWidth = itemsContainer.rect.width;
        float containerHeight = itemsContainer.rect.height;

        Vector2 randomPos;
        bool positionFound = false;
        int attempts = 0;
        int maxAttempts = 50;

        while (!positionFound && attempts < maxAttempts)
        {
            attempts++;

            float randomX = Random.Range(itemWidth / 2, containerWidth - itemWidth / 2);
            float randomY = Random.Range(itemHeight / 2, containerHeight - itemHeight / 2);

            randomPos = new Vector2(
                randomX - containerWidth / 2,
                randomY - containerHeight / 2
            );

            // Проверяем, не занята ли позиция другим предметом
            if (!IsPositionOccupied(randomPos, itemWidth))
            {
                positionFound = true;

                GameObject newItem = Instantiate(itemPrefab, itemsContainer);
                newItem.tag = "Item";

                RectTransform newRect = newItem.GetComponent<RectTransform>();
                newRect.anchorMin = new Vector2(0.5f, 0.5f);
                newRect.anchorMax = new Vector2(0.5f, 0.5f);
                newRect.pivot = new Vector2(0.5f, 0.5f);
                newRect.anchoredPosition = randomPos;

                Item itemScript = newItem.GetComponent<Item>();
                itemScript.Setup(1);
            }
        }

        if (!positionFound)
        {
            // Если за 50 попыток не нашли место — поле забито
            Debug.LogWarning("Нет свободного места для предмета!");
            GameManager.Instance.currentEnergy++; // Возвращаем энергию
            GameManager.Instance.UpdateEnergyUI();
        }
    }

    // Новый метод проверки занятости
    private bool IsPositionOccupied(Vector2 position, float itemSize)
    {
        Item[] allItems = itemsContainer.GetComponentsInChildren<Item>();
        float minDistance = itemSize * 0.8f; // Минимальное расстояние между центрами

        foreach (Item item in allItems)
        {
            RectTransform itemRect = item.GetComponent<RectTransform>();
            float dist = Vector2.Distance(position, itemRect.anchoredPosition);
            if (dist < minDistance)
                return true;
        }
        return false;
    }
}