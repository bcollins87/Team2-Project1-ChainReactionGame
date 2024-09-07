using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // Find GameManager automatically

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }

    // This method will be called by the laser when it hits the enemy
    public void TakeDamage()
    {
        Die();
    }

    // Handle the enemy's death
    void Die()
    {
        gameManager.EnemyKilled(); // Notify GameManager
        Destroy(gameObject); // Destroy the enemy object

        // Play enemy death sound
        AudioManager.Instance.PlaySound(AudioManager.Instance.enemyDeathClip);
    }
}
