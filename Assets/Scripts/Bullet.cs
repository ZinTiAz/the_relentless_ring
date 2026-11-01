using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Tek satırda Yarasa, Okçu, Dash, Tank ve ilerideki tüm Boss'lara hasar verir
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector2 knockbackDir = Vector2.zero;
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
#if UNITY_6000_0_OR_NEWER
                    knockbackDir = rb.linearVelocity.normalized;
#else
                    knockbackDir = rb.velocity.normalized;
#endif
                }

                damageable.TakeDamage(damage, knockbackDir);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}