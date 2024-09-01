using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform player;           // Reference to the player's transform
    public Transform laserStart;       // Reference to the laser's starting position
    public Vector3 offset;             // Offset from the player
    public float panDuration = 2f;     // Duration of the camera pan
    public float pauseDuration = 1f;   // Duration to pause at the laser position

    private Vector3 initialPosition;   // Initial position of the camera
    private bool isPanning = true;     // Flag to indicate if the camera is currently panning

    void Start()
    {
        initialPosition = transform.position; // Store the initial camera position

        // Start the camera pan sequence at the beginning of the level
        StartCoroutine(CameraPanSequence());
    }

    void LateUpdate()
    {
        if (!isPanning)
        {
            // Follow the player if not panning
            transform.position = player.position + offset;
        }
    }

    IEnumerator CameraPanSequence()
    {
        GameStateManager.Instance.SetPanning(true); // Set game to panning state

        // Pan to the laser's starting position
        yield return StartCoroutine(PanToPosition(laserStart.position + offset, panDuration));

        // Pause at the laser's position
        yield return new WaitForSeconds(pauseDuration);

        // Pan back to the player
        yield return StartCoroutine(PanToPosition(player.position + offset, panDuration));

        GameStateManager.Instance.SetPanning(false); // Set game back to normal state
        isPanning = false; // Local flag for camera behavior
    }

    IEnumerator PanToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure the camera reaches the target position
    }
}
