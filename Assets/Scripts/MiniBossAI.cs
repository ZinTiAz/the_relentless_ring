using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MiniBossAI : MonoBehaviour, IDamageable
{
    [Header("Temel Boss Deðerleri")]
    public float maxHealth = 1200f;
    public float health;
    public float walkSpeed = 1.6f;
    public float contactDamage = 30f;
    [Range(0f, 1f)]
    public float knockbackResistance = 0.95f;

    [Header("Saldýrý Mekaniði (Atýlma / Dash)")]
    public float dashCooldown = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.45f;
    public float chargePrepareTime = 0.8f;

    [Header("Boss UI (Can Barý)")]
    [HideInInspector] public GameObject bossBarPanel;
    [HideInInspector] public Image bossHealthFill;
    [HideInInspector] public TMP_Text bossNameText;
    public string bossTitle = "GOBILIN BERSERKER";

    [Header("Ödül & Düþme")]
    public int dropGold = 25;
    public int dropXpCount = 5;

    [Header("Efektler & Prefablar")]
    public GameObject vurusEfektiPrefab;
    public GameObject xpKristaliPrefab;
    public GameObject hasarYazisiPrefab;
    public GameObject[] kanBirikintisiPrefablari;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private float nextDashTime;
    private bool isDashing = false;
    private bool isPreparing = false;
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
        nextDashTime = Time.time + 3f;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    // WaveManager'ýn çaðýrdýðý arayüz baþlangýç fonksiyonu
    public void InitHealthUI()
    {
        if (bossBarPanel != null)
        {
            bossBarPanel.SetActive(true);
            if (bossNameText != null) bossNameText.text = bossTitle;
            UpdateHealthBarUI();
        }
    }

    void UpdateHealthBarUI()
    {
        if (bossHealthFill != null)
        {
            bossHealthFill.fillAmount = Mathf.Clamp01(health / maxHealth);
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (sr != null && !isPreparing && !isDashing)
        {
            float dirX = player.position.x - transform.position.x;
            if (Mathf.Abs(dirX) > 0.05f) sr.flipX = dirX < 0;
        }

        if (Time.time >= nextDashTime && !isPreparing && !isDashing)
        {
            StartCoroutine(ChargeAndDashRoutine());
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null) return;

        if (!isPreparing && !isDashing)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * walkSpeed;
        }
    }

    IEnumerator ChargeAndDashRoutine()
    {
        isPreparing = true;
        rb.linearVelocity = Vector2.zero;

        if (sr != null) sr.color = new Color(1f, 0.4f, 0.4f);
        yield return new WaitForSeconds(chargePrepareTime);

        isPreparing = false;
        isDashing = true;

        if (sr != null) sr.color = Color.white;

        Vector2 dashDir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        nextDashTime = Time.time + dashCooldown;
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector2.zero);
    }

    public void TakeDamage(float amount, Vector2 knockbackDir)
    {
        if (isDead) return;

        health -= amount;
        UpdateHealthBarUI();

        if (hasarYazisiPrefab != null)
        {
            Vector3 yaziPos = transform.position + new Vector3(0f, -1.8f, 0f);
            GameObject yaziObj = Instantiate(hasarYazisiPrefab, yaziPos, Quaternion.identity);
            FloatingText ft = yaziObj.GetComponent<FloatingText>();
            if (ft != null) ft.Kurulum((int)amount);
        }

        if (vurusEfektiPrefab != null)
        {
            Instantiate(vurusEfektiPrefab, transform.position, Quaternion.identity);
        }

        if (sr != null && !isPreparing)
        {
            sr.color = Color.red;
            CancelInvoke(nameof(ResetColor));
            Invoke(nameof(ResetColor), 0.1f);
        }

        if (rb != null && knockbackDir != Vector2.zero)
        {
            Vector2 emilmisKuvvet = knockbackDir * 4f * (1f - knockbackResistance);
            rb.linearVelocity += emilmisKuvvet;
        }

        if (health <= 0) Die();
    }

    void ResetColor()
    {
        if (sr != null && !isDead && !isPreparing)
        {
            sr.color = Color.white;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (bossBarPanel != null)
        {
            bossBarPanel.SetActive(false);
        }

        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        if (col != null) col.enabled = false;

        if (GameManager.Instance != null) GameManager.Instance.AddKill();
        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null) wm.EnemyDied();

        if (player != null)
        {
            PlayerMovement pScript = player.GetComponent<PlayerMovement>();
            if (pScript != null) pScript.AddGold(dropGold);
        }

        if (kanBirikintisiPrefablari != null && kanBirikintisiPrefablari.Length > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                int r = Random.Range(0, kanBirikintisiPrefablari.Length);
                Instantiate(kanBirikintisiPrefablari[r], transform.position + (Vector3)Random.insideUnitCircle * 0.5f, Quaternion.identity);
            }
        }

        if (xpKristaliPrefab != null)
        {
            for (int i = 0; i < dropXpCount; i++)
            {
                Vector3 ofs = (Vector3)Random.insideUnitCircle * 0.7f;
                Instantiate(xpKristaliPrefab, transform.position + ofs, Quaternion.identity);
            }
        }

        this.enabled = false;
        Destroy(gameObject, 1f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement pScript = collision.gameObject.GetComponent<PlayerMovement>();
            if (pScript != null)
            {
                float totalDmg = isDashing ? contactDamage * 1.5f : contactDamage;
                pScript.TakeDamage((int)totalDmg);
            }
        }
    }
}