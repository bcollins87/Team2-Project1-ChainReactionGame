using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Color startColor = new Color(1f, 0.5f, 0.5f); // Light red
    public Color endColor = new Color(0.5f, 0f, 0f);     // Dark red
    public int maxHealth = 5;  // Maximum health (number of hits before death)

    private int currentHealth;
    private Renderer enemyRenderer;
    private GameManager gameManager;

    void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        gameManager = FindObjectOfType<GameManager>(); // Find GameManager automatically
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
        UpdateColor();
    }

    // This method will be called by the laser when it hits the enemy
    public void TakeDamage()
    {
        currentHealth--;
        UpdateColor();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Update the color based on current health
    void UpdateColor()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        enemyRenderer.material.color = Color.Lerp(endColor, startColor, healthPercentage);
    }

    // Handle the enemy's death
    void Die()
    {
        gameManager.EnemyKilled(); // Notify GameManager
        Destroy(gameObject);
    }
}
