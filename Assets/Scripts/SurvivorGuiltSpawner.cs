using UnityEngine;
using System.Collections.Generic;

public class SurvivorGuiltSpawner : MonoBehaviour
{
    public float beklemeSuresi = 3.5f;
    public float hasar = 22f;
    public int sicramaSayisi = 3; // Kaç düþmana sekerek gidecek
    public float sicramaMenzili = 5.5f;

    private float sayac = 0f;

    void Update()
    {
        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            ZincirTetikle();
            sayac = beklemeSuresi;
        }
    }

    void ZincirTetikle()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<Transform> canlilar = new List<Transform>();

        foreach (GameObject e in enemies)
        {
            if (e != null && e.activeInHierarchy)
            {
                Collider2D c = e.GetComponent<Collider2D>();
                if (c != null && c.enabled) canlilar.Add(e.transform);
            }
        }

        if (canlilar.Count == 0) return;

        // 1. Ýlk düþmaný bul (karaktere en yakýn olan)
        Transform ilkHedef = null;
        float enKisaMesafe = Mathf.Infinity;
        foreach (Transform t in canlilar)
        {
            float d = Vector2.Distance(transform.position, t.position);
            if (d < enKisaMesafe && d <= 8.5f)
            {
                enKisaMesafe = d;
                ilkHedef = t;
            }
        }

        if (ilkHedef == null) return;

        // Zincirleme takip listesi
        List<Transform> vurulanlar = new List<Transform>();
        List<Vector3> cizgiNoktalari = new List<Vector3>();

        cizgiNoktalari.Add(transform.position); // Karakterden baþlar
        vurulanlar.Add(ilkHedef);
        cizgiNoktalari.Add(ilkHedef.position);
        HasarVer(ilkHedef);

        // 2. Diðer hedeflere sýçra
        Transform sonHedef = ilkHedef;
        for (int i = 1; i < sicramaSayisi; i++)
        {
            Transform siradaki = null;
            float yakinlik = Mathf.Infinity;

            foreach (Transform t in canlilar)
            {
                if (vurulanlar.Contains(t)) continue; // Ayný hedefe iki kez sekmesin

                float mesafe = Vector2.Distance(sonHedef.position, t.position);
                if (mesafe < yakinlik && mesafe <= sicramaMenzili)
                {
                    yakinlik = mesafe;
                    siradaki = t;
                }
            }

            if (siradaki != null)
            {
                vurulanlar.Add(siradaki);
                cizgiNoktalari.Add(siradaki.position);
                HasarVer(siradaki);
                sonHedef = siradaki;
            }
            else
            {
                break; // Yakýnda baþka düþman kalmadýysa zincir biter
            }
        }

        // Görsel çizgiyi oluþtur
        GameObject go = new GameObject("SurvivorGuilt_Effect");
        SurvivorGuilt sg = go.AddComponent<SurvivorGuilt>();
        sg.BagOlustur(cizgiNoktalari, hasar);
    }

    void HasarVer(Transform dusman)
    {
        Vector2 vurusYonu = (dusman.position - transform.position).normalized;

        IDamageable d = dusman.GetComponent<IDamageable>();
        if (d != null) { d.TakeDamage(hasar, vurusYonu * 2f); return; }

        DashEnemyAI dash = dusman.GetComponent<DashEnemyAI>();
        if (dash != null) { dash.TakeDamage(hasar, vurusYonu * 2f); return; }

        Enemy yarasa = dusman.GetComponent<Enemy>();
        if (yarasa != null) { yarasa.TakeDamage(hasar, vurusYonu * 2f); return; }

        OkcuAI okcu = dusman.GetComponent<OkcuAI>();
        if (okcu != null) { okcu.TakeDamage(hasar); return; }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1)
        {
            beklemeSuresi = 3.5f;
            hasar = 22f;
            sicramaSayisi = 3;
            sicramaMenzili = 5.5f;
        }
        else if (seviye == 2)
        {
            beklemeSuresi = 2.7f;
            hasar = 32f;
            sicramaSayisi = 4;
            sicramaMenzili = 6.5f;
        }
        else if (seviye == 3)
        {
            beklemeSuresi = 2.0f;
            hasar = 45f;
            sicramaSayisi = 6;
            sicramaMenzili = 7.5f;
        }
    }
}