using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("UI Elemanlarý")]
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Toggle vsyncToggle;

    // Diðer script'lerden eriþebilmek için statik fare hýzý deðiþkeni
    public static float mouseSensitivity = 1f;

    void Start()
    {
        // 1. Tam Ekran Ayarýný Yükle
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = Screen.fullScreen;

        // 2. Ses Ayarýný Yükle
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
        }

        // 3. Fare Hýzý Ayarýný Yükle (Varsayýlan 1)
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.1f;
            sensitivitySlider.maxValue = 5f;
            sensitivitySlider.value = mouseSensitivity;
        }

        // 4. V-Sync Ayarýný Yükle
        if (vsyncToggle != null)
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
    }

    // --- BUTON FONKSÝYONLARI ---

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // --- AYAR DEÐÝÞÝM FONKSÝYONLARI ---

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
    }
}