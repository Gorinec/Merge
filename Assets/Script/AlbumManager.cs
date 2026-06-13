using UnityEngine;
using System.Collections.Generic;

public class AlbumManager : MonoBehaviour
{
    public static AlbumManager Instance;

    private HashSet<int> unlockedLevels = new HashSet<int>();

    private void Awake()
    {
        Instance = this;
    }
    public void UnlockLevel(int level)
    {
        if (!unlockedLevels.Contains(level))
            unlockedLevels.Add(level);
    }

    public void AddItem(int level)
    {
        if (unlockedLevels.Contains(level)) return;

        unlockedLevels.Add(level);
        StoryManager.Instance.ShowStory(level);
        if (StoryManager.Instance.IsLastPart(level))
        {
            StoryManager.Instance.ShowFinal();
        }

        if (StoryManager.Instance.IsLastPart(level))
        {
            Debug.Log("ФИНАЛ! Медведь собран!");
            // Потом — финальная сцена
        }
        SaveManager.Instance.SaveGame();
    }

    public bool IsUnlocked(int level)
    {
        return unlockedLevels.Contains(level);
    }
}