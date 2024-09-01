using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Color startColor = new Color(1f, 0.5f, 0.5f); // Light red
    public Color endColor = new Color(0.5f, 0f, 0f);     // Dark red
    public int maxHealth = 5;  // Maximum health (number of hits before death)

    private int currentHealth;
    private Renderer enemyRenderer;
    private GameManager gameManager;
    private Laser laserScript; // Reference to the Laser script to grant extra shots

    void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        gameManager = FindObjectOfType<GameManager>(); // Find GameManager automatically
        laserScript = FindObjectOfType<Laser>();       // Find the Laser script in the scene

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
        if (laserScript == null)
        {
            Debug.LogError("Laser script not found in the scene!");
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

        if (laserScript != null)
        {
            laserScript.GainExtraShot(); // Add an extra shot to the Laser script
        }

        Destroy(gameObject);
    }
}
