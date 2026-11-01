using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FinalBossAI : MonoBehaviour, IDamageable
{
    [Header("Geçici Ana Boss Deðerleri")]
    public float maxHealth = 3500f;
    public float health;
    public float walkSpeed = 1.3f;
    public float contactDamage = 15f;
    public float contactCooldown = 1f;

    [Header("Mermi Saldýrýsý")]
    public GameObject bossBulletPrefab;
    public float attackCooldown = 3.5f;
    public float bulletSpeed = 6f;
    public int bulletCount = 8;

    [Header("Ödül")]
    public int dropGold = 100;
    public int dropXpCount = 8; // Boss ölünce bol XP fýrlasýn

    [Header("Efektler & Prefablar (Enemy ile Ayný)")]
    public GameObject vurusEfektiPrefab;
    public GameObject xpKristaliPrefab;
    public GameObject hasarYazisiPrefab;
    public GameObject[] kanBirikintisiPrefablari;

    [HideInInspector] public GameObject bossBarPanel;
    [HideInInspector] public Image bossHealthFill;
    [HideInInspector] public TMP_Text bossNameText;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private float nextAttackTime;
    private float nextContactTime;
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        health = maxHealth;
        nextAttackTime = Time.time + 2f;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    public void InitHealthUI()
    {
        if (bossBarPanel != null)
        {
            bossBarPanel.SetActive(true);
            if (bossNameText != null) bossNameText.text = "KARA EFENDÝ (FINAL BOSS)";
            if (bossHealthFill != null)
            {
                bossHealthFill.color = new Color(0.6f, 0.1f, 0.9f); // Mor can barý
                bossHealthFill.fillAmount = 1f;
            }
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Baktýðý yönü çevir
        float dirX = player.position.x - transform.position.x;
        if (sr != null && Mathf.Abs(dirX) > 0.05f) sr.flipX = dirX < 0;

        // Saldýrý Zamaný
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(ShootNovaRoutine());
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null) return;

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * walkSpeed;
    }

    IEnumerator ShootNovaRoutine()
    {
        rb.linearVelocity = Vector2.zero;
        if (sr != null) sr.color = Color.magenta;
        yield return new WaitForSeconds(0.6f);
        if (sr != null) sr.color = Color.white;

        if (bossBulletPrefab != null)
        {
            float angleStep = 360f / bulletCount;
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                GameObject b = Instantiate(bossBulletPrefab, transform.position, Quaternion.identity);

                BossBullet bulletScript = b.GetComponent<BossBullet>();
                if (bulletScript == null) b.AddComponent<BossBullet>();

                Rigidbody2D brb = b.GetComponent<Rigidbody2D>();
                if (brb != null) brb.linearVelocity = dir * bulletSpeed;

                Destroy(b, 5f);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector2.zero);
    }

    public void TakeDamage(float amount, Vector2 knockbackDir)
    {
        if (isDead) return;

        health -= amount;

        // Can barýný güncelle
        if (bossHealthFill != null)
            bossHealthFill.fillAmount = Mathf.Clamp01(health / maxHealth);

        // 1. Hasar Yazýsý Efekti
        if (hasarYazisiPrefab != null)
        {
            Vector3 yaziPozisyonu = transform.position + new Vector3(0f, -1.8f, 0f);
            GameObject yaziObj = Instantiate(hasarYazisiPrefab, yaziPozisyonu, Quaternion.identity);
            FloatingText ft = yaziObj.GetComponent<FloatingText>();
            if (ft != null) ft.Kurulum((int)amount);
        }

        // 2. Vuruþ Partikülü
        if (vurusEfektiPrefab != null)
        {
            Instantiate(vurusEfektiPrefab, transform.position, Quaternion.identity);
        }

        // 3. Kýrmýzý Yanýp Sönme (Flash)
        StartCoroutine(FlashRed());

        if (health <= 0) Die();
    }

    IEnumerator FlashRed()
    {
        if (sr == null) yield break;
        Color originalColor = Color.white;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sr != null && !isDead) sr.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (bossBarPanel != null) bossBarPanel.SetActive(false);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        if (col != null) col.enabled = false;

        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null) wm.EnemyDied();

        if (GameManager.Instance != null) GameManager.Instance.AddKill();

        // Altýn Ödülü
        if (player != null)
        {
            PlayerMovement pScript = player.GetComponent<PlayerMovement>();
            if (pScript != null) pScript.AddGold(dropGold);
        }

        // Kan Birikintisi Efektleri
        if (kanBirikintisiPrefablari != null && kanBirikintisiPrefablari.Length > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                int r = Random.Range(0, kanBirikintisiPrefablari.Length);
                Instantiate(kanBirikintisiPrefablari[r], transform.position + (Vector3)Random.insideUnitCircle * 0.7f, Quaternion.identity);
            }
        }

        // Bolca XP Patlamasý
        if (xpKristaliPrefab != null)
        {
            for (int i = 0; i < dropXpCount; i++)
            {
                Vector3 ofs = (Vector3)Random.insideUnitCircle * 0.8f;
                Instantiate(xpKristaliPrefab, transform.position + ofs, Quaternion.identity);
            }
        }

        this.enabled = false;
        Destroy(gameObject, 0.8f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player") && Time.time >= nextContactTime)
        {
            nextContactTime = Time.time + contactCooldown;
            PlayerMovement p = collision.gameObject.GetComponent<PlayerMovement>();
            if (p != null) p.TakeDamage((int)contactDamage);
        }
    }
}