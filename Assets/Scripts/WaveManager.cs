using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Düþman Havuzu")]
    [Tooltip("Element 0: Yarasa, Element 1: Okçu, Element 2: Dash Tazý, Element 3: Tank")]
    public GameObject[] enemyPrefabs;

    [Header("Boss Ayarlarý")]
    [Tooltip("10 ve 20. dalgalarda çýkacak Mini-Boss prefabý")]
    public GameObject miniBossPrefab;
    [Tooltip("30. dalgada çýkacak Final Boss prefabý")]
    public GameObject finalBossPrefab;

    [Header("Boss UI Baðlantýlarý")]
    public GameObject bossBarPanel;
    public Image bossHealthFill;
    public TMP_Text bossNameText;

    public Transform player;
    public float spawnRadius = 5.2f;
    public int maxWave = 30;

    [Header("UI Baðlantýlarý")]
    public TMP_Text waveText;
    public TMP_Text enemyCountText;
    public TMP_Text waveNoticeText;

    [Header("Dalga Sonu & Maðaza Panelleri")]
    public GameObject shopPanel;
    public GameObject victoryPanel;

    private int currentWave = 0;
    private int totalEnemiesToSpawn;
    private int enemiesRemaining;
    private bool isWaveActive = false;
    private int spawnedTanksInWave = 0;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (shopPanel != null) shopPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (bossBarPanel != null) bossBarPanel.SetActive(false);

        StartCoroutine(StartNextWaveRoutine());
    }

    IEnumerator StartNextWaveRoutine()
    {
        isWaveActive = false;
        currentWave++;
        spawnedTanksInWave = 0;

        bool isMiniBossWave = (currentWave == 10 || currentWave == 20);
        bool isFinalBossWave = (currentWave >= maxWave);
        bool isBossWave = isMiniBossWave || isFinalBossWave;

        if (isBossWave)
        {
            totalEnemiesToSpawn = 1;
            enemiesRemaining = 1;
        }
        else
        {
            totalEnemiesToSpawn = 3 + (currentWave * 2);
            enemiesRemaining = totalEnemiesToSpawn;
        }

        if (waveText != null)
            waveText.text = "DALGA: " + currentWave + "/" + maxWave;

        UpdateEnemyCountUI();

        // Dalga Baþlangýç Bildirimi
        if (waveNoticeText != null)
        {
            waveNoticeText.gameObject.SetActive(true);

            if (isFinalBossWave)
            {
                waveNoticeText.text = "BÜYÜK TEHLÝKE: SON BOSS GELDÝ!";
                waveNoticeText.color = new Color(0.8f, 0.1f, 1f); // Mor uyarý
            }
            else if (isMiniBossWave)
            {
                waveNoticeText.text = "BÖLÜM BOSS'U GELDÝ!";
                waveNoticeText.color = Color.red;
            }
            else
            {
                waveNoticeText.text = "DALGA " + currentWave;
                waveNoticeText.color = Color.white;
            }

            yield return new WaitForSeconds(2f);
            waveNoticeText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        isWaveActive = true;

        if (isBossWave)
        {
            SpawnBoss(isFinalBossWave);
        }
        else
        {
            StartCoroutine(SpawnEnemiesOverTime());
        }
    }

    IEnumerator SpawnEnemiesOverTime()
    {
        for (int i = 0; i < totalEnemiesToSpawn; i++)
        {
            SpawnSingleEnemy(i);
            yield return new WaitForSeconds(0.6f);
        }
    }

    void SpawnBoss(bool isFinalBoss)
    {
        if (player == null) return;

        Vector2 randomCircle = Random.insideUnitCircle.normalized * (spawnRadius + 2f);
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, randomCircle.y, 0);

        if (isFinalBoss && finalBossPrefab != null)
        {
            GameObject fBoss = Instantiate(finalBossPrefab, spawnPos, Quaternion.identity);
            FinalBossAI fScript = fBoss.GetComponent<FinalBossAI>();
            if (fScript != null)
            {
                fScript.bossBarPanel = bossBarPanel;
                fScript.bossHealthFill = bossHealthFill;
                fScript.bossNameText = bossNameText;
                fScript.InitHealthUI();
            }
        }
        else if (miniBossPrefab != null)
        {
            GameObject mBoss = Instantiate(miniBossPrefab, spawnPos, Quaternion.identity);
            MiniBossAI mScript = mBoss.GetComponent<MiniBossAI>();
            if (mScript != null)
            {
                mScript.bossBarPanel = bossBarPanel;
                mScript.bossHealthFill = bossHealthFill;
                mScript.bossNameText = bossNameText;
                mScript.InitHealthUI();
            }
        }
    }

    void SpawnSingleEnemy(int spawnIndex)
    {
        if (player == null || enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, randomCircle.y, 0);

        int selectedIndex = 0;
        int maxTankLimit = (currentWave >= 6) ? 2 : 1;
        bool tankHakkiVar = (currentWave >= 3 && spawnedTanksInWave < maxTankLimit && enemyPrefabs.Length > 3);

        if (currentWave < 3)
        {
            selectedIndex = (Random.value < 0.3f && enemyPrefabs.Length > 2) ? 2 : 0;
        }
        else
        {
            if (tankHakkiVar && (spawnIndex == 0 || Random.Range(0, 100) < 15))
            {
                selectedIndex = 3;
                spawnedTanksInWave++;
            }
            else
            {
                int chance = Random.Range(0, 100);

                if (chance < 10 && enemyPrefabs.Length > 1 && currentWave >= 4)
                    selectedIndex = 1;
                else if (chance < 45 && enemyPrefabs.Length > 2)
                    selectedIndex = 2;
                else
                    selectedIndex = 0;
            }
        }

        if (enemyPrefabs[selectedIndex] != null)
        {
            Instantiate(enemyPrefabs[selectedIndex], spawnPos, Quaternion.identity);
        }
    }

    public void EnemyDied()
    {
        enemiesRemaining--;
        if (enemiesRemaining < 0) enemiesRemaining = 0;

        UpdateEnemyCountUI();

        if (enemiesRemaining == 0 && isWaveActive)
        {
            BitirDalgayi();
        }
    }

    void Update()
    {
        if (isWaveActive && enemiesRemaining <= 2)
        {
            GameObject[] canliDusmanlar = GameObject.FindGameObjectsWithTag("Enemy");
            if (canliDusmanlar.Length == 0)
            {
                BitirDalgayi();
            }
        }
    }

    void BitirDalgayi()
    {
        isWaveActive = false;
        enemiesRemaining = 0;
        UpdateEnemyCountUI();
        StopAllCoroutines();

        if (bossBarPanel != null) bossBarPanel.SetActive(false);

        OnWaveCompleted();
    }

    void OnWaveCompleted()
    {
        if (currentWave >= maxWave)
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }
        else
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                StartCoroutine(StartNextWaveRoutine());
            }
        }
    }

    public void SonrakiDalgayaGec()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        Time.timeScale = 1f;
        StartCoroutine(StartNextWaveRoutine());
    }

    void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
            enemyCountText.text = "DÜÞMAN SAYISI: " + enemiesRemaining;
    }
}