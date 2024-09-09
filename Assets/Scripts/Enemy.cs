using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;
    private NavMeshAgent agent;
    public Animator animator;
    
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
        gameManager = FindObjectOfType<GameManager>(); // Find GameManager automatically
        agent = GetComponent<NavMeshAgent>();  // NavMeshAgent for movement
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
        
        // Start patrolling
        PatrolToNextWaypoint();
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
        // If there are no waypoints, return early
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned for patrol.");
            return;
        }

        // Check if enemy has reached the current waypoint
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop back to the start
            Debug.Log("Moving to next waypoint: " + currentWaypointIndex);
            PatrolToNextWaypoint();
        }
    }

    void PatrolToNextWaypoint()
    {
        if (waypoints.Length > 0)
        {
            Debug.Log("Setting destination to waypoint: " + currentWaypointIndex);
            agent.SetDestination(waypoints[currentWaypointIndex].position); // Set the NavMeshAgent to move towards the waypoint
        }
    }

    // Detect the player only
    void DetectPlayer()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach (Collider target in targetsInViewRadius)
        {
            // Ensure the detected object is the player
            if (target.CompareTag("Player"))
            {
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                float angleBetweenEnemyAndTarget = Vector3.Angle(transform.forward, directionToTarget);

                // Check if target is within FOV
                if (angleBetweenEnemyAndTarget < fieldOfViewAngle / 2)
                {
                    // Check if there's an obstacle blocking the view
                    if (!Physics.Raycast(transform.position, directionToTarget, Vector3.Distance(transform.position, target.transform.position), obstacleLayer))
                    {
                        playerDetected = true;
                        lastKnownPosition = target.transform.position; // Store the last known position of the detected player
                        isAlerted = true;
                        alertTimer = alertDuration;  // Reset the alert timer
                        agent.SetDestination(lastKnownPosition);  // Move towards the last known position
                        Debug.Log("Player detected! Moving to investigate...");
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
            PatrolToNextWaypoint();  // Return to patrolling after the alert ends
            Debug.Log("Enemy is no longer alerted. Returning to patrol.");
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

        // Play enemy death sound
        AudioManager.Instance.PlaySound(AudioManager.Instance.enemyDeathClip);
    }
}
