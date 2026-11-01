using UnityEngine;
using System.Collections.Generic;

public class EchoOfMistakes : MonoBehaviour
{
    [Header("Mekanik Ayarlarý")]
    public float hasar = 18f;
    public float firlatmaHizi = 12f;
    public float donusHizi = 14f;
    public float maksimumMesafe = 6.5f;
    public float donmeHizi = 720f;

    [HideInInspector] public Transform oyuncu;
    [HideInInspector] public Vector2 firlatmaYonu;

    private Vector2 baslangicNoktasi;
    private bool geriDonuyor = false;
    private HashSet<Collider2D> vurulanGidis = new HashSet<Collider2D>();
    private HashSet<Collider2D> vurulanDonus = new HashSet<Collider2D>();

    void Start()
    {
        baslangicNoktasi = transform.position;
        if (firlatmaYonu == Vector2.zero) firlatmaYonu = Vector2.right;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, -donmeHizi * Time.deltaTime);

        if (!geriDonuyor)
        {
            transform.Translate(firlatmaYonu * firlatmaHizi * Time.deltaTime, Space.World);

            if (Vector2.Distance(baslangicNoktasi, transform.position) >= maksimumMesafe)
            {
                geriDonuyor = true;
            }
        }
        else
        {
            if (oyuncu != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, oyuncu.position, donusHizi * Time.deltaTime);

                if (Vector2.Distance(transform.position, oyuncu.position) <= 0.5f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            if (!geriDonuyor)
            {
                if (vurulanGidis.Contains(col)) return;
                vurulanGidis.Add(col);
            }
            else
            {
                if (vurulanDonus.Contains(col)) return;
                vurulanDonus.Add(col);
            }

            Vector2 vurusYonu = ((Vector2)col.transform.position - (Vector2)transform.position).normalized;

            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(hasar, vurusYonu * 2f);
                return;
            }

            DashEnemyAI dash = col.GetComponent<DashEnemyAI>();
            if (dash != null)
            {
                dash.TakeDamage(hasar, vurusYonu * 2f);
                return;
            }

            Enemy yarasa = col.GetComponent<Enemy>();
            if (yarasa != null)
            {
                yarasa.TakeDamage(hasar, vurusYonu * 2f);
                return;
            }

            OkcuAI okcu = col.GetComponent<OkcuAI>();
            if (okcu != null)
            {
                okcu.TakeDamage(hasar);
                return;
            }
        }
    }
}