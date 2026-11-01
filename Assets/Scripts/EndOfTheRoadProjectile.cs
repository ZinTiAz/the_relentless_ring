using UnityEngine;
using System.Collections.Generic;

public class EndOfTheRoadProjectile : MonoBehaviour
{
    public float hasar = 30f;
    public float hiz = 16f;
    public float omur = 2.5f;

    private Vector2 hareketYonu;
    private HashSet<Collider2D> vurulanlar = new HashSet<Collider2D>();

    public void Firlat(Vector2 yon, float mermiHasari)
    {
        hareketYonu = yon.normalized;
        hasar = mermiHasari;

        // Görselin yönünü hareket doðrultusuna çevir
        float aci = Mathf.Atan2(hareketYonu.y, hareketYonu.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, aci);

        Destroy(gameObject, omur);
    }

    void Update()
    {
        transform.position += (Vector3)(hareketYonu * hiz * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !vurulanlar.Contains(col))
        {
            vurulanlar.Add(col); // Ayný düþmana bir kez vursun, delip devam etsin
            HasarVer(col.transform);
        }
    }

    void HasarVer(Transform dusman)
    {
        Vector2 vurusYonu = hareketYonu;

        IDamageable d = dusman.GetComponent<IDamageable>();
        if (d != null) { d.TakeDamage(hasar, vurusYonu * 3f); return; }

        DashEnemyAI dash = dusman.GetComponent<DashEnemyAI>();
        if (dash != null) { dash.TakeDamage(hasar, vurusYonu * 3f); return; }

        Enemy yarasa = dusman.GetComponent<Enemy>();
        if (yarasa != null) { yarasa.TakeDamage(hasar, vurusYonu * 3f); return; }

        OkcuAI okcu = dusman.GetComponent<OkcuAI>();
        if (okcu != null) { okcu.TakeDamage(hasar); return; }
    }
}