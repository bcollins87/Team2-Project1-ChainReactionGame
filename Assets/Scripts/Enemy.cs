using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void TakeDamage()
    {
        Die();
    }

    private void Die()
    {
        // Notify the GameManager that this enemy has been killed
        GameManager.instance.EnemyKilled();

        // Disable or destroy the enemy object
        // If you want to reuse the object (e.g., with object pooling), use SetActive(false)
        // Otherwise, you can destroy it completely
        Destroy(gameObject);
        // Alternatively, use gameObject.SetActive(false);
    }
}
