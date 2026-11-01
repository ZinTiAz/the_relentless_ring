using UnityEngine;

public class EndOfTheRoadSpawner : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float atisAraligi = 3.5f;
    public float hasar = 32f;

    private float sayac = 0f;

    void Update()
    {
        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            YoluTemizle();
            sayac = atisAraligi;
        }
    }

    void YoluTemizle()
    {
        if (projectilePrefab == null) return;

        // En yakýn canlý düþmaný bul ve ona doðru doðrusal bir hat çiz
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform enYakin = null;
        float enKisaMesafe = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            if (e != null && e.activeInHierarchy)
            {
                Collider2D c = e.GetComponent<Collider2D>();
                if (c != null && c.enabled)
                {
                    float d = Vector2.Distance(transform.position, e.transform.position);
                    if (d < enKisaMesafe && d <= 12f)
                    {
                        enKisaMesafe = d;
                        enYakin = e.transform;
                    }
                }
            }
        }

        Vector2 atisYonu;
        if (enYakin != null)
        {
            atisYonu = ((Vector2)enYakin.position - (Vector2)transform.position).normalized;
        }
        else
        {
            // Yakýnda düþman yoksa karakterin baktýðý yöne doðru at
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            atisYonu = (sr != null && sr.flipX) ? Vector2.left : Vector2.right;
        }

        GameObject mermi = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EndOfTheRoadProjectile projScript = mermi.GetComponent<EndOfTheRoadProjectile>();
        if (projScript != null)
        {
            projScript.Firlat(atisYonu, hasar);
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1)
        {
            atisAraligi = 3.5f;
            hasar = 32f;
        }
        else if (seviye == 2)
        {
            atisAraligi = 2.6f;
            hasar = 48f;
        }
        else if (seviye == 3)
        {
            atisAraligi = 1.8f;
            hasar = 70f;
        }
    }
}