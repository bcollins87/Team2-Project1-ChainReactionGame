using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTut : MonoBehaviour
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
    private bool isLaserVisible = false; // Track if the laser is currently visible for playtesting
    private Vector3 fireDirection;
    private Vector3 currentStartPosition;
    private float currentLaserLength = 0f;
    private int bouncesLeft;
    private int availableShots = 3;     // Initial number of available shots

    private GameManager gameManager;
    private MirrorPlacement mirrorPlacement;
    public AudioManager audioManager;  // Reference to AudioManager
    public Animator animator;

    public PlayerCollisionsHELPSCREEN playerCollisionsHELPSCREEN;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mirrorPlacement = FindObjectOfType<MirrorPlacement>();
        audioManager = FindObjectOfType<AudioManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
        if (mirrorPlacement == null)
        {
            Debug.LogError("MirrorPlacement script not found in the scene!");
        }
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene!");
        }
    }

    void Update()
    {
        // Cooldown timer
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining <= 0)
            {
                isLaserActive = false; // Allow laser to be active again after cooldown
            }
        }

        // Toggle laser visibility for playtesting
        if (playerCollisionsHELPSCREEN.laserTrace == true)
        {
            Debug.Log("Trace Visible");
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("L Pressed");
                ToggleLaserVisibility();
            }
            // Display laser for visualization without firing
            if (isLaserVisible && !isFiring)
                {
                    VisualizeLaser();
                }

            if (playerCollisionsHELPSCREEN.shootLaser == true)
            {
                // Start firing only if not currently active, cooldown is finished, not placing a mirror, and shots are available
                if (Input.GetKeyDown(KeyCode.Space) && cooldownRemaining <= 0 && !isFiring && availableShots > 0 && (mirrorPlacement == null || !mirrorPlacement.IsPlacingMirror))
                {
                    Invoke("StartFiring", .75f);
                    animator.SetTrigger("signalStrike");
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
        }
    }

    void StartFiring()
    {
        isFiring = true;
        isLaserActive = true; // Mark the laser as active
        currentLaserLength = 0f;
        bouncesLeft = maxBounces;
        currentStartPosition = laserStartPoint.position;
        fireDirection = laserStartPoint.forward; // Use the laserStartPoint's forward direction
        lineRenderer.positionCount = 2; // Start with two points for the line
        lineRenderer.SetPosition(0, currentStartPosition); // Set start point
        lineRenderer.SetPosition(1, currentStartPosition); // Initialize end point
        gameManager.FireLaser(); // Notify GameManager that the laser has been fired
        availableShots--; // Decrease the number of available shots

        // Play laser firing sound
        if (audioManager != null && audioManager.laserShotClip != null)
        {
            audioManager.PlaySound(audioManager.laserShotClip);
        }
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

        // Ensure the position count is enough to handle the new position before setting it
        if (lineRenderer.positionCount < 2)
        {
            lineRenderer.positionCount = 2;  // Ensure the line renderer has at least 2 positions
        }

        lineRenderer.SetPosition(1, newEndPosition);  // Set the end position of the laser

        if (currentLaserLength >= maxLaserLength)
        {
            StopFiring();
        }
    }


    void StopFiring()
    {
        isFiring = false;
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0; // Clear the laser line properly
        }

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

            // Play laser bounce sound
            if (audioManager != null && audioManager.laserBounceClip != null)
            {
                audioManager.PlaySound(audioManager.laserBounceClip);
            }
        }
        else if (hit.collider.CompareTag("Enemy"))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage();  // Kill the enemy

                // Stop firing the laser immediately after hitting an enemy
                StopFiring();
            }
        }
        else if (hit.collider.CompareTag("Glass"))
        {
            // Continue through the glass without altering the laser path
            currentStartPosition = hit.point + fireDirection * 0.01f; // Continue laser slightly past the glass
            currentLaserLength = 0f; // Reset length to continue extending
            lineRenderer.SetPosition(0, currentStartPosition); // Update the line renderer start position
        }
        else
        {
            StopFiring(); // Stop firing if it hits any other object
        }
    }

    void ToggleLaserVisibility()
    {
        isLaserVisible = !isLaserVisible;
        if (!isLaserVisible)
        {
            lineRenderer.positionCount = 0; // Hide the laser line if visibility is toggled off
        }
    }

    void VisualizeLaser()
    {
        // Initialize starting position and direction
        currentStartPosition = laserStartPoint.position;
        fireDirection = laserStartPoint.forward;
        currentLaserLength = 0f;  // Reset the laser length for visualization
        bouncesLeft = maxBounces; // Reset the number of bounces allowed

        // Initialize the line renderer to start at the laser's start point
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentStartPosition);

        int positionIndex = 1;  // Index for positions in the line renderer

        // Iterate as long as there are bounces left and the laser hasn't reached its max length
        while (bouncesLeft > 0 && currentLaserLength < maxLaserLength)
        {
            Ray ray = new Ray(currentStartPosition, fireDirection);  // Ray from the current start position in the fire direction
            RaycastHit hit;

            // Check for any objects in the path of the laser
            if (Physics.Raycast(ray, out hit, maxLaserLength - currentLaserLength))
            {
                // Increase the line renderer's position count and set the position at the hit point
                lineRenderer.positionCount = positionIndex + 1;
                lineRenderer.SetPosition(positionIndex, hit.point);
                positionIndex++;

                // If the laser hits a mirror, reflect the direction and continue
                if (hit.collider.CompareTag("Mirror"))
                {
                    fireDirection = Vector3.Reflect(fireDirection, hit.normal);  // Reflect the laser's direction
                    currentStartPosition = hit.point + fireDirection * 0.01f;    // Slightly offset to avoid re-triggering the same mirror
                    bouncesLeft--;  // Decrease the number of allowed bounces
                }
                // If the laser hits glass, continue through it without altering the direction
                else if (hit.collider.CompareTag("Glass"))
                {
                    // Continue the laser slightly past the glass and reset length
                    currentLaserLength += Vector3.Distance(currentStartPosition, hit.point);
                    currentStartPosition = hit.point + fireDirection * 0.01f;  // Move the start point slightly past the glass
                }
                else
                {
                    // Stop if the laser hits any other object
                    break;
                }
            }
            else
            {
                // If no object is hit, extend the laser to its maximum length
                lineRenderer.positionCount = positionIndex + 1;
                lineRenderer.SetPosition(positionIndex, currentStartPosition + fireDirection * (maxLaserLength - currentLaserLength));
                break;
            }

            // Update the current laser length
            currentLaserLength += Vector3.Distance(lineRenderer.GetPosition(positionIndex - 2), lineRenderer.GetPosition(positionIndex - 1));
        }
    }
}
