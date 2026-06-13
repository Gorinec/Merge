using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenSettings);
    }

    private void OpenSettings()
    {
        Debug.Log("Кнопка настроек нажата");
        if (SettingsPanel.Instance != null)
        {
            SettingsPanel.Instance.Show();
        }
        else
        {
            Debug.LogError("SettingsPanel.Instance = null! Нет SettingsPanel на сцене.");
        }
    }
}