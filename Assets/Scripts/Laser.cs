using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform laserStartPoint;  // The start point of the laser
    public LineRenderer lineRenderer;  // LineRenderer to visualize the laser
    public float laserSpeed = 10f;     // Speed at which the laser extends
    public float maxLaserLength = 100f; // Maximum length of the laser
    public float cooldownTime = 2.0f;  // Cooldown time before the laser can be fired again
    public int maxBounces = 10;        // Maximum number of bounces

    private float cooldownRemaining = 0f;
    private bool isFiring = false;
    private bool isLaserActive = false; // Track if the laser is currently active
    private Vector3 fireDirection;
    private Vector3 currentStartPosition;
    private float currentLaserLength = 0f;
    private int bouncesLeft;
    private int availableShots = 3;     // Initial number of available shots

    private GameManager gameManager;
    private MirrorPlacement mirrorPlacement;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mirrorPlacement = FindObjectOfType<MirrorPlacement>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
        if (mirrorPlacement == null)
        {
            Debug.LogError("MirrorPlacement script not found in the scene!");
        }
    }

    void Update()
    {
        // Check the global game state
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsPanning)
        {
            // Disable laser firing while the game is panning
            return;
        }

        // Cooldown timer
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining <= 0)
            {
                isLaserActive = false; // Allow laser to be active again after cooldown
            }
        }

        // Start firing only if not currently active, cooldown is finished, not placing a mirror, and shots are available
        if (Input.GetKeyDown(KeyCode.Space) && cooldownRemaining <= 0 && !isFiring && availableShots > 0 && (mirrorPlacement == null || !mirrorPlacement.IsPlacingMirror))
        {
            StartFiring();
        }

        if (isFiring)
        {
            ExtendLaser();
        }

        // Stop firing when the space bar is released
        if (Input.GetKeyUp(KeyCode.Space) && isFiring)
        {
            StopFiring();
        }
    }

    void StartFiring()
    {
        isFiring = true;
        isLaserActive = true; // Mark the laser as active
        currentLaserLength = 0f;
        bouncesLeft = maxBounces;
        currentStartPosition = laserStartPoint.position;
        fireDirection = Vector3.back;
        lineRenderer.positionCount = 2; // Start with two points for the line
        lineRenderer.SetPosition(0, currentStartPosition); // Set start point
        lineRenderer.SetPosition(1, currentStartPosition); // Initialize end point
        gameManager.FireLaser(); // Notify GameManager that the laser has been fired
        availableShots--; // Decrease the number of available shots
    }

    void ExtendLaser()
    {
        if (fireDirection == Vector3.zero)
        {
            StopFiring();
            return;
        }

        currentLaserLength += laserSpeed * Time.deltaTime;
        Vector3 newEndPosition = currentStartPosition + fireDirection * currentLaserLength;

        Ray ray = new Ray(currentStartPosition, fireDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, currentLaserLength))
        {
            newEndPosition = hit.point;
            HandleHit(hit);
        }

        lineRenderer.SetPosition(1, newEndPosition);

        if (currentLaserLength >= maxLaserLength)
        {
            StopFiring();
        }
    }

    void StopFiring()
    {
        isFiring = false;
        lineRenderer.positionCount = 0; // Clear the laser line

        // Start cooldown only after firing stops
        if (isLaserActive)
        {
            cooldownRemaining = cooldownTime;
            isLaserActive = false; // Laser is now inactive, cooldown is in effect
        }

        // Check the game state only after the laser stops firing
        gameManager.CheckGameState();
    }

    void HandleHit(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Mirror") && bouncesLeft > 0)
        {
            Vector3 reflectionDirection = Vector3.Reflect(fireDirection, hit.normal);
            currentStartPosition = hit.point + reflectionDirection * 0.01f; // Move the start point to the hit position, offset slightly
            fireDirection = reflectionDirection;
            currentLaserLength = 0f; // Reset length to start extending from the new point
            bouncesLeft--; // Decrement the bounce counter
            lineRenderer.SetPosition(0, currentStartPosition); // Update the line renderer start position
        }
        else if (hit.collider.CompareTag("Enemy"))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage();
                GainExtraShot(); // Add one shot when an enemy is killed
                // Notify GameManager to check win condition after hitting an enemy
                gameManager.EnemyKilled(); // Update to check win condition
            }
            StopFiring(); // Stop firing after hitting an enemy
        }
        else if (hit.collider.CompareTag("Glass"))
        {
            currentStartPosition = hit.point + fireDirection * 0.01f; // Continue laser slightly past the glass
            currentLaserLength = 0f; // Reset length to continue extending
            lineRenderer.SetPosition(0, currentStartPosition); // Update the line renderer start position
        }
        else
        {
            StopFiring(); // Stop firing if it hits any other object
        }
    }

    // Method to gain an extra shot - changed to public
    public void GainExtraShot()
    {
        availableShots++;
        Debug.Log("Gained an extra shot! Total available shots: " + availableShots);
    }
}
