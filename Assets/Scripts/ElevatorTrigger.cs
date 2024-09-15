using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene. Please ensure a GameManager exists in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Assuming the player has the tag "Player"
        {
            Debug.Log("Player entered the elevator.");
            if (gameManager != null)
            {
                gameManager.PlayerEnteredElevator();  // Notify GameManager to load the next level
            }
            else
            {
                Debug.LogError("GameManager reference is null.");
            }
        }
    }
}
