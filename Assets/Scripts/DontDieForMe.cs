using UnityEngine;

public class DontDieForMe : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public float hasar = 16f;
    public float hareketHizi = 6f;
    public float omur = 5.5f;
    public float patlamaHasari = 25f;
    public float patlamaMenzili = 3f;
    public float saldiriAraligi = 0.8f;

    private float olumZamani;
    private float sonrakiVurusZamani = 0f;
    private Transform hedefDusman;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        olumZamani = Time.time + omur;

        if (sr != null)
        {
            sr.color = new Color(0.4f, 0.7f, 1f, 0.75f);
            sr.sortingOrder = 10;
        }
    }

    void Update()
    {
        if (Time.time >= olumZamani)
        {
            AyrilikVaktiPatla();
            return;
        }

        if (Time.time < sonrakiVurusZamani) return;

        // Hedef yoksa, sahnede pasifleþtiyse veya collider'ý kapandýysa yeni hedef bul
        if (hedefDusman == null || !hedefDusman.gameObject.activeInHierarchy || !ColliderAktifMi(hedefDusman))
        {
            HedefBul();
        }

        if (hedefDusman != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, hedefDusman.position, hareketHizi * Time.deltaTime);

            if (sr != null)
            {
                sr.flipX = hedefDusman.position.x < transform.position.x;
            }

            if (Vector2.Distance(transform.position, hedefDusman.position) <= 0.6f)
            {
                HasarVer(hedefDusman);
                sonrakiVurusZamani = Time.time + saldiriAraligi;
                hedefDusman = null;
            }
        }
    }

    void HedefBul()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float enKisaMesafe = Mathf.Infinity;
        Transform enYakin = null;

        foreach (GameObject e in enemies)
        {
            if (e != null && e.activeInHierarchy && ColliderAktifMi(e.transform))
            {
                float d = Vector2.Distance(transform.position, e.transform.position);
                if (d < enKisaMesafe && d <= 10f)
                {
                    enKisaMesafe = d;
                    enYakin = e.transform;
                }
            }
        }
        hedefDusman = enYakin;
    }

    bool ColliderAktifMi(Transform t)
    {
        Collider2D col = t.GetComponent<Collider2D>();
        return col != null && col.enabled;
    }

    void HasarVer(Transform dusman)
    {
        Vector2 yon = ((Vector2)dusman.position - (Vector2)transform.position).normalized;

        IDamageable d = dusman.GetComponent<IDamageable>();
        if (d != null) { d.TakeDamage(hasar, yon * 2f); return; }

        DashEnemyAI dash = dusman.GetComponent<DashEnemyAI>();
        if (dash != null) { dash.TakeDamage(hasar, yon * 2f); return; }

        Enemy yarasa = dusman.GetComponent<Enemy>();
        if (yarasa != null) { yarasa.TakeDamage(hasar, yon * 2f); return; }

        OkcuAI okcu = dusman.GetComponent<OkcuAI>();
        if (okcu != null) { okcu.TakeDamage(hasar); return; }
    }

    void AyrilikVaktiPatla()
    {
        Collider2D[] hedefler = Physics2D.OverlapCircleAll(transform.position, patlamaMenzili);
        foreach (Collider2D col in hedefler)
        {
            if (col.CompareTag("Enemy"))
            {
                HasarVer(col.transform);
            }
        }

        Destroy(gameObject);
    }
}