using UnityEngine;

public class ArcOrb : MonoBehaviour
{
    [Header("Orb Ayarlarý")]
    public float donmeHizi = 220f;       // Dönüþ hýzý
    public float mesafe = 2.0f;          // Karakterden uzaklýðý
    public float hasar = 15f;            // Hasar (GDD Seviye 1: 15)
    public float vurusYaricapi = 0.6f;   // Hasar vurma alaný
    public float vurusAraligi = 0.35f;   // Vuruþ sýklýðý

    [HideInInspector] public Transform oyuncu;
    private float aci;
    private float sonVurusZamani;

    void Start()
    {
        if (oyuncu == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) oyuncu = p.transform;
        }
    }

    void Update()
    {
        if (oyuncu == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) oyuncu = p.transform;
            return;
        }

        // Karakterin etrafýnda dairesel dönüþ
        aci += donmeHizi * Time.deltaTime;
        float radyan = aci * Mathf.Deg2Rad;
        Vector3 ofset = new Vector3(Mathf.Cos(radyan), Mathf.Sin(radyan), 0) * mesafe;
        transform.position = oyuncu.position + ofset;

        // Hasar kontrolü (Fizik motoruna takýlmadan doðrudan alan taramasý)
        if (Time.time >= sonVurusZamani + vurusAraligi)
        {
            HasarKontrol();
        }
    }

    void HasarKontrol()
    {
        Collider2D[] carpanlar = Physics2D.OverlapCircleAll(transform.position, vurusYaricapi);
        bool vurduMu = false;

        foreach (Collider2D col in carpanlar)
        {
            if (col.CompareTag("Enemy"))
            {
                Vector2 knockback = (col.transform.position - transform.position).normalized;

                // 1. Evrensel Arayüz Kontrolü (Varsa tüm düþmanlarý tek satýrda yakalar)
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(hasar, knockback);
                    vurduMu = true;
                    continue;
                }

                // 2. Dash Düþmaný Kontrolü
                DashEnemyAI dash = col.GetComponent<DashEnemyAI>();
                if (dash != null)
                {
                    dash.TakeDamage(hasar, knockback);
                    vurduMu = true;
                    continue;
                }

                // 3. Yarasa Kontrolü
                Enemy yarasa = col.GetComponent<Enemy>();
                if (yarasa != null)
                {
                    yarasa.TakeDamage(hasar, knockback);
                    vurduMu = true;
                    continue;
                }

                // 4. Okçu Kontrolü
                OkcuAI okcu = col.GetComponent<OkcuAI>();
                if (okcu != null)
                {
                    okcu.TakeDamage(hasar);
                    vurduMu = true;
                    continue;
                }
            }
        }

        if (vurduMu)
        {
            sonVurusZamani = Time.time;
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1) hasar = 15f;
        else if (seviye == 2) hasar = 25f;
        else if (seviye == 3) hasar = 40f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, vurusYaricapi);
    }
}