using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float damage = 15f; // Boss mermisinin hasarý

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage((int)damage);
            }
            Destroy(gameObject); // Oyuncuya çarpýnca yok olsun
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // Duvara çarpýnca yok olsun
        }
    }
}