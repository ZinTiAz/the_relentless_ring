using UnityEngine;

public class EchoOfMistakesSpawner : MonoBehaviour
{
    public GameObject mistakePrefab;
    public float beklemeSuresi = 3.2f;
    public float hasar = 18f;
    public float mesafe = 6.5f;

    private float sayac = 0f;

    void Update()
    {
        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            HataFirlat();
            sayac = beklemeSuresi;
        }
    }

    void HataFirlat()
    {
        if (mistakePrefab == null) return;

        Vector2 atisYonu = Vector2.right;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float enKisaMesafe = Mathf.Infinity;
        Transform enYakinDusman = null;

        foreach (GameObject e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < enKisaMesafe && d <= 9f)
            {
                enKisaMesafe = d;
                enYakinDusman = e.transform;
            }
        }

        if (enYakinDusman != null)
        {
            atisYonu = ((Vector2)enYakinDusman.position - (Vector2)transform.position).normalized;
        }
        else
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null && pm.transform.localScale.x < 0)
                atisYonu = Vector2.left;
        }

        GameObject mermi = Instantiate(mistakePrefab, transform.position, Quaternion.identity);
        EchoOfMistakes script = mermi.GetComponent<EchoOfMistakes>();
        if (script != null)
        {
            script.oyuncu = transform;
            script.firlatmaYonu = atisYonu;
            script.hasar = hasar;
            script.maksimumMesafe = mesafe;
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1)
        {
            beklemeSuresi = 3.2f;
            hasar = 18f;
            mesafe = 6.5f;
        }
        else if (seviye == 2)
        {
            beklemeSuresi = 2.4f;
            hasar = 26f;
            mesafe = 8.0f;
        }
        else if (seviye == 3)
        {
            beklemeSuresi = 1.6f;
            hasar = 36f;
            mesafe = 9.5f;
        }
    }
}