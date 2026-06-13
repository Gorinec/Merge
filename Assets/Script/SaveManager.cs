using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadGame();
    }

    // ===== СОХРАНЕНИЕ =====

    public void SaveGame()
    {
        // Альбом
        List<int> unlocked = new List<int>();
        for (int i = 2; i <= 10; i++)
        {
            if (AlbumManager.Instance.IsUnlocked(i))
                unlocked.Add(i);
        }
        PlayerPrefs.SetString("Album", string.Join(",", unlocked));

        // Энергия
        PlayerPrefs.SetInt("Energy", GameManager.Instance.currentEnergy);

        // Предметы на поле
        Item[] items = FindObjectsByType<Item>(FindObjectsSortMode.None);
        List<string> itemData = new List<string>();
        foreach (Item item in items)
        {
            RectTransform rt = item.GetComponent<RectTransform>();
            string data = $"{item.level}|{rt.anchoredPosition.x}|{rt.anchoredPosition.y}";
            itemData.Add(data);
        }
        PlayerPrefs.SetString("Items", string.Join(";", itemData));

        // Время
        PlayerPrefs.SetString("LastTime", System.DateTime.Now.ToString());

        PlayerPrefs.Save();
        Debug.Log($"Сохранено: {items.Length} предметов");
    }

    // ===== ЗАГРУЗКА =====

    public void LoadGame()
    {
        // Альбом
        string albumData = PlayerPrefs.GetString("Album", "");
        if (!string.IsNullOrEmpty(albumData))
        {
            string[] levels = albumData.Split(',');
            foreach (string s in levels)
            {
                int level;
                if (int.TryParse(s, out level))
                {
                    AlbumManager.Instance.UnlockLevel(level);
                }
            }
        }

        // Энергия
        if (PlayerPrefs.HasKey("Energy"))
        {
            GameManager.Instance.currentEnergy = PlayerPrefs.GetInt("Energy");
            GameManager.Instance.UpdateEnergyUI();
        }

        // Начисление энергии за отсутствие
        if (PlayerPrefs.HasKey("LastTime"))
        {
            System.DateTime lastTime = System.DateTime.Parse(PlayerPrefs.GetString("LastTime"));
            System.TimeSpan passed = System.DateTime.Now - lastTime;
            int energyToAdd = Mathf.FloorToInt((float)passed.TotalSeconds / GameManager.Instance.energyRestoreInterval);
            if (energyToAdd > 0)
            {
                GameManager.Instance.AddEnergy(energyToAdd);
            }
        }

        // Предметы на поле
        string itemsData = PlayerPrefs.GetString("Items", "");
        if (!string.IsNullOrEmpty(itemsData))
        {
            string[] items = itemsData.Split(';');
            foreach (string data in items)
            {
                if (string.IsNullOrEmpty(data)) continue;
                string[] parts = data.Split('|');
                if (parts.Length == 3)
                {
                    int level = int.Parse(parts[0]);
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);

                    GameObject newItem = Instantiate(
                        GameManager.Instance.itemSpawner.itemPrefab,
                        GameManager.Instance.itemSpawner.itemsContainer
                    );
                    newItem.tag = "Item";

                    RectTransform rt = newItem.GetComponent<RectTransform>();
                    rt.anchorMin = new Vector2(0.5f, 0.5f);
                    rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = new Vector2(x, y);

                    Item item = newItem.GetComponent<Item>();
                    item.Setup(level);
                }
            }
        }

        Debug.Log("Игра загружена");
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}