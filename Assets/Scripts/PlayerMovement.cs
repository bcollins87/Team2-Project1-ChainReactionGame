using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed at which the player moves
    private CharacterController characterController;
    private GameStateManager gameStateManager;

    void Start()
    {
        // Get the CharacterController component
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found on the player.");
        }

        // Find the GameStateManager in the scene
        gameStateManager = FindObjectOfType<GameStateManager>();
        if (gameStateManager == null)
        {
            Debug.LogError("GameStateManager not found in the scene.");
        }
    }

    void Update()
    {
        // Check if the game is currently panning
        if (gameStateManager != null && gameStateManager.IsPanning)
        {
            // If panning, prevent player movement
            return;
        }

        // Get input axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Create movement vector
        Vector3 move = new Vector3(horizontal, 0, vertical);
        move = transform.TransformDirection(move);

        // Apply movement
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }
}
