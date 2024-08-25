using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Player's transform
    public Vector3 offset;    // Offset from the player (e.g., new Vector3(0, 10, 0))

    void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}
