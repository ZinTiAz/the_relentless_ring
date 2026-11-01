using UnityEngine;
using TMPro; // TextMeshPro kullandýðýn için gerekli

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Baðlantýsý")]
    public TMP_Text killText;
    public TMP_Text enemyCountText; // ANLIK DÜÞMAN SAYISI ÝÇÝN EKLENDÝ

    private int killCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateKillUI();
    }

    private void Update()
    {
        // Her karede sahnede kaç tane düþman olduðunu sayýp UI'a yazar
        UpdateEnemyCountUI();
    }

    // Düþman öldüðünde bu fonksiyonu çaðýracaðýz
    public void AddKill()
    {
        killCount++;
        UpdateKillUI();
    }

    private void UpdateKillUI()
    {
        if (killText != null)
        {
            killText.text = "Öldürme Sayýsý : " + killCount;
        }
    }

    private void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            // Tag'i "Enemy" olan düþmanlarýn anlýk sayýsýný alýr
            int canlýDusmanSayisi = GameObject.FindGameObjectsWithTag("Enemy").Length;
            enemyCountText.text = "Düþman Sayýsý : " + canlýDusmanSayisi;
        }
    }
}