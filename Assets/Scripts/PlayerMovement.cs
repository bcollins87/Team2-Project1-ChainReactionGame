using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;  // Speed of the player

    void Update()
    {
        // Check if the game is in a panning state
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsPanning)
        {
            // If the camera is panning, prevent player movement
            return;
        }

        // Get input for horizontal and vertical movement
        float moveHorizontal = Input.GetAxis("Horizontal");  // Get horizontal input (A/D, Left/Right Arrow)
        float moveVertical = Input.GetAxis("Vertical");      // Get vertical input (W/S, Up/Down Arrow)

        // Calculate movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Apply movement to the player
        transform.position += movement * speed * Time.deltaTime;
    }
}
