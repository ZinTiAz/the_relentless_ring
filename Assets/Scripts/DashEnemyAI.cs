using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashEnemyAI : MonoBehaviour, IDamageable
{
    [Header("Temel Ayarlar")]
    public float maxCan = 16f;           // Yarasa 10, Okçu 50; bu yaratýk 16 can ile 2 vuruþta erir
    private float mevcutCan;
    public float normalHiz = 2.0f;
    public float hasar = 8f;             // Normal temas hasarý
    public float dashHasari = 12f;       // Roket gibi fýrlarken vurursa vereceði hasar
    public float saldiriAraligi = 1.0f;  // Tek atmasýný engelleyen hasar sýklýðý (saniyede 1 vuruþ)
    private float sonSaldiriZamani = -10f;

    [Header("Dash (Hücum) Ayarlarý")]
    public float tetiklenmeMesafesi = 4.8f;
    public float hazirlikSuresi = 0.8f;  // Oyuncunun fark edip kaçmasý için yeterli pay
    public float dashHizi = 8.0f;
    public float dashSuresi = 0.35f;
    public float beklemeSuresi = 3.2f;   // Ýki hücum arasý dinlenme süresi

    [Header("Ekonomi (Altýn & XP)")]
    public int minGold = 2;
    public int maxGold = 3;
    public GameObject xpKristaliPrefab;

    [Header("Efektler")]
    public GameObject hasarYazisiPrefab;
    public GameObject[] kanBirikintisiPrefablari;

    private Transform oyuncu;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color orijinalRenk;

    private enum Durum { Takip, Hazirlik, Dash, Bekleme }
    private Durum aktifDurum = Durum.Takip;
    private float sonDashZamani = -10f;
    private Vector2 dashYonu;
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) orijinalRenk = sr.color;
    }

    void Start()
    {
        mevcutCan = maxCan;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) oyuncu = p.transform;
    }

    void Update()
    {
        if (isDead || oyuncu == null || Time.timeScale == 0f) return;

        if (aktifDurum == Durum.Takip && sr != null)
        {
            sr.flipX = oyuncu.position.x < transform.position.x;
        }

        float mesafe = Vector2.Distance(transform.position, oyuncu.position);

        if (aktifDurum == Durum.Takip)
        {
            if (mesafe <= tetiklenmeMesafesi && Time.time >= sonDashZamani + beklemeSuresi)
            {
                StartCoroutine(DashRoutini());
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead || oyuncu == null) return;

        if (aktifDurum == Durum.Takip)
        {
            Vector2 yon = (oyuncu.position - transform.position).normalized;
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = yon * normalHiz;
#else
            rb.velocity = yon * normalHiz;
#endif
        }
        else if (aktifDurum == Durum.Dash)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = dashYonu * dashHizi;
#else
            rb.velocity = dashYonu * dashHizi;
#endif
        }
        else
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
        }
    }

    IEnumerator DashRoutini()
    {
        aktifDurum = Durum.Hazirlik;
        if (sr != null) sr.color = Color.red;

        yield return new WaitForSeconds(hazirlikSuresi);

        dashYonu = (oyuncu.position - transform.position).normalized;
        aktifDurum = Durum.Dash;
        if (sr != null) sr.color = Color.yellow;

        yield return new WaitForSeconds(dashSuresi);

        aktifDurum = Durum.Bekleme;
        if (sr != null) sr.color = orijinalRenk;

        yield return new WaitForSeconds(0.4f);

        sonDashZamani = Time.time;
        aktifDurum = Durum.Takip;
    }

    public void TakeDamage(float hasarMiktari)
    {
        TakeDamage(hasarMiktari, Vector2.zero);
    }

    public void TakeDamage(float hasarMiktari, Vector2 itmeYonu)
    {
        if (isDead) return;

        mevcutCan -= hasarMiktari;

        if (hasarYazisiPrefab != null)
        {
            Vector3 yaziPozisyonu = transform.position + new Vector3(0f, 0.8f, 0f);
            GameObject yaziObj = Instantiate(hasarYazisiPrefab, yaziPozisyonu, Quaternion.identity);
            FloatingText ft = yaziObj.GetComponent<FloatingText>();
            if (ft != null) ft.Kurulum((int)hasarMiktari);
        }

        if (aktifDurum != Durum.Dash && itmeYonu != Vector2.zero)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = itmeYonu * 3f;
#else
            rb.velocity = itmeYonu * 3f;
#endif
        }

        if (sr != null) StartCoroutine(HasarFlash());

        if (mevcutCan <= 0f)
        {
            Olum();
        }
    }

    IEnumerator HasarFlash()
    {
        if (sr != null)
        {
            Color onceki = sr.color;
            sr.color = Color.white;
            yield return new WaitForSeconds(0.08f);
            if (sr != null) sr.color = (aktifDurum == Durum.Hazirlik) ? Color.red : orijinalRenk;
        }
    }

    void Olum()
    {
        if (isDead) return;
        isDead = true;

        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null) wm.EnemyDied();

        if (GameManager.Instance != null) GameManager.Instance.AddKill();

        if (oyuncu != null)
        {
            PlayerMovement p = oyuncu.GetComponent<PlayerMovement>();
            if (p != null)
            {
                int verilecekAltin = Random.Range(minGold, maxGold + 1);
                p.AddGold(verilecekAltin);
            }
        }

        if (xpKristaliPrefab != null)
        {
            Instantiate(xpKristaliPrefab, transform.position, Quaternion.identity);
        }

        if (kanBirikintisiPrefablari != null && kanBirikintisiPrefablari.Length > 0)
        {
            int randIndex = Random.Range(0, kanBirikintisiPrefablari.Length);
            if (kanBirikintisiPrefablari[randIndex] != null)
            {
                Instantiate(kanBirikintisiPrefablari[randIndex], transform.position, Quaternion.identity);
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

        this.enabled = false;
        Destroy(gameObject, 0.4f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Belirlenen aralýk dolmadan tekrar hasar veremez (Tek atma sorununu çözen yer)
            if (Time.time >= sonSaldiriZamani + saldiriAraligi)
            {
                PlayerMovement p = collision.gameObject.GetComponent<PlayerMovement>();
                if (p != null)
                {
                    // Dash anýndaysa 12, normal takip anýndaysa 8 hasar verir
                    float verilecekHasar = (aktifDurum == Durum.Dash) ? dashHasari : hasar;
                    p.TakeDamage((int)verilecekHasar);
                    sonSaldiriZamani = Time.time;
                }
            }
        }
    }
}