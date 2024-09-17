using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;
    public AudioManager audioManager;  // Reference to AudioManager
    public Animator animator;
    private Collider enemyCollider;  // Renamed to avoid conflict with inherited 'collider'

    // Patrol variables
    public Transform[] waypoints;  // Set waypoints in the Inspector
    private int currentWaypointIndex = 0;

    // Detection variables
    public float detectionRadius = 5f;
    public float fieldOfViewAngle = 90f;
    public LayerMask playerLayer;    // Assign in Inspector (layer for player)
    public LayerMask obstacleLayer;  // Obstacles like walls

    private bool playerDetected = false;
    private bool isDead = false;     // Track if the enemy is dead

    // Spotlight detection
    public Light playerSpotlight;    // Reference to the player's spotlight (assign in Inspector or find dynamically)
    public float lightDetectionRadius = 7f;  // Radius in which the enemy can detect the player's light

    // Alert system
    public float alertDuration = 3f;   // Time enemy stays alert after detecting player
    private bool isAlerted = false;
    private float alertTimer = 0f;
    private Vector3 lastKnownPosition; // Where the enemy detected the player

    // Light manager for controlling lights
    public LightManager lightManager; // Reference to the LightManager to control lights

    void Start()
    {
        enemyCollider = GetComponent<Collider>();
        gameManager = FindObjectOfType<GameManager>(); // Find GameManager automatically
        audioManager = FindObjectOfType<AudioManager>(); // Find AudioManager automatically

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }

        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene!");
        }

        if (lightManager == null)
        {
            lightManager = FindObjectOfType<LightManager>(); // Automatically find the LightManager if not set
        }

        // Find the player's spotlight if it's not assigned in the inspector
        if (playerSpotlight == null)
        {
            playerSpotlight = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Light>();
        }

        // Start patrolling
        PatrolToNextWaypoint();
        animator.SetBool("isAlive", true);
    }

    void Update()
    {
        // If the enemy is dead, don't allow further detection or alert behavior
        if (isDead) return;

        if (isAlerted)
        {
            AlertMode();
        }
        else
        {
            Patrol();
            DetectPlayerSpotlight(); // Detect the player's spotlight
        }
    }

    // Patrolling logic
    void Patrol()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned for patrol.");
            return;
        }

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop back to the start
        PatrolToNextWaypoint();
    }

    void PatrolToNextWaypoint()
    {
        if (waypoints.Length > 0)
        {
            Debug.Log("Setting destination to waypoint: " + currentWaypointIndex);
        }
    }

    // Detect the player's spotlight
    void DetectPlayerSpotlight()
    {
        if (playerSpotlight == null) return;  // Exit if no spotlight is found

        // Get the position of the spotlight
        Vector3 spotlightPosition = playerSpotlight.transform.position;
        Vector3 directionToLight = (spotlightPosition - transform.position).normalized;
        float distanceToLight = Vector3.Distance(transform.position, spotlightPosition);

        // Check if the spotlight is within the detection radius
        if (distanceToLight <= lightDetectionRadius)
        {
            float angleToLight = Vector3.Angle(transform.forward, directionToLight);

            // Check if the light is within the enemy's field of view
            if (angleToLight < fieldOfViewAngle / 2)
            {
                // Check for obstacles between enemy and the spotlight
                if (!Physics.Raycast(transform.position, directionToLight, distanceToLight, obstacleLayer))
                {
                    playerDetected = true;
                    lastKnownPosition = spotlightPosition;
                    isAlerted = true;
                    alertTimer = alertDuration;

                    Debug.Log("Player's spotlight detected!");

                    // Play alert sound
                    if (audioManager != null && audioManager.enemyAlert != null)
                    {
                        audioManager.PlaySound(audioManager.enemyAlert);
                    }

                    // Turn on lights when the player's spotlight is detected
                    if (lightManager != null)
                    {
                        lightManager.TurnLightsOn();
                        Debug.Log("Spotlight detected, lights turned on!");
                    }
                }
            }
        }

        // If the spotlight is not detected, turn off the lights
        if (!playerDetected && isAlerted)
        {
            if (lightManager != null)
            {
                lightManager.TurnLightsOff();
                Debug.Log("Spotlight not detected, lights turned off!");
            }
        }
    }

    // Alert mode behavior
    void AlertMode()
    {
        alertTimer -= Time.deltaTime;

        if (alertTimer <= 0f)
        {
            isAlerted = false;
            PatrolToNextWaypoint();

            // Turn off lights if the player's spotlight is no longer detected
            if (lightManager != null)
            {
                lightManager.TurnLightsOff();
                Debug.Log("Alert over, lights turned off!");
            }
        }
    }

    // Called when the laser hits the enemy
    public void TakeDamage()
    {
        Die();
    }

    // Handle the enemy's death
    void Die()
    {
        gameManager.EnemyKilled(); // Notify GameManager
        animator.SetBool("isAlive", false);
        enemyCollider.enabled = false;
        isDead = true;  // Mark the enemy as dead to prevent further detection

        // Play enemy death sound
        if (audioManager != null && audioManager.enemyDeathClip != null)
        {
            audioManager.PlaySound(audioManager.enemyDeathClip);
        }
    }
}
