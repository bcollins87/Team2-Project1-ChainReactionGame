using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform laserStartPoint;  // The start point of the laser
    public LineRenderer lineRenderer;  // LineRenderer to visualize the laser
    public float laserSpeed = 10f;     // Speed at which the laser extends
    public float maxLaserLength = 100f; // Maximum length of the laser
    public float cooldownTime = 2.0f;  // Cooldown time before the laser can be fired again

    private float cooldownRemaining = 0f;
    private bool isFiring = false;
    private bool isLaserActive = false; // Track if the laser is currently active
    private Vector3 fireDirection;
    private Vector3 currentStartPosition;
    private float currentLaserLength = 0f;
    private int bouncesLeft;
    public int maxBounces = 5; // Maximum number of bounces

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }

    void Update()
    {
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && cooldownRemaining <= 0 && !isLaserActive)
        {
            StartFiring();
        }

        if (isFiring)
        {
            ExtendLaser();
        }

        if (Input.GetKeyUp(KeyCode.Space))
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
        fireDirection = GetMouseDirection();
        lineRenderer.positionCount = 2; // Start with two points for the line
        lineRenderer.SetPosition(0, currentStartPosition); // Set start point
        lineRenderer.SetPosition(1, currentStartPosition); // Initialize end point
        gameManager.FireLaser(); // Notify GameManager that the laser has been fired
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
        isLaserActive = false; // Mark the laser as inactive
        cooldownRemaining = cooldownTime;
        lineRenderer.positionCount = 0; // Clear the laser line

        // Check the game state only after the laser stops firing
        gameManager.CheckGameState();
    }

    Vector3 GetMouseDirection()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit mouseHit, Mathf.Infinity, LayerMask.GetMask("Floor", "Default")))
        {
            Vector3 targetPoint = mouseHit.point;
            targetPoint.y = laserStartPoint.position.y; // Keep the laser at the same y-level as the start point
            return (targetPoint - laserStartPoint.position).normalized;
        }
        return Vector3.zero;
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
}
