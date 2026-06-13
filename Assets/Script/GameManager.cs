using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Энергия")]
    public int maxEnergy = 20;
    public int currentEnergy;
    public float energyRestoreInterval = 60f;
    public TMP_Text energyText;
    public TMP_Text energyTimerText;

    [Header("Лимит предметов")]
    public int maxItemsOnField = 12;

    public ItemSpawner itemSpawner;

    private float energyTimer;

    private void Awake()
    {
        Instance = this;
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            energyTimer += Time.deltaTime;
            if (energyTimer >= energyRestoreInterval)
            {
                energyTimer = 0f;
                currentEnergy++;
                UpdateEnergyUI();
            }

            if (energyTimerText != null)
            {
                int secondsLeft = Mathf.CeilToInt(energyRestoreInterval - energyTimer);
                int minutes = secondsLeft / 60;
                int seconds = secondsLeft % 60;
                energyTimerText.text = $"Энергия через {minutes}:{seconds:D2}";
            }
        }
        else
        {
            if (energyTimerText != null)
                energyTimerText.text = "Полная";
        }
    }

    public bool CanSpawn()
    {
        return currentEnergy > 0 && GetCurrentItemCount() < maxItemsOnField;
    }

    public void SpendEnergy(int amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0) currentEnergy = 0;
        UpdateEnergyUI();
    }

    public int GetCurrentItemCount()
    {
        return GameObject.FindGameObjectsWithTag("Item").Length;
    }

    public void UpdateEnergyUI()
    {
        if (energyText != null)
            energyText.text = $"Энергия: {currentEnergy}";
    }

    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        UpdateEnergyUI();
    }
}