using UnityEngine;
using System.Collections;

public class HystericCleaveSpawner : MonoBehaviour
{
    public GameObject cleavePrefab;
    public float beklemeSuresi = 3.0f;
    public float vurusHasari = 16f;
    public int kesikSayisi = 4;
    public float kesikAraligi = 0.1f;

    private float sayac = 0.5f;

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (cleavePrefab == null) return;

        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            StartCoroutine(SeriKesikRutini());
            sayac = beklemeSuresi;
        }
    }

    IEnumerator SeriKesikRutini()
    {
        for (int i = 0; i < kesikSayisi; i++)
        {
            Vur();
            yield return new WaitForSeconds(kesikAraligi);
        }
    }

    void Vur()
    {
        if (Camera.main == null) return;

        Vector3 mouseDunyaPozisyonu = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float bakisYonu = (mouseDunyaPozisyonu.x >= transform.position.x) ? 1f : -1f;

        Vector3 spawnPos = transform.position + new Vector3(bakisYonu * 1.8f, Random.Range(-0.2f, 0.2f), 0f);

        GameObject cleave = Instantiate(cleavePrefab, spawnPos, Quaternion.identity);
        HystericCleave hc = cleave.GetComponent<HystericCleave>();
        if (hc != null)
        {
            hc.Baslat(vurusHasari, new Vector2(bakisYonu, 0f));
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1) { vurusHasari = 16f; kesikSayisi = 4; beklemeSuresi = 3.0f; }
        else if (seviye == 2) { vurusHasari = 24f; kesikSayisi = 5; beklemeSuresi = 2.4f; }
        else if (seviye == 3) { vurusHasari = 34f; kesikSayisi = 7; beklemeSuresi = 1.8f; }
    }
}