using UnityEngine;

public class AshRuptureProjectile : MonoBehaviour
{
    [Header("Görsel ve Alan Ayarları")]
    public float patlamaBoyutCarpani = 3.5f; // Patlayınca sprite kaç kat büyüsün

    private float hasar;
    private float patlamaYaricapi;
    private float savurmaGucu;
    private Vector2 hedefYonu;
    private float ucusHizi;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private bool patladi = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Baslat(Vector2 yon, float hiz, float hsr, float yaricap, float savurma)
    {
        hedefYonu = yon;
        ucusHizi = hiz;
        hasar = hsr;
        patlamaYaricapi = yaricap;
        savurmaGucu = savurma;

        if (rb != null)
        {
            rb.linearVelocity = hedefYonu * ucusHizi;
        }

        Invoke(nameof(Patlat), 4f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (patladi) return;

        if (collision.CompareTag("Enemy"))
        {
            Patlat();
        }
    }

    void Patlat()
    {
        if (patladi) return;
        patladi = true;

        CancelInvoke(nameof(Patlat));

        // 1. Hareketi durdur
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 2. Patlama görselini büyüt
        transform.localScale = transform.localScale * patlamaBoyutCarpani;

        // 3. Görsel boyut ile hasar alanını birebir eşitle
        if (sr != null && sr.sprite != null)
        {
            patlamaYaricapi = (sr.sprite.rect.width / sr.sprite.pixelsPerUnit) * 0.5f * transform.localScale.x;
            sr.sortingOrder = 15;
        }

        // 4. Patlama animasyonunu tetikle
        if (anim != null)
        {
            anim.enabled = true;
            anim.SetTrigger("Explode");
        }

        // 5. Tam eşitlenen yarıçap ile alan hasarı uygula
        Collider2D[] vurulanlar = Physics2D.OverlapCircleAll(transform.position, patlamaYaricapi);

        foreach (Collider2D col in vurulanlar)
        {
            if (col.CompareTag("Enemy"))
            {
                Vector2 savurmaYonu = (col.transform.position - transform.position).normalized;
                Vector2 savurmaKuvveti = savurmaYonu * savurmaGucu;

                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(hasar, savurmaKuvveti);
                    continue;
                }

                DashEnemyAI dash = col.GetComponent<DashEnemyAI>();
                if (dash != null)
                {
                    dash.TakeDamage(hasar, savurmaKuvveti);
                    continue;
                }

                Enemy yarasa = col.GetComponent<Enemy>();
                if (yarasa != null)
                {
                    yarasa.TakeDamage(hasar, savurmaKuvveti);
                    continue;
                }

                OkcuAI okcu = col.GetComponent<OkcuAI>();
                if (okcu != null)
                {
                    okcu.TakeDamage(hasar);
                    continue;
                }
            }
        }

        Destroy(gameObject, 0.6f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, patlamaYaricapi);
    }
}