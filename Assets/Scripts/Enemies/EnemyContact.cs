using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockback = 3f;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        PlayerHealth health = col.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
            health.TakeDamage(damage);

        Rigidbody2D playerRb = col.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 direction = (col.transform.position - transform.position).normalized;
            playerRb.linearVelocity = direction * knockback;
        }
    }
}