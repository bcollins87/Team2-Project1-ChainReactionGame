using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float rotationSpeed = 700f;  // Speed at which the player rotates
    public float normalSpeed = 2f;      // Normal walking speed
    public float sprintSpeed = 5f;      // Speed when sprinting
    public Animator animator;
    private CharacterController characterController;
    private GameStateManager gameStateManager;
    public GameManager gameManager;
    private float animSpeed;
    public Animator elevatorAnimator;

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

        // Get input from the player
        float horizontal = Input.GetAxis("Horizontal"); // A, D, Left Arrow, Right Arrow
        float vertical = Input.GetAxis("Vertical");     // W, S, Up Arrow, Down Arrow

        // Determine if the player is sprinting (holding Left Shift)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Adjust movement speed based on sprinting or walking
        float currentSpeed = isSprinting ? sprintSpeed : normalSpeed;

        // Determine the movement direction based on input
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Rotate the player towards the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 10);
            
            // Move the player
          //  Vector3 movement = direction * currentSpeed * Time.deltaTime;
           // characterController.Move(movement);

            // Update animation speed
            animSpeed = isSprinting ? 3 : 2;  // Sprinting has a faster animation speed
            animator.SetFloat("IdleWalk", Mathf.Lerp(animator.GetFloat("IdleWalk"), animSpeed, Time.deltaTime * 20));
        }
        else
        {
            animSpeed = 0;
            animator.SetFloat("IdleWalk", Mathf.Lerp(animator.GetFloat("IdleWalk"), animSpeed, Time.deltaTime * 20));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Elevator"))
        {
            Debug.Log("Trigger Entered");
            elevatorAnimator.SetTrigger("onTriggerEnter");
            gameManager.PlayerEnteredElevator();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Elevator"))
        {
            Debug.Log("Trigger Exited");
            elevatorAnimator.SetTrigger("onTriggerExit");
        }
    }
}
