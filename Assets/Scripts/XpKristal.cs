using UnityEngine;

public class XpKristali : MonoBehaviour
{
    [Header("Ayarlar")]
    public int xpDegeri = 24;
    public float cekimMesafesi = 3f;
    public float cekimHizi = 10f;

    private Transform player;
    private bool cekiliyorMu = false;
    private bool toplandiMi = false; // Çift puanı engelleyen kilit

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || toplandiMi) return;

        float mesafe = Vector2.Distance(transform.position, player.position);

        if (mesafe <= cekimMesafesi)
        {
            cekiliyorMu = true;
        }

        if (cekiliyorMu)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, cekimHizi * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Eğer zaten toplandıysa veya çarpan şey Player değilse hiçbir şey yapma
        if (toplandiMi || !other.CompareTag("Player")) return;

        PlayerMovement oyuncuScripti = other.GetComponent<PlayerMovement>();

        if (oyuncuScripti != null)
        {
            toplandiMi = true; // Kilidi kapat, ikinci kez çalışmasın
            oyuncuScripti.PuanEkle(xpDegeri);

            // Görsel olarak hemen kaybetmek yerine 0.01 saniye sonra yok et
            Destroy(gameObject);
        }
    }
}