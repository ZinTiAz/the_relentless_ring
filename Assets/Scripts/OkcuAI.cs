using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OkcuAI : MonoBehaviour, IDamageable
{
    [Header("Referanslar")]
    public Transform oyuncu;
    public GameObject okPrefab;
    public Transform atisNoktasi;

    [Header("Ekonomi (Altın ve XP)")]
    public int minGold = 2;
    public int maxGold = 3;
    public GameObject xpKristaliPrefab;

    [Header("Efektler")]
    public GameObject vurusEfektiPrefab;
    public GameObject[] kanBirikintisiPrefablari;
    public GameObject hasarYazisiPrefab;

    [Header("Hareket Ayarlari")]
    public float hiz = 2.8f;
    public float durmaMesafesi = 5.5f;
    public float takipMesafesi = 6.5f;

    [Header("Saldırı Ayarlari")]
    public float atisMesafesi = 8.5f;
    public float atesHizi = 2.2f;
    public int okHasari = 10;
    private float nextFireTime;

    [Header("Can Ayarlari")]
    public float maxCan = 50;
    private float can;
    private bool isDead = false;

    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        can = maxCan;

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 5f;
    }

    void Start()
    {
        if (oyuncu == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) oyuncu = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (isDead || oyuncu == null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
            return;
        }

        float mesafe = Vector2.Distance(transform.position, oyuncu.position);
        HandleRotation();

        if (mesafe > takipMesafesi)
        {
            Vector2 dir = (oyuncu.position - transform.position).normalized;
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = dir * hiz;
#else
            rb.velocity = dir * hiz;
#endif
            anim.SetBool("isWalking", true);
        }
        else if (mesafe <= durmaMesafesi)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
            anim.SetBool("isWalking", false);
        }

        if (mesafe <= atisMesafesi && Time.time >= nextFireTime)
        {
            Attack();
        }
    }

    void Attack()
    {
        nextFireTime = Time.time + atesHizi;
        anim.SetTrigger("attack");

        if (okPrefab != null && atisNoktasi != null)
        {
            GameObject yeniOk = Instantiate(okPrefab, atisNoktasi.position, Quaternion.identity);
            Ok okScript = yeniOk.GetComponent<Ok>();
            if (okScript != null)
            {
                Vector2 yon = (oyuncu.position - atisNoktasi.position).normalized;
                okScript.Kurulum(yon, okHasari);
            }
        }
    }

    public void TakeDamage(float miktar)
    {
        TakeDamage(miktar, Vector2.zero);
    }

    public void TakeDamage(float miktar, Vector2 knockbackDir)
    {
        if (isDead) return;
        can -= miktar;

        if (hasarYazisiPrefab != null)
        {
            Vector3 yaziPozisyonu = transform.position + new Vector3(0f, -1.5f, 0f);
            GameObject yaziObj = Instantiate(hasarYazisiPrefab, yaziPozisyonu, Quaternion.identity);
            FloatingText ft = yaziObj.GetComponent<FloatingText>();
            if (ft != null) ft.Kurulum((int)miktar);
        }

        if (vurusEfektiPrefab != null)
        {
            Vector3 kanPozisyonu = transform.position + new Vector3(0f, -0.4f, 0f);
            Instantiate(vurusEfektiPrefab, kanPozisyonu, Quaternion.identity);
        }

        if (can > 0) anim.SetTrigger("hit");
        else Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null) waveManager.EnemyDied();

        if (GameManager.Instance != null) GameManager.Instance.AddKill();

        if (oyuncu != null)
        {
            PlayerMovement pScript = oyuncu.GetComponent<PlayerMovement>();
            if (pScript != null)
            {
                int verilecekAltin = Random.Range(minGold, maxGold + 1);
                pScript.AddGold(verilecekAltin);
            }
        }

        if (xpKristaliPrefab != null)
            Instantiate(xpKristaliPrefab, transform.position, Quaternion.identity);

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector2.zero;
#else
        rb.velocity = Vector2.zero;
#endif
        rb.simulated = false;
        anim.SetBool("isWalking", false);
        anim.SetTrigger("die");

        if (kanBirikintisiPrefablari != null && kanBirikintisiPrefablari.Length > 0)
        {
            int randIndex = Random.Range(0, kanBirikintisiPrefablari.Length);
            if (kanBirikintisiPrefablari[randIndex] != null)
            {
                Vector3 birikintiPozisyonu = transform.position + new Vector3(0f, -1.6f, 0f);
                Instantiate(kanBirikintisiPrefablari[randIndex], birikintiPozisyonu, Quaternion.identity);
            }
        }

        this.enabled = false;
        Destroy(gameObject, 2f);
    }

    void HandleRotation()
    {
        if (oyuncu.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }
}