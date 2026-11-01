using UnityEngine;

public class LeaveMeAlone : MonoBehaviour
{
    [HideInInspector] public Transform oyuncu;

    [Header("Aura Ayarlarý")]
    public float beklemeSuresi = 3.0f;
    public float etkiYaricapi = 4.5f;
    public float savurmaGucu = 9.0f;

    private float sayac;

    void Start()
    {
        // Oyuncu atanmamýþsa sahneden otomatik bul
        if (oyuncu == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) oyuncu = p.transform;
            else oyuncu = transform; // Kendisi oyuncuysa
        }

        sayac = beklemeSuresi;
    }

    void Update()
    {
        // Eðer script ayrý bir aura objesindeyse, konumu sürekli karaktere sabitle
        if (oyuncu != null && transform != oyuncu)
        {
            transform.position = oyuncu.position;
        }

        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            SadecePuskurt();
            sayac = beklemeSuresi;
        }
    }

    void SadecePuskurt()
    {
        // Merkez olarak mutlaka karakterin o anki gerçek pozisyonunu al
        Vector3 merkezNoktasi = (oyuncu != null) ? oyuncu.position : transform.position;

        Collider2D[] hedefler = Physics2D.OverlapCircleAll(merkezNoktasi, etkiYaricapi);

        foreach (Collider2D col in hedefler)
        {
            if (col.CompareTag("Enemy"))
            {
                Vector2 itmeYonu = ((Vector2)col.transform.position - (Vector2)merkezNoktasi).normalized;
                if (itmeYonu == Vector2.zero) itmeYonu = Vector2.right;

                // 1. Pozisyonu ötele
                col.transform.position = (Vector2)col.transform.position + (itmeYonu * 1.8f);

                // 2. Düþmanýn sersemleme ve savrulmasýný tetikle (0 hasar)
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(0f, itmeYonu * savurmaGucu);
                    continue;
                }

                DashEnemyAI dash = col.GetComponent<DashEnemyAI>();
                if (dash != null)
                {
                    dash.TakeDamage(0f, itmeYonu * savurmaGucu);
                    continue;
                }

                Enemy yarasa = col.GetComponent<Enemy>();
                if (yarasa != null)
                {
                    yarasa.TakeDamage(0f, itmeYonu * savurmaGucu);
                    continue;
                }

                Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
#if UNITY_6000_0_OR_NEWER
                    rb.linearVelocity = itmeYonu * savurmaGucu;
#else
                    rb.velocity = itmeYonu * savurmaGucu;
#endif
                }
            }
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1)
        {
            beklemeSuresi = 3.0f;
            etkiYaricapi = 4.5f;
            savurmaGucu = 9.0f;
        }
        else if (seviye == 2)
        {
            beklemeSuresi = 2.4f;
            etkiYaricapi = 5.2f;
            savurmaGucu = 12.0f;
        }
        else if (seviye == 3)
        {
            beklemeSuresi = 1.7f;
            etkiYaricapi = 6.0f;
            savurmaGucu = 16.0f;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 merkez = (oyuncu != null) ? oyuncu.position : transform.position;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(merkez, etkiYaricapi);
    }
}