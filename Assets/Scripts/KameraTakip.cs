using UnityEngine;

public class KameraTakip : MonoBehaviour
{
    public Transform hedef;
    public float takipHizi = 5f;
    public Vector3 mesafe = new Vector3(0f, 0f, -10f);

    void LateUpdate()
    {
        if (hedef != null)
        {
            Vector3 hedefPozisyon = hedef.position + mesafe;
            transform.position = Vector3.Lerp(transform.position, hedefPozisyon, takipHizi * Time.deltaTime);
        }
    }
}