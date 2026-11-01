using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Ayarlar")]
    public float health = 10f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.1f;
    public float knockbackLength = 0.15f;

    [Header("Ekonomi (Altın Ödülü)")]
    public int minGold = 1;
    public int maxGold = 2;

    [Header("Efektler")]
    public GameObject vurusEfektiPrefab;
    public GameObject xpKristaliPrefab;
    public GameObject hasarYazisiPrefab;
    public GameObject[] kanBirikintisiPrefablari;

    [Header("Referanslar")]
    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    private float knockbackCount;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        if (rb != null) rb.freezeRotation = true;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isDead) return;

        if (player != null && knockbackCount <= 0)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            float currentSpeed = (Time.time < lastAttackTime + 0.5f) ? moveSpeed * 0.2f : moveSpeed;

#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = dir * currentSpeed;
#else
            rb.velocity = dir * currentSpeed;
#endif

            if (dir.x > 0) sprite.flipX = false;
            else if (dir.x < 0) sprite.flipX = true;
        }
        else
        {
            knockbackCount -= Time.deltaTime;
        }
    }

    public void SetStats(float newH, float newS)
    {
        health = newH;
        moveSpeed = newS;
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector2.zero);
    }

    public void TakeDamage(float amount, Vector2 knockbackDir)
    {
        if (isDead) return;

        health -= amount;
        knockbackCount = knockbackLength;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = knockbackDir * 5f;
#else
        rb.velocity = knockbackDir * 5f;
#endif

        if (hasarYazisiPrefab != null)
        {
            Vector3 yaziPozisyonu = transform.position + new Vector3(0f, -1.5f, 0f);
            GameObject yaziObj = Instantiate(hasarYazisiPrefab, yaziPozisyonu, Quaternion.identity);
            FloatingText ft = yaziObj.GetComponent<FloatingText>();
            if (ft != null) ft.Kurulum((int)amount);
        }

        if (anim != null) anim.SetTrigger("Hurt");

        StartCoroutine(FlashWhite());

        if (vurusEfektiPrefab != null)
        {
            Instantiate(vurusEfektiPrefab, transform.position, Quaternion.identity);
        }

        if (health <= 0) Die();
    }

    IEnumerator FlashWhite()
    {
        if (sprite == null) yield break;
        Color originalColor = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sprite != null) sprite.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null) waveManager.EnemyDied();

        if (GameManager.Instance != null) GameManager.Instance.AddKill();

        if (player != null)
        {
            PlayerMovement pScript = player.GetComponent<PlayerMovement>();
            if (pScript != null)
            {
                int verilecekAltin = Random.Range(minGold, maxGold + 1);
                pScript.AddGold(verilecekAltin);
            }
        }

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector2.zero;
#else
        rb.velocity = Vector2.zero;
#endif
        rb.bodyType = RigidbodyType2D.Static;

        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

        if (anim != null) anim.SetBool("isDead", true);

        if (kanBirikintisiPrefablari != null && kanBirikintisiPrefablari.Length > 0)
        {
            int rastgeleIndex = Random.Range(0, kanBirikintisiPrefablari.Length);
            Instantiate(kanBirikintisiPrefablari[rastgeleIndex], transform.position, Quaternion.identity);
        }

        if (xpKristaliPrefab != null)
            Instantiate(xpKristaliPrefab, transform.position, Quaternion.identity);

        this.enabled = false;
        Destroy(gameObject, 1.2f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time > lastAttackTime + attackCooldown)
            {
                if (anim != null) anim.SetTrigger("Attack");
                lastAttackTime = Time.time;

                var pScript = collision.gameObject.GetComponent<PlayerMovement>();
                if (pScript != null) pScript.TakeDamage(10);
            }
        }
    }
}