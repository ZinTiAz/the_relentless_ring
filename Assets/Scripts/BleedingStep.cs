using UnityEngine;
using System.Collections.Generic;

public class BleedingStep : MonoBehaviour
{
    public float hasar = 8f;
    public float omur = 1.4f;               // Yerde kaç saniye kalacak
    public float hasarAraligi = 0.4f;       // Üzerinde duran düþmana kaç saniyede bir vuracak

    private Dictionary<Collider2D, float> sonHasarZamanlari = new Dictionary<Collider2D, float>();

    void Start()
    {
        // Belirlenen süre sonra iz kendiliðinden silinir
        Destroy(gameObject, omur);
    }

    public void Baslat(float izHasari)
    {
        hasar = izHasari;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            if (!sonHasarZamanlari.ContainsKey(col) || Time.time >= sonHasarZamanlari[col] + hasarAraligi)
            {
                sonHasarZamanlari[col] = Time.time;

                // 1. Evrensel Arayüz (IDamageable)
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(hasar, Vector2.zero);
                    return;
                }

                // 2. Dash Düþmaný
                DashEnemyAI dash = col.GetComponent<DashEnemyAI>();
                if (dash != null)
                {
                    dash.TakeDamage(hasar, Vector2.zero);
                    return;
                }

                // 3. Yarasa
                Enemy yarasa = col.GetComponent<Enemy>();
                if (yarasa != null)
                {
                    yarasa.TakeDamage(hasar, Vector2.zero);
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
}