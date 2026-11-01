using UnityEngine;

public class HystericCleave : MonoBehaviour
{
    public float hasar = 18f;
    public float itmeGucu = 4f;
    public float omur = 0.25f;

    void Start()
    {
        Destroy(gameObject, omur);
    }

    public void Baslat(float vurusHasari, Vector2 yon)
    {
        hasar = vurusHasari;

        // Prefab zaten yatay olduðu için sadece yönünü sað/sol yapýyoruz
        Vector3 s = transform.localScale;
        if (yon.x < 0)
        {
            s.x = -Mathf.Abs(s.x);
        }
        else
        {
            s.x = Mathf.Abs(s.x);
        }
        transform.localScale = s;
        transform.rotation = Quaternion.identity; // Ekstra dönüþü sýfýrla
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            // Ýtme yönünü kesiðin baktýðý yöne göre ayarla
            float yonX = transform.localScale.x > 0 ? 1f : -1f;
            Vector2 itmeYonu = new Vector2(yonX, 0.1f).normalized;
            Vector2 uygulananKuvvet = itmeYonu * itmeGucu;

            // 1. Evrensel Arayüz (IDamageable)
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(hasar, uygulananKuvvet);
                return;
            }

            // 2. Dash Düþmaný
            DashEnemyAI dash = col.GetComponent<DashEnemyAI>();
            if (dash != null)
            {
                dash.TakeDamage(hasar, uygulananKuvvet);
                return;
            }

            // 3. Yarasa
            Enemy yarasa = col.GetComponent<Enemy>();
            if (yarasa != null)
            {
                yarasa.TakeDamage(hasar, uygulananKuvvet);
                return;
            }

            // 4. Okçu
            OkcuAI okcu = col.GetComponent<OkcuAI>();
            if (okcu != null)
            {
                okcu.TakeDamage(hasar);
                return;
            }
        }
    }
}