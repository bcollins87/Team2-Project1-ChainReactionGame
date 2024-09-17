using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform laserStartPoint;  // The start point of the laser
    public LineRenderer lineRenderer;  // LineRenderer to visualize the laser
    public float laserSpeed = 10f;     // Speed at which the laser extends
    public float maxLaserLength = 100f; // Maximum length of the laser
    public float cooldownTime = 2.0f;  // Cooldown time before the laser can be fired again
    public int maxBounces = 10;        // Maximum number of bounces

    public Color laserColorOn = Color.red;    // Laser color when active (red)
    public Color laserColorOff = Color.green; // Laser color when lights are off (green)

    private float cooldownRemaining = 0f;
    private bool isFiring = false;
    private bool isLaserActive = false; // Track if the laser is currently active
    private Vector3 fireDirection;
    private Vector3 currentStartPosition;
    private float currentLaserLength = 0f;
    private int bouncesLeft;

    private GameManager gameManager;
    private MirrorPlacement mirrorPlacement;
    private LightManager lightManager;   // Reference to LightManager to check light status
    private AudioManager audioManager;  // Reference to AudioManager

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mirrorPlacement = FindObjectOfType<MirrorPlacement>();
        audioManager = FindObjectOfType<AudioManager>();
        lightManager = FindObjectOfType<LightManager>();  // Find the LightManager

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
        if (lightManager == null)
        {
            Debug.LogError("LightManager not found in the scene!");
        }

        lineRenderer.startColor = laserColorOn;
        lineRenderer.endColor = laserColorOn;
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

        // Determine if the lights are on
        bool lightsOn = lightManager != null && lightManager.AreLightsOn();

        // Check if the player is detected by an enemy
        bool playerDetected = gameManager != null && gameManager.IsPlayerDetected();

        // Manage laser visibility and color based on the lights and player detection
        if (!lightsOn)
        {
            lineRenderer.enabled = true; // Keep laser visible at all times when lights are off
            lineRenderer.startColor = laserColorOff;
            lineRenderer.endColor = laserColorOff;

            if (!isFiring)
            {
                VisualizeLaser(); // Update laser positions
            }
        }
        else if (playerDetected)
        {
            // Lights are on and player is detected
            lineRenderer.enabled = true;
            lineRenderer.startColor = laserColorOn; // Set laser color to red
            lineRenderer.endColor = laserColorOn;

            if (!isFiring)
            {
                VisualizeLaser(); // Update laser positions
            }
        }
        else if (!isFiring)
        {
            lineRenderer.enabled = false; // Hide the laser when not firing and player is not detected
        }

        // Fire the laser
        if (Input.GetKeyDown(KeyCode.Space) && cooldownRemaining <= 0 && !isFiring && gameManager.shotsRemaining > 0 && (mirrorPlacement == null || !mirrorPlacement.IsPlacingMirror))
        {
            FireLaser();
        }

        if (isFiring)
        {
            ExtendLaser();
        }
    }

    void FireLaser()
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

        // Set the laser color to red when firing
        lineRenderer.startColor = laserColorOn;
        lineRenderer.endColor = laserColorOn;

        gameManager.FireLaser(); // Notify GameManager that the laser has been fired

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

            // If the laser has stopped firing, exit the method
            if (!isFiring)
            {
                return;
            }
        }

        if (lineRenderer.positionCount < 2)
        {
            lineRenderer.positionCount = 2;
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

        // Reset laser direction and length to prevent unexpected behavior
        fireDirection = Vector3.zero;  // Reset the direction
        currentLaserLength = 0f;  // Reset the length

        // Check the game state only after the laser stops firing
        gameManager.CheckGameState();
    }

    void HandleHit(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Mirror") && bouncesLeft > 0)
        {
            Vector3 reflectionDirection = Vector3.Reflect(fireDirection, hit.normal);
            currentStartPosition = hit.point + reflectionDirection * 0.01f;
            fireDirection = reflectionDirection;
            currentLaserLength = 0f;
            bouncesLeft--;
            lineRenderer.SetPosition(0, currentStartPosition);

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
                enemy.TakeDamage();
                StopFiring();
                return;  // Exit the method immediately
            }
        }
        else if (hit.collider.CompareTag("Glass"))
        {
            currentStartPosition = hit.point + fireDirection * 0.01f;
            currentLaserLength = 0f;
            lineRenderer.SetPosition(0, currentStartPosition);
        }
        else
        {
            StopFiring();
            return;  // Exit the method immediately
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

            // Calculate the remaining laser length
            float remainingLength = maxLaserLength - currentLaserLength;

            // Check for any objects in the path of the laser
            if (Physics.Raycast(ray, out hit, remainingLength))
            {
                if (lineRenderer.positionCount <= positionIndex)
                {
                    lineRenderer.positionCount = positionIndex + 1;  // Increase the position count for the new position
                }

                lineRenderer.SetPosition(positionIndex, hit.point);
                currentLaserLength += Vector3.Distance(currentStartPosition, hit.point);
                positionIndex++;

                if (hit.collider.CompareTag("Mirror"))
                {
                    fireDirection = Vector3.Reflect(fireDirection, hit.normal);  // Reflect the laser's direction
                    currentStartPosition = hit.point + fireDirection * 0.01f;    // Slightly offset to avoid re-triggering the same mirror
                    bouncesLeft--;  // Decrease the number of allowed bounces
                }
                else if (hit.collider.CompareTag("Glass"))
                {
                    // Continue the laser slightly past the glass
                    currentStartPosition = hit.point + fireDirection * 0.01f;
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
                if (lineRenderer.positionCount <= positionIndex)
                {
                    lineRenderer.positionCount = positionIndex + 1;
                }
                Vector3 endPosition = currentStartPosition + fireDirection * remainingLength;
                lineRenderer.SetPosition(positionIndex, endPosition);
                currentLaserLength += remainingLength;
                break;
            }
        }
    }
}
