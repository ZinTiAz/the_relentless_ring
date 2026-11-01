using UnityEngine;
using System.Collections.Generic;

public class DontDieForMeSpawner : MonoBehaviour
{
    public GameObject soulPrefab;
    public float cagirmaAraligi = 7f;
    public float hasar = 16f;
    public float patlamaHasari = 25f;
    public float omur = 5.5f;

    private float sayac = 0f;

    void Update()
    {
        sayac -= Time.deltaTime;
        if (sayac <= 0f)
        {
            RuhCagir();
            sayac = cagirmaAraligi;
        }
    }

    void RuhCagir()
    {
        if (soulPrefab == null) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<Transform> canliDusmanlar = new List<Transform>();

        foreach (GameObject e in enemies)
        {
            if (e != null && e.activeInHierarchy)
            {
                Collider2D col = e.GetComponent<Collider2D>();
                if (col != null && col.enabled)
                {
                    canliDusmanlar.Add(e.transform);
                }
            }
        }

        if (canliDusmanlar.Count == 0) return;

        Transform secilenDusman = canliDusmanlar[Random.Range(0, canliDusmanlar.Count)];

        GameObject ruh = Instantiate(soulPrefab, secilenDusman.position, Quaternion.identity);
        DontDieForMe soulScript = ruh.GetComponent<DontDieForMe>();

        SpriteRenderer dusmanSr = secilenDusman.GetComponent<SpriteRenderer>();
        SpriteRenderer ruhSr = ruh.GetComponent<SpriteRenderer>();
        if (dusmanSr != null && ruhSr != null)
        {
            ruhSr.sprite = dusmanSr.sprite;
        }

        if (soulScript != null)
        {
            soulScript.hasar = hasar;
            soulScript.patlamaHasari = patlamaHasari;
            soulScript.omur = omur;
        }
    }

    public void SeviyeGuncelle(int seviye)
    {
        if (seviye == 1)
        {
            cagirmaAraligi = 7f;
            hasar = 16f;
            patlamaHasari = 25f;
            omur = 5.5f;
        }
        else if (seviye == 2)
        {
            cagirmaAraligi = 5.5f;
            hasar = 24f;
            patlamaHasari = 40f;
            omur = 6.5f;
        }
        else if (seviye == 3)
        {
            cagirmaAraligi = 4.2f;
            hasar = 35f;
            patlamaHasari = 60f;
            omur = 7.5f;
        }
    }
}