using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Paneller")]
    [Tooltip("Oyun içi duraklatma paneli (Sadece Oyun Sahnesinde Kullanılır)")]
    public GameObject pausePanel;

    [Tooltip("Ayarlar paneli")]
    public GameObject settingsPanel;

    [Header("Sahne Ayarları")]
    [Tooltip("Başla butonuna basılınca açılacak sahnenin tam adı")]
    public string levelName = "Level1";

    private bool isPaused = false;

    void Update()
    {
        // ESC tuşu kontrolü
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 1. Eğer Ayarlar Paneli açıksa, ESC'ye basınca önce Ayarları kapat ve Pause Paneline dön!
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);

                if (pausePanel != null)
                {
                    pausePanel.SetActive(true);
                }
            }
            // 2. Ayarlar kapalıysa normal Pause/Resume mantığı çalışsın
            else if (pausePanel != null)
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
    }

    // --- ANA MENÜ BUTONLARI ---

    // 1. BAŞLA (Play) Butonu İçin
    public void PlayGame()
    {
        Time.timeScale = 1f; // Zamanın normal aktığından emin oluyoruz
        SceneManager.LoadScene(levelName);
    }

    // 2. AYARLAR (Options) Butonu İçin (Ana menüdeki veya Pause menüsündeki buton için)
    public void OpenOptions()
    {
        if (settingsPanel != null)
        {
            if (pausePanel != null) pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Ayarlar paneli atanmamış!");
        }
    }

    // 3. ÇIKIŞ (Quit) Butonu İçin
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkıldı.");

#if UNITY_EDITOR
        // Unity Editör'de test ederken Play modunu durdurur
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Gerçek derlenmiş oyunda (.exe / apk vb.) oyunu tamamen kapatır
        Application.Quit();
#endif
    }

    // --- OYUN İÇİ (PAUSE) BUTONLARI ---

    // Oyun İçi: Oyuna Devam Et Butonu
    public void Resume()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false); // Ayarlar açıksa onu da kapat

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    // Oyun İçi: Oyunu Duraklat
    public void Pause()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    // Ayarlar Panelini Kapatıp Pause Paneline Dönme Fonksiyonu
    public void CloseOptions()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    // Oyun İçi: Duraklatma Menüsünden Ana Menüye Dönüş Butonu
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Zaman akışını normale döndür
        SceneManager.LoadScene(0); // Build Settings'teki 0 numaralı (Ana Menü) sahneye gider
    }
}