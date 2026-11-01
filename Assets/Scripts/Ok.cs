using UnityEngine;

public class Ok : MonoBehaviour
{
    [Header("Ayarlar")]
    public float mermiHizi = 12f;
    public float yokOlmaSuresi = 3f;

    private int hasar;
    private Vector2 gidisYonu;

    // Okçu bu fonksiyonu çağırarak yönü ve hasarı belirler
    public void Kurulum(Vector2 yon, int gelenHasar)
    {
        gidisYonu = yon;
        hasar = gelenHasar;

        // Oku gittiği yöne doğru çevirir
        float aci = Mathf.Atan2(yon.y, yon.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, aci);

        // Boşa giderse 3 saniye sonra yok olur
        Destroy(gameObject, yokOlmaSuresi);
    }

    void Update()
    {
        // Oku hareket ettir
        transform.Translate(gidisYonu * mermiHizi * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D temas)
    {
        // Eğer çarptığı objenin Tag'i "Player" ise
        if (temas.CompareTag("Player"))
        {
            // Senin PlayerMovement scriptine erişiyoruz
            PlayerMovement oyuncuCanSistemi = temas.GetComponent<PlayerMovement>();

            if (oyuncuCanSistemi != null)
            {
                // Senin kodundaki TakeDamage fonksiyonunu çağırıyoruz
                oyuncuCanSistemi.TakeDamage(hasar);
                Debug.Log("Okçu sana " + hasar + " hasar verdi!");
            }

            // Çarptıktan sonra oku yok et
            Destroy(gameObject);
        }

        // Not: Eğer 'Obstacle' veya 'Ground' hatası alırsan Unity'den Tag eklemeyi unutma.
        // Hata almamak için şimdilik sadece Player kontrolü bıraktım.
    }
}