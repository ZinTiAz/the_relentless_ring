using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [Header("Paneller ve Referanslar")]
    public GameObject levelUpPanel;
    public PlayerMovement player;

    [System.Serializable]
    public class UpgradeCard
    {
        public string kartAdi;
        [TextArea] public string aciklama;
        public UpgradeType tip;
    }

    public enum UpgradeType
    {
        KanYemini,       // Hasar +%15
        GolgeAdimi,      // Hareket Hizi +%10
        KirikHalka,      // Saldiri Hizi +%12
        KaranliginKalbi, // Maksimum Can +20
        BoslugunGozu,    // XP/Altin Toplama Menzili (Bilgi)
        SonNefes,        // %25 Can Yenileme
        ZincirliRuh,     // Saldiri Bekleme Suresi Azaltma
        KayipRuh         // XP Kazanci +%15
    }

    [Header("3 Butonun UI Referansları")]
    public Button[] secimButonlari;       // 3 Buton
    public TMP_Text[] kartBaslikTextleri;  // 3 Butonun Başlık Yazıları
    public TMP_Text[] kartAciklamaTextleri;// 3 Butonun Açıklama Yazıları

    private List<UpgradeCard> tumKartlar = new List<UpgradeCard>();
    private UpgradeCard[] suankiSecenekler = new UpgradeCard[3];

    void Awake()
    {
        KartlariOlustur();
    }

    void KartlariOlustur()
    {
        tumKartlar.Add(new UpgradeCard { kartAdi = "Kan Yemini", aciklama = "Mermi ve yakin dovus hasari +%15 artar.", tip = UpgradeType.KanYemini });
        tumKartlar.Add(new UpgradeCard { kartAdi = "Gölge Adımı", aciklama = "Hareket hizi +%10 artar.", tip = UpgradeType.GolgeAdimi });
        tumKartlar.Add(new UpgradeCard { kartAdi = "Kırık Halka", aciklama = "Saldiri hizi +%12 artar (ates araligi duser).", tip = UpgradeType.KirikHalka });
        tumKartlar.Add(new UpgradeCard { kartAdi = "Karanlığın Kalbi", aciklama = "Maksimum Can +20 artar ve can doldurur.", tip = UpgradeType.KaranliginKalbi });
        tumKartlar.Add(new UpgradeCard { kartAdi = "Son Nefes", aciklama = "Maksimum caninin %25'ini aninda yeniler.", tip = UpgradeType.SonNefes });
        tumKartlar.Add(new UpgradeCard { kartAdi = "Zincirli Ruh", aciklama = "Yakin dovus bekleme suresi %10 duser.", tip = UpgradeType.ZincirliRuh });
        tumKartlar.Add(new UpgradeCard { kartAdi = "Kayıp Ruh", aciklama = "Gelecek seviyeler icin XP ihtiyacini %15 hafifletir.", tip = UpgradeType.KayipRuh });
    }

    // Seviye atlandığında çağrılır
    public void MenuyuAcVeKartlariSec()
    {
        if (levelUpPanel == null) return;

        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;

        // Rastgele 3 farkli kart sec
        List<UpgradeCard> geciciHavuz = new List<UpgradeCard>(tumKartlar);

        for (int i = 0; i < 3; i++)
        {
            if (geciciHavuz.Count == 0) break;

            int rastgeleIndex = Random.Range(0, geciciHavuz.Count);
            suankiSecenekler[i] = geciciHavuz[rastgeleIndex];
            geciciHavuz.RemoveAt(rastgeleIndex);

            // Buton UI metinlerini guncelle
            if (kartBaslikTextleri.Length > i && kartBaslikTextleri[i] != null)
                kartBaslikTextleri[i].text = suankiSecenekler[i].kartAdi;

            if (kartAciklamaTextleri.Length > i && kartAciklamaTextleri[i] != null)
                kartAciklamaTextleri[i].text = suankiSecenekler[i].aciklama;
        }
    }

    // Buton 1 tiklandiginda
    public void Buton1Secildi() { KartiUygula(suankiSecenekler[0]); }

    // Buton 2 tiklandiginda
    public void Buton2Secildi() { KartiUygula(suankiSecenekler[1]); }

    // Buton 3 tiklandiginda
    public void Buton3Secildi() { KartiUygula(suankiSecenekler[2]); }

    void KartiUygula(UpgradeCard kart)
    {
        if (kart == null || player == null) { DevamEt(); return; }

        switch (kart.tip)
        {
            case UpgradeType.KanYemini:
                player.mermiHasari *= 1.15f;
                player.meleeDamage *= 1.15f;
                break;

            case UpgradeType.GolgeAdimi:
                player.moveSpeed *= 1.10f;
                break;

            case UpgradeType.KirikHalka:
                player.fireRate *= 0.88f; // Bekleme suresi azaldigi icin ates hizi artar
                break;

            case UpgradeType.KaranliginKalbi:
                player.maxHealth += 20;
                player.currentHealth += 20;
                player.TakeDamage(0); // UI bari guncellensin
                break;

            case UpgradeType.SonNefes:
                int iyilesme = Mathf.RoundToInt(player.maxHealth * 0.25f);
                player.currentHealth = Mathf.Min(player.currentHealth + iyilesme, player.maxHealth);
                player.TakeDamage(0);
                break;

            case UpgradeType.ZincirliRuh:
                player.meleeCooldown *= 0.90f;
                break;

            case UpgradeType.KayipRuh:
                player.sonrakiSeviyeXP = Mathf.Max(10, Mathf.RoundToInt(player.sonrakiSeviyeXP * 0.85f));
                if (player.xpSlider != null) player.xpSlider.maxValue = player.sonrakiSeviyeXP;
                break;
        }

        DevamEt();
    }

    void DevamEt()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}