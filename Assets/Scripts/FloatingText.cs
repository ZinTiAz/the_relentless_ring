using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float yokOlmaSuresi = 0.8f;
    public float yukselmeHizi = 0.6f;
    private TextMeshPro textMesh;
    private Color alphaColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Kurulum(int hasarMiktari)
    {
        if (textMesh != null)
        {
            textMesh.text = hasarMiktari.ToString();
            alphaColor = textMesh.color;
        }

        Destroy(gameObject, yokOlmaSuresi);
    }

    void Update()
    {
        // Yukarý doðru yavaþça süzülsün
        transform.position += new Vector3(0, yukselmeHizi * Time.deltaTime, 0);

        // Zamanla saydamlaþarak yok olsun
        if (textMesh != null)
        {
            alphaColor.a -= (1f / yokOlmaSuresi) * Time.deltaTime;
            textMesh.color = alphaColor;
        }
    }
}