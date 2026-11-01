using UnityEngine;
using System.Collections;

public class BloodSplatter : MonoBehaviour
{
    public float kalmaSuresi = 60f; // 1 dakika (60 saniye)
    public float silinmeHizi = 2f; // Silinirken ne kadar sürede şeffaflaşacak
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // Rastgele biraz döndürelim ki her kan lekesi aynı durmasın
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        // Rastgele hafif boyut farkı verelim (doğallık için)
        float randomScale = Random.Range(0.8f, 1.2f);
        transform.localScale = new Vector3(randomScale, randomScale, 1);

        StartCoroutine(YavascaSil());
    }

    IEnumerator YavascaSil()
    {
        // 1 dakika boyunca hiçbir şey yapmadan bekle
        yield return new WaitForSeconds(kalmaSuresi);

        float sayac = 0;
        Color baslangicRengi = sr.color;

        // Belirlenen sürede yavaşça şeffaflaş (Alpha düşür)
        while (sayac < silinmeHizi)
        {
            sayac += Time.deltaTime;
            float yeniAlpha = Mathf.Lerp(1, 0, sayac / silinmeHizi);
            sr.color = new Color(baslangicRengi.r, baslangicRengi.g, baslangicRengi.b, yeniAlpha);
            yield return null;
        }

        // Tamamen görünmez olunca objeyi yok et (Hafıza temizliği)
        Destroy(gameObject);
    }
}