using UnityEngine;

public class BleedingStepsSpawner : MonoBehaviour
{
    public GameObject stepPrefab;
    public float adimAraligi = 0.28f;      // Kaç saniyede bir iz býrakýlacak
    public float adimHasari = 8f;

    private float sayac = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (stepPrefab == null) return;

        // Karakterin hareket edip etmediðini kontrol et
        bool hareketEdiyor = false;
        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            hareketEdiyor = rb.linearVelocity.sqrMagnitude > 0.05f;
#else
            hareketEdiyor = rb.velocity.sqrMagnitude > 0.05f;
#endif
        }
        else
        {
            // Rigidbody yoksa klavye tuþlarýndan kontrol et
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            hareketEdiyor = (h != 0 || v != 0);
        }

        if (hareketEdiyor)
        {
            sayac -= Time.deltaTime;
            if (sayac <= 0f)
            {
                IzBirak();
                sayac = adimAraligi;
            }
        }
        else
        {
            // Dururken sayaç sýfýrlanmasýn, yürümeye baþladýðý an ilk iz çýksýn
            sayac = 0.05f;
        }
    }

    void IzBirak()
    {
        // Karakterin ayak ucunun biraz altýna denk gelsin
        Vector3 ayakPozisyonu = transform.position + new Vector3(Random.Range(-0.1f, 0.1f), -0.5f, 0f);

        GameObject iz = Instantiate(stepPrefab, ayakPozisyonu, Quaternion.identity);
        BleedingStep bs = iz.GetComponent<BleedingStep>();
        if (bs != null)
        {
            bs.Baslat(adimHasari);
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1) { adimHasari = 8f; adimAraligi = 0.28f; }
        else if (seviye == 2) { adimHasari = 13f; adimAraligi = 0.22f; }
        else if (seviye == 3) { adimHasari = 20f; adimAraligi = 0.16f; }
    }
}