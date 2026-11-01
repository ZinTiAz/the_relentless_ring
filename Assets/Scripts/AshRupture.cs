using UnityEngine;

public class AshRupture : MonoBehaviour
{
    [Header("Prefab & Hýz")]
    public GameObject mermiPrefab;      // Çizdiðin taþ/patlama prefab'ý
    public float mermiHizi = 8f;        // Taþýn uçuþ hýzý

    [Header("Yetenek Ayarlarý")]
    public float beklemeSuresi = 3f;
    public float patlamaYaricapi = 3.5f;
    public float hasar = 25f;
    public float savurmaGucu = 4f;
    public float algilamaMenzili = 12f; // Karakterin ne kadar uzaðýndaki düþmaný görecek

    [HideInInspector] public Transform oyuncu;
    private float sayac;

    void Start()
    {
        BulOyuncu();
        sayac = beklemeSuresi;
    }

    void Update()
    {
        if (oyuncu == null)
        {
            BulOyuncu();
            return;
        }

        transform.position = oyuncu.position;

        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            Transform hedef = EnYakinDusmaniBul();
            if (hedef != null)
            {
                Firlat(hedef.position);
                sayac = beklemeSuresi;
            }
        }
    }

    void BulOyuncu()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) oyuncu = p.transform;
    }

    Transform EnYakinDusmaniBul()
    {
        GameObject[] dusmanlar = GameObject.FindGameObjectsWithTag("Enemy");
        Transform enYakin = null;
        float enKisaMesafe = algilamaMenzili;

        foreach (GameObject d in dusmanlar)
        {
            float mesafe = Vector2.Distance(transform.position, d.transform.position);
            if (mesafe < enKisaMesafe)
            {
                enKisaMesafe = mesafe;
                enYakin = d.transform;
            }
        }

        return enYakin;
    }

    void Firlat(Vector3 hedefPozisyonu)
    {
        if (mermiPrefab == null) return;

        GameObject mermi = Instantiate(mermiPrefab, transform.position, Quaternion.identity);
        Vector2 firlatmaYonu = (hedefPozisyonu - transform.position).normalized;

        AshRuptureProjectile projectileScript = mermi.GetComponent<AshRuptureProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Baslat(firlatmaYonu, mermiHizi, hasar, patlamaYaricapi, savurmaGucu);
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1)
        {
            hasar = 25f;
            patlamaYaricapi = 3.5f;
            beklemeSuresi = 3f;
        }
        else if (seviye == 2)
        {
            hasar = 40f;
            patlamaYaricapi = 4.2f;
            beklemeSuresi = 2.5f;
        }
        else if (seviye == 3)
        {
            hasar = 65f;
            patlamaYaricapi = 5f;
            beklemeSuresi = 2f;
        }
    }
}