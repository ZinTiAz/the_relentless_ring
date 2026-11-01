using UnityEngine;
using System.Collections.Generic;

public class ShadowCompanion : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public float hasar = 14f;
    public float hareketHizi = 5.5f;
    public float saldiriBeklemeSuresi = 1.4f;
    public float tasmaMenzili = 7.5f;        // Oyuncudan en fazla ne kadar uzaklaþabilir
    public float vurmaMesafesi = 0.4f;

    [Header("Seviye")]
    public int seviye = 1;

    [HideInInspector] public Transform oyuncu;

    private Transform hedef;
    private float sonrakiSaldiriZamani = 0f;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (oyuncu == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) oyuncu = p.transform;
        }

        if (sr != null)
        {
            sr.color = new Color(0.15f, 0.25f, 0.5f, 0.65f);
            sr.sortingOrder = -1; // Karakter ve düþmanlarýn altýnda kalsýn
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

        // Karakter çok uzaklaþýrsa hedefini býrakýp hemen oyuncuya geri dönsün
        float oyuncuyaMesafe = Vector2.Distance(transform.position, oyuncu.position);
        if (oyuncuyaMesafe > tasmaMenzili)
        {
            transform.position = Vector2.MoveTowards(transform.position, oyuncu.position, hareketHizi * 1.5f * Time.deltaTime);
            hedef = null;
            return;
        }

        // Saldýrý bekleme süresindeyse yerinde dursun veya sahibine yaklaþsýn
        if (Time.time < sonrakiSaldiriZamani)
        {
            transform.position = Vector2.MoveTowards(transform.position, oyuncu.position, (hareketHizi * 0.4f) * Time.deltaTime);
            return;
        }

        // Geçerli bir hedef yoksa yeni hedef seç
        if (hedef == null || !hedef.gameObject.activeInHierarchy)
        {
            HedefBul();
        }

        // Hedefe doðru kay
        if (hedef != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, hedef.position, hareketHizi * Time.deltaTime);

            if (sr != null)
            {
                sr.flipX = hedef.position.x < transform.position.x;
            }

            // Hedefe ulaþtý mý?
            if (Vector2.Distance(transform.position, hedef.position) <= vurmaMesafesi)
            {
                Vur(hedef);
                sonrakiSaldiriZamani = Time.time + saldiriBeklemeSuresi;
                hedef = null; // Vurduktan sonra yeni hedef arayacak
            }
        }
        else
        {
            // Yakýnda düþman yoksa oyuncunun yanýnda dursun
            transform.position = Vector2.MoveTowards(transform.position, oyuncu.position, hareketHizi * Time.deltaTime);
        }
    }

    void HedefBul()
    {
        Collider2D[] hedefler = Physics2D.OverlapCircleAll(oyuncu.position, tasmaMenzili);
        List<Transform> gecerliDusmanlar = new List<Transform>();

        foreach (Collider2D col in hedefler)
        {
            if (col.CompareTag("Enemy"))
            {
                gecerliDusmanlar.Add(col.transform);
            }
        }

        if (gecerliDusmanlar.Count > 0)
        {
            int rastgeleIndex = Random.Range(0, gecerliDusmanlar.Count);
            hedef = gecerliDusmanlar[rastgeleIndex];
        }
    }

    void Vur(Transform dusmanTransform)
    {
        // 1. Evrensel IDamageable
        IDamageable damageable = dusmanTransform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(hasar, Vector2.zero);
            return;
        }

        // 2. Dash Düþmaný
        DashEnemyAI dash = dusmanTransform.GetComponent<DashEnemyAI>();
        if (dash != null)
        {
            dash.TakeDamage(hasar, Vector2.zero);
            return;
        }

        // 3. Yarasa
        Enemy yarasa = dusmanTransform.GetComponent<Enemy>();
        if (yarasa != null)
        {
            yarasa.TakeDamage(hasar, Vector2.zero);
            return;
        }

        // 4. Okçu
        OkcuAI okcu = dusmanTransform.GetComponent<OkcuAI>();
        if (okcu != null)
        {
            okcu.TakeDamage(hasar);
            return;
        }
    }

    public void SeviyeGuncelle(int yeniSeviye)
    {
        seviye = yeniSeviye;
        if (seviye == 1)
        {
            hasar = 14f;
            hareketHizi = 5.5f;
            saldiriBeklemeSuresi = 1.4f;
        }
        else if (seviye == 2)
        {
            hasar = 22f;
            hareketHizi = 7.0f;
            saldiriBeklemeSuresi = 1.0f;
        }
        else if (seviye == 3)
        {
            hasar = 32f;
            hareketHizi = 8.5f;
            saldiriBeklemeSuresi = 0.65f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (oyuncu != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(oyuncu.position, tasmaMenzili);
        }
    }
}