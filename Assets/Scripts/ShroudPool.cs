using UnityEngine;
using System.Collections.Generic;

public class ShroudPool : MonoBehaviour
{
    [Header("Temel Parametreler")]
    public float omurSuresi = 3f;
    public float hasarAraligi = 0.5f;
    public float tikBasiHasar = 6f;
    public float yaricap = 2.2f;

    [Header("Yavaþlatma Oraný")]
    [Range(0.1f, 0.9f)]
    public float yavaslatmaOrani = 0.5f; // Hýzý %50'ye düþürür

    private float timer = 0f;
    private float hasarSayaci = 0f;

    // Yavaþlatýlan tüm düþmanlarýn orijinal hýz kayýtlarý
    private Dictionary<Enemy, float> yavaslayanYarasalar = new Dictionary<Enemy, float>();
    private Dictionary<DashEnemyAI, float> yavaslayanDashler = new Dictionary<DashEnemyAI, float>();
    private Dictionary<TankEnemyAI, float> yavaslayanTanklar = new Dictionary<TankEnemyAI, float>();
    private Dictionary<OkcuAI, float> yavaslayanOkcular = new Dictionary<OkcuAI, float>();
    private Dictionary<MiniBossAI, float> yavaslayanMiniBosslar = new Dictionary<MiniBossAI, float>();
    private Dictionary<FinalBossAI, float> yavaslayanFinalBosslar = new Dictionary<FinalBossAI, float>();

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1)
        {
            tikBasiHasar = 6f;
            yaricap = 2.0f;
            omurSuresi = 3f;
            yavaslatmaOrani = 0.6f;
        }
        else if (seviye == 2)
        {
            tikBasiHasar = 9f;
            yaricap = 2.4f;
            omurSuresi = 3.5f;
            yavaslatmaOrani = 0.5f;
        }
        else if (seviye == 3)
        {
            tikBasiHasar = 13f;
            yaricap = 2.8f;
            omurSuresi = 4f;
            yavaslatmaOrani = 0.4f; // %60 yavaþlatma
        }

        timer = omurSuresi;
    }

    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        if (timer <= 0f) timer = omurSuresi;
        hasarSayaci = 0f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        hasarSayaci -= Time.deltaTime;

        if (hasarSayaci <= 0f)
        {
            EtkiyiUygula();
            hasarSayaci = hasarAraligi;
        }

        if (timer <= 0f)
        {
            HizlariGeriYukle();
            Destroy(gameObject);
        }
    }

    void EtkiyiUygula()
    {
        Collider2D[] hedefler = Physics2D.OverlapCircleAll(transform.position, yaricap);

        foreach (Collider2D col in hedefler)
        {
            if (col.CompareTag("Enemy"))
            {
                // 1. Hasar Verme (IDamageable Arayüzü Üzerinden)
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(tikBasiHasar, Vector2.zero);
                }

                // 2. Türüne Göre Hýz Yavaþlatma
                Yavaslat(col);
            }
        }
    }

    void Yavaslat(Collider2D col)
    {
        // Yarasa (Enemy)
        Enemy yarasa = col.GetComponent<Enemy>();
        if (yarasa != null && !yavaslayanYarasalar.ContainsKey(yarasa))
        {
            yavaslayanYarasalar.Add(yarasa, yarasa.moveSpeed);
            yarasa.moveSpeed *= yavaslatmaOrani;
            return;
        }

        // Dash Düþmaný
        DashEnemyAI dash = col.GetComponent<DashEnemyAI>();
        if (dash != null && !yavaslayanDashler.ContainsKey(dash))
        {
            yavaslayanDashler.Add(dash, dash.normalHiz);
            dash.normalHiz *= yavaslatmaOrani;
            return;
        }

        // Tank Düþmaný
        TankEnemyAI tank = col.GetComponent<TankEnemyAI>();
        if (tank != null && !yavaslayanTanklar.ContainsKey(tank))
        {
            yavaslayanTanklar.Add(tank, tank.moveSpeed);
            tank.moveSpeed *= yavaslatmaOrani;
            return;
        }

        // Okçu
        OkcuAI okcu = col.GetComponent<OkcuAI>();
        if (okcu != null && !yavaslayanOkcular.ContainsKey(okcu))
        {
            yavaslayanOkcular.Add(okcu, okcu.hiz);
            okcu.hiz *= yavaslatmaOrani;
            return;
        }

        // Mini Boss
        MiniBossAI miniBoss = col.GetComponent<MiniBossAI>();
        if (miniBoss != null && !yavaslayanMiniBosslar.ContainsKey(miniBoss))
        {
            yavaslayanMiniBosslar.Add(miniBoss, miniBoss.walkSpeed);
            miniBoss.walkSpeed *= yavaslatmaOrani;
            return;
        }

        // Final Boss
        FinalBossAI finalBoss = col.GetComponent<FinalBossAI>();
        if (finalBoss != null && !yavaslayanFinalBosslar.ContainsKey(finalBoss))
        {
            yavaslayanFinalBosslar.Add(finalBoss, finalBoss.walkSpeed);
            finalBoss.walkSpeed *= yavaslatmaOrani;
            return;
        }
    }

    void HizlariGeriYukle()
    {
        foreach (var pair in yavaslayanYarasalar)
        {
            if (pair.Key != null) pair.Key.moveSpeed = pair.Value;
        }
        yavaslayanYarasalar.Clear();

        foreach (var pair in yavaslayanDashler)
        {
            if (pair.Key != null) pair.Key.normalHiz = pair.Value;
        }
        yavaslayanDashler.Clear();

        foreach (var pair in yavaslayanTanklar)
        {
            if (pair.Key != null) pair.Key.moveSpeed = pair.Value;
        }
        yavaslayanTanklar.Clear();

        foreach (var pair in yavaslayanOkcular)
        {
            if (pair.Key != null) pair.Key.hiz = pair.Value;
        }
        yavaslayanOkcular.Clear();

        foreach (var pair in yavaslayanMiniBosslar)
        {
            if (pair.Key != null) pair.Key.walkSpeed = pair.Value;
        }
        yavaslayanMiniBosslar.Clear();

        foreach (var pair in yavaslayanFinalBosslar)
        {
            if (pair.Key != null) pair.Key.walkSpeed = pair.Value;
        }
        yavaslayanFinalBosslar.Clear();
    }

    private void OnDestroy()
    {
        HizlariGeriYukle();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0f, 0.6f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, yaricap);
    }
}