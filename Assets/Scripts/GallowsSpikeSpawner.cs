using UnityEngine;

public class GallowsSpikeSpawner : MonoBehaviour
{
    [Header("Yetenek Ayarlarý")]
    public GameObject spikePrefab;
    public float beklemeSuresi = 2.2f;
    public float menzil = 9f;
    public float hasar = 45f;

    [HideInInspector] public Transform oyuncu;
    private float sayac;

    void Start()
    {
        if (oyuncu == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) oyuncu = p.transform;
        }
        sayac = 0.5f;
    }

    void Update()
    {
        if (oyuncu == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) oyuncu = p.transform;
            return;
        }

        transform.position = oyuncu.position;

        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            KazikFirlat();
            sayac = beklemeSuresi;
        }
    }

    void KazikFirlat()
    {
        if (spikePrefab == null) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform enUzak = null;
        float enUzunMesafe = -1f;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(oyuncu.position, enemy.transform.position);
            if (dist <= menzil && dist > enUzunMesafe)
            {
                enUzunMesafe = dist;
                enUzak = enemy.transform;
            }
        }

        if (enUzak != null)
        {
            Vector3 hedefPos = enUzak.position;
            Vector3 dogusPos = hedefPos + Vector3.up * 4.5f;

            GameObject spike = Instantiate(spikePrefab, dogusPos, Quaternion.identity);
            GallowsSpike gs = spike.GetComponent<GallowsSpike>();
            if (gs != null)
            {
                gs.Baslat(hedefPos, hasar);
            }
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1) { hasar = 45f; beklemeSuresi = 2.2f; }
        else if (seviye == 2) { hasar = 75f; beklemeSuresi = 1.7f; }
        else if (seviye == 3) { hasar = 110f; beklemeSuresi = 1.2f; }
    }
}