using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;  // Speed of the player

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");  // Get horizontal input (A/D, Left/Right Arrow)
        float moveVertical = Input.GetAxis("Vertical");      // Get vertical input (W/S, Up/Down Arrow)

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.position += movement * speed * Time.deltaTime;
    }
}
