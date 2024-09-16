using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;
    public AudioManager audioManager;  // Reference to AudioManager
    public Animator animator;
    private Collider collider;

    // Patrol variables
    public Transform[] waypoints;  // Set waypoints in the Inspector
    private int currentWaypointIndex = 0;

    // Detection variables
    public float detectionRadius = 5f;
    public float fieldOfViewAngle = 90f;
    public LayerMask playerLayer;    // Assign in Inspector (layer for player)
    public LayerMask obstacleLayer;  // Obstacles like walls

    private bool playerDetected = false;

    // Alert system
    public float alertDuration = 3f;   // Time enemy stays alert after detecting player
    private bool isAlerted = false;
    private float alertTimer = 0f;
    private Vector3 lastKnownPosition; // Where the enemy detected the player

    void Start()
    {
        collider = GetComponent<Collider>();
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

        // Start patrolling
        PatrolToNextWaypoint();
        animator.SetBool("isAlive", true);
    }

    void Update()
    {
        if (isAlerted)
        {
            AlertMode();
        }
        else
        {
            Patrol();
            DetectPlayer();
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

    // Detect the player
    void DetectPlayer()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach (Collider target in targetsInViewRadius)
        {
            if (target.CompareTag("Player"))
            {
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                float angleBetweenEnemyAndTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleBetweenEnemyAndTarget < fieldOfViewAngle / 2)
                {
                    if (!Physics.Raycast(transform.position, directionToTarget, Vector3.Distance(transform.position, target.transform.position), obstacleLayer))
                    {
                        playerDetected = true;
                        lastKnownPosition = target.transform.position;
                        isAlerted = true;
                        alertTimer = alertDuration;

                        // Play alert sound
                        if (audioManager != null && audioManager.enemyAlert != null)
                        {
                            audioManager.PlaySound(audioManager.enemyAlert);
                        }
                    }
                    else
                    {
                        playerDetected = false;
                    }
                }
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
        collider.enabled = false;

        // Play enemy death sound
        if (audioManager != null && audioManager.enemyDeathClip != null)
        {
            audioManager.PlaySound(audioManager.enemyDeathClip);
        }
    }
}
