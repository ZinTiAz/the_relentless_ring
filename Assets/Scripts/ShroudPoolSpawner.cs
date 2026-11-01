using UnityEngine;

public class ShroudPoolSpawner : MonoBehaviour
{
    public GameObject poolPrefab;
    public float beklemeSuresi = 3.5f;
    public float olusmaMenzili = 3.5f;
    public int guncelSeviye = 1;

    private float sayac = 0.5f;

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (poolPrefab == null) return;

        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            HavuzOlustur();
            sayac = beklemeSuresi;
        }
    }

    void HavuzOlustur()
    {
        Vector2 rastgeleNokta = Random.insideUnitCircle * olusmaMenzili;
        Vector3 spawnPos = new Vector3(transform.position.x + rastgeleNokta.x, transform.position.y + rastgeleNokta.y, 0f);

        GameObject yeniHavuz = Instantiate(poolPrefab, spawnPos, Quaternion.identity);
        ShroudPool sp = yeniHavuz.GetComponent<ShroudPool>();
        if (sp != null)
        {
            sp.SeviyeGuncelle(guncelSeviye);
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        guncelSeviye = seviye;
        if (seviye == 1) beklemeSuresi = 3.5f;
        else if (seviye == 2) beklemeSuresi = 2.8f;
        else if (seviye == 3) beklemeSuresi = 2.0f;
    }
}