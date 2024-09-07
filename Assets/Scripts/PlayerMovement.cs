using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float rotationSpeed = 700f;  // Speed at which the player moves
    public Animator animator;
    private CharacterController characterController;
    private GameStateManager gameStateManager;
    private float animSpeed;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        float vertical = Input.GetAxis("Vertical"); // W, S, Up Arrow, Down Arrow

        // Determine the movement direction based on input
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Rotate the player towards the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 10);
            animSpeed = 2;
            animator.SetFloat("IdleWalk", Mathf.Lerp(animator.GetFloat("IdleWalk"), animSpeed, Time.deltaTime * 20));
        }
        else
        {
            animSpeed = 0;
            animator.SetFloat("IdleWalk", Mathf.Lerp(animator.GetFloat("IdleWalk"), animSpeed, Time.deltaTime * 20));
        }
    }
}
