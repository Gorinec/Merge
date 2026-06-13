using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    public static SettingsPanel Instance;

    public GameObject panel;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button closeButton;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        closeButton.onClick.AddListener(Hide);

        // Загружаем сохранённые настройки
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
    }

    private void Start()
    {
        // Применяем при старте
        ApplyVolumes();
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void OnMusicChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        ApplyVolumes();
    }

    private void OnSFXChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
            AudioManager.Instance.SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.7f));
        }
    }
}
