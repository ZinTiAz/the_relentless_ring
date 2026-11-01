using UnityEngine;
using System.Collections;

public class GallowsSpike : MonoBehaviour
{
    [Header("Düþüþ ve Boyut")]
    public float dususHizi = 22f;
    public float hasar = 40f;
    public float yakmaSaniyeBasiHasar = 15f;
    public float yakmaYaricapi = 1.6f;
    public float yerdeKalmaSuresi = 2.5f;

    private Vector3 hedefNokta;
    private bool hedefBelirlendi = false;
    private bool saplandi = false;

    private Animator anim;
    private SpriteRenderer sr;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Baslat(Vector3 hedef, float vurulacakHasar)
    {
        hedefNokta = hedef;
        hasar = vurulacakHasar;
        hedefBelirlendi = true;

        // Herhangi bir takýlma ihtimaline karþý maksimum ömür
        Destroy(gameObject, 6f);
    }

    void Update()
    {
        if (!hedefBelirlendi || saplandi) return;

        // Hedefe doðru düþüþ hareketi
        transform.position = Vector3.MoveTowards(transform.position, hedefNokta, dususHizi * Time.deltaTime);

        // Hedefe ulaþtýðý an saplan
        if (Vector3.Distance(transform.position, hedefNokta) <= 0.1f)
        {
            Saplan();
        }
    }

    void Saplan()
    {
        saplandi = true;
        transform.position = hedefNokta; // Pozisyonu hedefe tam kilitle

        // 1. Animator'ý saplanma ve yanma döngüsüne (Burn) geçir
        if (anim != null)
        {
            anim.SetTrigger("Impact");
        }

        // 2. Kýlýcý düþmanlarýn önünde net görünsün diye öne al
        if (sr != null)
        {
            sr.sortingOrder = 12;
        }

        // 3. Ýlk saplanma sert darbe hasarý
        HasarUygula(hasar);

        // 4. Yerde yandýðý süre boyunca periyodik alan hasarý ver (DoT)
        StartCoroutine(YakmaDongusu());

        // 5. Yanma süresi bitince sahneden sil
        Destroy(gameObject, yerdeKalmaSuresi);
    }

    IEnumerator YakmaDongusu()
    {
        float gecenSure = 0f;
        while (gecenSure < yerdeKalmaSuresi)
        {
            yield return new WaitForSeconds(0.5f); // Yarým saniyede bir yakar
            gecenSure += 0.5f;
            HasarUygula(yakmaSaniyeBasiHasar * 0.5f);
        }
    }

    void HasarUygula(float verilecekHasar)
    {
        Collider2D[] hedefler = Physics2D.OverlapCircleAll(transform.position, yakmaYaricapi);
        foreach (Collider2D col in hedefler)
        {
            if (col.CompareTag("Enemy"))
            {
                // 1. Evrensel Arayüz (IDamageable)
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(verilecekHasar, Vector2.down * 1.5f);
                    continue;
                }

                // 2. Dash Düþmaný
                DashEnemyAI dash = col.GetComponent<DashEnemyAI>();
                if (dash != null)
                {
                    dash.TakeDamage(verilecekHasar, Vector2.down * 1.5f);
                    continue;
                }

                // 3. Yarasa
                Enemy yarasa = col.GetComponent<Enemy>();
                if (yarasa != null)
                {
                    yarasa.TakeDamage(verilecekHasar, Vector2.down * 1.5f);
                    continue;
                }

                // 4. Okçu
                OkcuAI okcu = col.GetComponent<OkcuAI>();
                if (okcu != null)
                {
                    okcu.TakeDamage(verilecekHasar);
                    continue;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, yakmaYaricapi);
    }
}