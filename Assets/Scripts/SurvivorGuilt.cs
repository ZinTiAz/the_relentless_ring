using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurvivorGuilt : MonoBehaviour
{
    private LineRenderer lr;

    void Awake()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.12f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0.6f, 0.2f, 0.9f, 0.9f); // Kasvetli mor/eflatun
        lr.endColor = new Color(0.2f, 0.8f, 1f, 0.4f);
        lr.sortingOrder = 12;
    }

    public void BagOlustur(List<Vector3> noktalar, float hasar)
    {
        StartCoroutine(ZincirAnimasyonu(noktalar, hasar));
    }

    private IEnumerator ZincirAnimasyonu(List<Vector3> noktalar, float hasar)
    {
        lr.positionCount = noktalar.Count;
        for (int i = 0; i < noktalar.Count; i++)
        {
            lr.SetPosition(i, noktalar[i]);
        }

        // Kýsa süre parlayýp kaybolsun
        float gecen = 0f;
        float sure = 0.25f;
        while (gecen < sure)
        {
            gecen += Time.deltaTime;
            float alfa = Mathf.Lerp(1f, 0f, gecen / sure);
            lr.startColor = new Color(0.6f, 0.2f, 0.9f, alfa);
            lr.endColor = new Color(0.2f, 0.8f, 1f, alfa * 0.5f);
            yield return null;
        }

        Destroy(gameObject);
    }
}