using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Referanslar")]
    public PlayerMovement player;

    [Header("1. Eþya: Arc Orb")]
    public GameObject arcOrbPrefab;
    public Button arcOrbButton;
    public TMP_Text arcOrbYaziText;
    private int arcOrbSeviye = 0;
    private ArcOrb aktifOrbScript;

    [Header("2. Eþya: Ash Rupture")]
    public GameObject ashRupturePrefab;
    public Button ashRuptureButton;
    public TMP_Text ashRuptureYaziText;
    private int ashRuptureSeviye = 0;
    private AshRupture aktifRuptureScript;

    [Header("3. Eþya: Gallows Spike")]
    public GameObject spikeSpawnerPrefab;
    public Button spikeButton;
    public TMP_Text spikeYaziText;
    private int spikeSeviye = 0;
    private GallowsSpikeSpawner aktifSpikeScript;

    [Header("4. Eþya: Shroud Pool")]
    public GameObject poolSpawnerPrefab;
    public Button poolButton;
    public TMP_Text poolYaziText;
    private int poolSeviye = 0;
    private ShroudPoolSpawner aktifPoolScript;

    [Header("5. Eþya: Hysteric Cleave")]
    public GameObject cleavePrefab;
    public Button cleaveButton;
    public TMP_Text cleaveYaziText;
    private int cleaveSeviye = 0;
    private HystericCleaveSpawner aktifCleaveScript;

    [Header("6. Eþya: Bleeding Steps")]
    public GameObject stepPrefab;
    public Button stepButton;
    public TMP_Text stepYaziText;
    private int stepSeviye = 0;
    private BleedingStepsSpawner aktifStepScript;

    [Header("7. Eþya: Leave Me Alone")]
    public GameObject leaveMeAlonePrefab;
    public Button leaveMeAloneButton;
    public TMP_Text leaveMeAloneYaziText;
    private int leaveMeAloneSeviye = 0;
    private LeaveMeAlone aktifLeaveMeAloneScript;

    [Header("8. Eþya: Shadow Companion")]
    public GameObject shadowPrefab;
    public Button shadowButton;
    public TMP_Text shadowYaziText;
    private int shadowSeviye = 0;
    private ShadowCompanion aktifShadowScript;

    [Header("9. Eþya: Echo of Mistakes")]
    public GameObject mistakesPrefab;
    public Button mistakesButton;
    public TMP_Text mistakesYaziText;
    private int mistakesSeviye = 0;
    private EchoOfMistakesSpawner aktifMistakesScript;

    [Header("10. Eþya: Don't Die For Me")]
    public GameObject dontDiePrefab;
    public Button dontDieButton;
    public TMP_Text dontDieYaziText;
    private int dontDieSeviye = 0;
    private DontDieForMeSpawner aktifDontDieScript;

    [Header("11. Eþya: Hayatta Kalanýn Azabý (Survivor's Guilt)")]
    public Button guiltButton;
    public TMP_Text guiltYaziText;
    private int guiltSeviye = 0;
    private SurvivorGuiltSpawner aktifGuiltScript;

    [Header("12. Eþya: Yolun Sonu (End of the Road)")]
    public GameObject endOfRoadPrefab;
    public Button endOfRoadButton;
    public TMP_Text endOfRoadYaziText;
    private int endOfRoadSeviye = 0;
    private EndOfTheRoadSpawner aktifEndOfRoadScript;

    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerMovement>();

        GuncelleUI();
    }

    public void ArcOrbSatinAl()
    {
        if (arcOrbSeviye >= 3) return;
        int maliyet = (arcOrbSeviye == 0) ? 10 : (arcOrbSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            arcOrbSeviye++;
            if (arcOrbSeviye == 1)
            {
                GameObject orbObj = Instantiate(arcOrbPrefab, player.transform.position + Vector3.right * 2f, Quaternion.identity);
                aktifOrbScript = orbObj.GetComponent<ArcOrb>();
                if (aktifOrbScript != null)
                {
                    aktifOrbScript.oyuncu = player.transform;
                    aktifOrbScript.SeviyeGuncelle(arcOrbSeviye);
                }
            }
            else if (aktifOrbScript != null)
            {
                aktifOrbScript.SeviyeGuncelle(arcOrbSeviye);
            }
            GuncelleUI();
        }
    }

    public void AshRuptureSatinAl()
    {
        if (ashRuptureSeviye >= 3) return;
        int maliyet = (ashRuptureSeviye == 0) ? 10 : (ashRuptureSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            ashRuptureSeviye++;
            if (ashRuptureSeviye == 1)
            {
                GameObject ruptureObj = Instantiate(ashRupturePrefab, player.transform.position, Quaternion.identity);
                aktifRuptureScript = ruptureObj.GetComponent<AshRupture>();
                if (aktifRuptureScript != null)
                {
                    aktifRuptureScript.oyuncu = player.transform;
                    aktifRuptureScript.SeviyeGuncelle(ashRuptureSeviye);
                }
            }
            else if (aktifRuptureScript != null)
            {
                aktifRuptureScript.SeviyeGuncelle(ashRuptureSeviye);
            }
            GuncelleUI();
        }
    }

    public void SpikeSatinAl()
    {
        if (spikeSeviye >= 3) return;
        int maliyet = (spikeSeviye == 0) ? 10 : (spikeSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            spikeSeviye++;
            if (spikeSeviye == 1)
            {
                GameObject spawnerObj = Instantiate(spikeSpawnerPrefab, player.transform.position, Quaternion.identity);
                aktifSpikeScript = spawnerObj.GetComponent<GallowsSpikeSpawner>();
                if (aktifSpikeScript != null)
                {
                    aktifSpikeScript.oyuncu = player.transform;
                    aktifSpikeScript.SeviyeGuncelle(spikeSeviye);
                }
            }
            else if (aktifSpikeScript != null)
            {
                aktifSpikeScript.SeviyeGuncelle(spikeSeviye);
            }
            GuncelleUI();
        }
    }

    public void ShroudPoolSatinAl()
    {
        if (poolSeviye >= 3) return;
        int maliyet = (poolSeviye == 0) ? 10 : (poolSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            poolSeviye++;
            if (poolSeviye == 1)
            {
                aktifPoolScript = player.gameObject.AddComponent<ShroudPoolSpawner>();
                aktifPoolScript.poolPrefab = poolSpawnerPrefab;
                aktifPoolScript.SeviyeGuncelle(poolSeviye);
            }
            else if (aktifPoolScript != null)
            {
                aktifPoolScript.SeviyeGuncelle(poolSeviye);
            }
            GuncelleUI();
        }
    }

    public void HystericCleaveSatinAl()
    {
        if (cleaveSeviye >= 3) return;
        int maliyet = (cleaveSeviye == 0) ? 10 : (cleaveSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            cleaveSeviye++;
            if (cleaveSeviye == 1)
            {
                aktifCleaveScript = player.gameObject.AddComponent<HystericCleaveSpawner>();
                aktifCleaveScript.cleavePrefab = cleavePrefab;
                aktifCleaveScript.SeviyeGuncelle(cleaveSeviye);
            }
            else if (aktifCleaveScript != null)
            {
                aktifCleaveScript.SeviyeGuncelle(cleaveSeviye);
            }
            GuncelleUI();
        }
    }

    public void BleedingStepsSatinAl()
    {
        if (stepSeviye >= 3) return;
        int maliyet = (stepSeviye == 0) ? 10 : (stepSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            stepSeviye++;
            if (stepSeviye == 1)
            {
                aktifStepScript = player.gameObject.AddComponent<BleedingStepsSpawner>();
                aktifStepScript.stepPrefab = stepPrefab;
                aktifStepScript.SeviyeGuncelle(stepSeviye);
            }
            else if (aktifStepScript != null)
            {
                aktifStepScript.SeviyeGuncelle(stepSeviye);
            }
            GuncelleUI();
        }
    }

    public void LeaveMeAloneSatinAl()
    {
        if (leaveMeAloneSeviye >= 3) return;
        int maliyet = (leaveMeAloneSeviye == 0) ? 10 : (leaveMeAloneSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            leaveMeAloneSeviye++;
            if (leaveMeAloneSeviye == 1)
            {
                GameObject auraObj = Instantiate(leaveMeAlonePrefab, player.transform.position, Quaternion.identity);
                aktifLeaveMeAloneScript = auraObj.GetComponent<LeaveMeAlone>();
                if (aktifLeaveMeAloneScript != null)
                {
                    aktifLeaveMeAloneScript.oyuncu = player.transform;
                    aktifLeaveMeAloneScript.SeviyeGuncelle(leaveMeAloneSeviye);
                }
            }
            else if (aktifLeaveMeAloneScript != null)
            {
                aktifLeaveMeAloneScript.SeviyeGuncelle(leaveMeAloneSeviye);
            }
            GuncelleUI();
        }
    }

    public void ShadowCompanionSatinAl()
    {
        if (shadowSeviye >= 3) return;
        int maliyet = (shadowSeviye == 0) ? 10 : (shadowSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            shadowSeviye++;
            if (shadowSeviye == 1)
            {
                GameObject shadowObj = Instantiate(shadowPrefab, player.transform.position, Quaternion.identity);
                aktifShadowScript = shadowObj.GetComponent<ShadowCompanion>();
                if (aktifShadowScript != null)
                {
                    aktifShadowScript.oyuncu = player.transform;
                    aktifShadowScript.SeviyeGuncelle(shadowSeviye);
                }
            }
            else if (aktifShadowScript != null)
            {
                aktifShadowScript.SeviyeGuncelle(shadowSeviye);
            }
            GuncelleUI();
        }
    }

    public void EchoOfMistakesSatinAl()
    {
        if (mistakesSeviye >= 3) return;
        int maliyet = (mistakesSeviye == 0) ? 10 : (mistakesSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            mistakesSeviye++;
            if (mistakesSeviye == 1)
            {
                aktifMistakesScript = player.gameObject.AddComponent<EchoOfMistakesSpawner>();
                aktifMistakesScript.mistakePrefab = mistakesPrefab;
                aktifMistakesScript.SeviyeGuncelle(mistakesSeviye);
            }
            else if (aktifMistakesScript != null)
            {
                aktifMistakesScript.SeviyeGuncelle(mistakesSeviye);
            }
            GuncelleUI();
        }
    }

    public void DontDieForMeSatinAl()
    {
        if (dontDieSeviye >= 3) return;
        int maliyet = (dontDieSeviye == 0) ? 10 : (dontDieSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            dontDieSeviye++;
            if (dontDieSeviye == 1)
            {
                aktifDontDieScript = player.gameObject.AddComponent<DontDieForMeSpawner>();
                aktifDontDieScript.soulPrefab = dontDiePrefab;
                aktifDontDieScript.SeviyeGuncelle(dontDieSeviye);
            }
            else if (aktifDontDieScript != null)
            {
                aktifDontDieScript.SeviyeGuncelle(dontDieSeviye);
            }
            GuncelleUI();
        }
    }

    public void SurvivorGuiltSatinAl()
    {
        if (guiltSeviye >= 3) return;
        int maliyet = (guiltSeviye == 0) ? 10 : (guiltSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            guiltSeviye++;
            if (guiltSeviye == 1)
            {
                aktifGuiltScript = player.gameObject.AddComponent<SurvivorGuiltSpawner>();
                aktifGuiltScript.SeviyeGuncelle(guiltSeviye);
            }
            else if (aktifGuiltScript != null)
            {
                aktifGuiltScript.SeviyeGuncelle(guiltSeviye);
            }
            GuncelleUI();
        }
    }

    public void EndOfTheRoadSatinAl()
    {
        if (endOfRoadSeviye >= 3) return;
        int maliyet = (endOfRoadSeviye == 0) ? 10 : (endOfRoadSeviye == 1 ? 15 : 20);

        if (player != null && player.SpendGold(maliyet))
        {
            endOfRoadSeviye++;
            if (endOfRoadSeviye == 1)
            {
                aktifEndOfRoadScript = player.gameObject.AddComponent<EndOfTheRoadSpawner>();
                aktifEndOfRoadScript.projectilePrefab = endOfRoadPrefab;
                aktifEndOfRoadScript.SeviyeGuncelle(endOfRoadSeviye);
            }
            else if (aktifEndOfRoadScript != null)
            {
                aktifEndOfRoadScript.SeviyeGuncelle(endOfRoadSeviye);
            }
            GuncelleUI();
        }
    }

    void GuncelleUI()
    {
        if (arcOrbYaziText != null)
        {
            int maliyet = (arcOrbSeviye == 0) ? 10 : (arcOrbSeviye == 1 ? 15 : 20);
            if (arcOrbSeviye >= 3) { arcOrbYaziText.text = "Arc Orb\n(MAKS)"; if (arcOrbButton != null) arcOrbButton.interactable = false; }
            else if (arcOrbSeviye == 0) arcOrbYaziText.text = "Arc Orb\n(" + maliyet + " Altýn)";
            else arcOrbYaziText.text = "Arc Orb (Sev " + (arcOrbSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (ashRuptureYaziText != null)
        {
            int maliyet = (ashRuptureSeviye == 0) ? 10 : (ashRuptureSeviye == 1 ? 15 : 20);
            if (ashRuptureSeviye >= 3) { ashRuptureYaziText.text = "Kül Patlamasý\n(MAKS)"; if (ashRuptureButton != null) ashRuptureButton.interactable = false; }
            else if (ashRuptureSeviye == 0) ashRuptureYaziText.text = "Kül Patlamasý\n(" + maliyet + " Altýn)";
            else ashRuptureYaziText.text = "Kül Patlamasý (Sev " + (ashRuptureSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (spikeYaziText != null)
        {
            int maliyet = (spikeSeviye == 0) ? 10 : (spikeSeviye == 1 ? 15 : 20);
            if (spikeSeviye >= 3) { spikeYaziText.text = "Ýnfaz Kazýðý\n(MAKS)"; if (spikeButton != null) spikeButton.interactable = false; }
            else if (spikeSeviye == 0) spikeYaziText.text = "Ýnfaz Kazýðý\n(" + maliyet + " Altýn)";
            else spikeYaziText.text = "Ýnfaz Kazýðý (Sev " + (spikeSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (poolYaziText != null)
        {
            int maliyet = (poolSeviye == 0) ? 10 : (poolSeviye == 1 ? 15 : 20);
            if (poolSeviye >= 3) { poolYaziText.text = "Kefen Havuzu\n(MAKS)"; if (poolButton != null) poolButton.interactable = false; }
            else if (poolSeviye == 0) poolYaziText.text = "Kefen Havuzu\n(" + maliyet + " Altýn)";
            else poolYaziText.text = "Kefen Havuzu (Sev " + (poolSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (cleaveYaziText != null)
        {
            int maliyet = (cleaveSeviye == 0) ? 10 : (cleaveSeviye == 1 ? 15 : 20);
            if (cleaveSeviye >= 3) { cleaveYaziText.text = "Histerik Feryat\n(MAKS)"; if (cleaveButton != null) cleaveButton.interactable = false; }
            else if (cleaveSeviye == 0) cleaveYaziText.text = "Histerik Feryat\n(" + maliyet + " Altýn)";
            else cleaveYaziText.text = "Histerik Feryat (Sev " + (cleaveSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (stepYaziText != null)
        {
            int maliyet = (stepSeviye == 0) ? 10 : (stepSeviye == 1 ? 15 : 20);
            if (stepSeviye >= 3) { stepYaziText.text = "Kanayan Adýmlar\n(MAKS)"; if (stepButton != null) stepButton.interactable = false; }
            else if (stepSeviye == 0) stepYaziText.text = "Kanayan Adýmlar\n(" + maliyet + " Altýn)";
            else stepYaziText.text = "Kanayan Adýmlar (Sev " + (stepSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (leaveMeAloneYaziText != null)
        {
            int maliyet = (leaveMeAloneSeviye == 0) ? 10 : (leaveMeAloneSeviye == 1 ? 15 : 20);
            if (leaveMeAloneSeviye >= 3) { leaveMeAloneYaziText.text = "Beni Yalnýz Býrak\n(MAKS)"; if (leaveMeAloneButton != null) leaveMeAloneButton.interactable = false; }
            else if (leaveMeAloneSeviye == 0) leaveMeAloneYaziText.text = "Beni Yalnýz Býrak\n(" + maliyet + " Altýn)";
            else leaveMeAloneYaziText.text = "Beni Yalnýz Býrak (Sev " + (leaveMeAloneSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (shadowYaziText != null)
        {
            int maliyet = (shadowSeviye == 0) ? 10 : (shadowSeviye == 1 ? 15 : 20);
            if (shadowSeviye >= 3) { shadowYaziText.text = "Gölge Yoldaþ\n(MAKS)"; if (shadowButton != null) shadowButton.interactable = false; }
            else if (shadowSeviye == 0) shadowYaziText.text = "Gölge Yoldaþ\n(" + maliyet + " Altýn)";
            else shadowYaziText.text = "Gölge Yoldaþ (Sev " + (shadowSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (mistakesYaziText != null)
        {
            int maliyet = (mistakesSeviye == 0) ? 10 : (mistakesSeviye == 1 ? 15 : 20);
            if (mistakesSeviye >= 3) { mistakesYaziText.text = "Hatalarýn Dönüþü\n(MAKS)"; if (mistakesButton != null) mistakesButton.interactable = false; }
            else if (mistakesSeviye == 0) mistakesYaziText.text = "Hatalarýn Dönüþü\n(" + maliyet + " Altýn)";
            else mistakesYaziText.text = "Hatalarýn Dönüþü (Sev " + (mistakesSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (dontDieYaziText != null)
        {
            int maliyet = (dontDieSeviye == 0) ? 10 : (dontDieSeviye == 1 ? 15 : 20);
            if (dontDieSeviye >= 3) { dontDieYaziText.text = "Benim Ýçin Ölme\n(MAKS)"; if (dontDieButton != null) dontDieButton.interactable = false; }
            else if (dontDieSeviye == 0) dontDieYaziText.text = "Benim Ýçin Ölme\n(" + maliyet + " Altýn)";
            else dontDieYaziText.text = "Benim Ýçin Ölme (Sev " + (dontDieSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (guiltYaziText != null)
        {
            int maliyet = (guiltSeviye == 0) ? 10 : (guiltSeviye == 1 ? 15 : 20);
            if (guiltSeviye >= 3) { guiltYaziText.text = "Vicdan Baðý\n(MAKS)"; if (guiltButton != null) guiltButton.interactable = false; }
            else if (guiltSeviye == 0) guiltYaziText.text = "Vicdan Baðý\n(" + maliyet + " Altýn)";
            else guiltYaziText.text = "Vicdan Baðý (Sev " + (guiltSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }

        if (endOfRoadYaziText != null)
        {
            int maliyet = (endOfRoadSeviye == 0) ? 10 : (endOfRoadSeviye == 1 ? 15 : 20);
            if (endOfRoadSeviye >= 3) { endOfRoadYaziText.text = "Yolun Sonu\n(MAKS)"; if (endOfRoadButton != null) endOfRoadButton.interactable = false; }
            else if (endOfRoadSeviye == 0) endOfRoadYaziText.text = "Yolun Sonu\n(" + maliyet + " Altýn)";
            else endOfRoadYaziText.text = "Yolun Sonu (Sev " + (endOfRoadSeviye + 1) + ")\n(" + maliyet + " Altýn)";
        }
    }
}