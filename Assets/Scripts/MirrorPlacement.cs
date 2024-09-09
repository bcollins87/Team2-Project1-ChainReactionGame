using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMesh Pro for UI text

public class MirrorPlacement : MonoBehaviour
{
    public GameObject mirrorPrefab;          // The mirror prefab to place
    public int maxMirrors = 5;               // Maximum number of mirrors that can be placed
    public LayerMask placementLayer;         // Layer on which mirrors can be placed
    public LayerMask obstacleLayer;          // Layer to check for obstacles (e.g., walls, other objects)
    public float rotationSpeed = 10f;        // Speed at which the mirror rotates
    public float pickupRange = 3f;           // Range within which a player can pick up a mirror
    public float colorChangeRange = 2.5f;    // Range for changing mirror color
    public Color pickupColor = Color.green;  // Color to change when player is in range and mouse is over mirror
    public Color defaultColor = Color.white; // Default color of the mirror
    public Color invalidPlacementColor = Color.red; // Color to indicate invalid placement
    public float placementCheckRadius = 0.5f; // Radius for checking valid placement
    public TMP_Text pickupText;              // Reference to the TextMeshPro UI Text for pickup prompt

    private GameObject currentMirror;        // Currently selected mirror to place
    public int mirrorsPlaced = 0;            // Current count of placed mirrors
    private List<GameObject> pickedUpMirrors = new List<GameObject>(); // List to track picked-up mirrors

    public GameObject player;                // Reference to the player object
    private GameStateManager gameStateManager; // Reference to the GameStateManager

    // New variables
    private bool isPlacingMirror = false;    // Tracks if a mirror is being placed
    private float rotationTimer = 0f;        // Timer for handling rotation intervals
    public float rotationStep = 15f;         // Degrees per rotation step
    public float rotationInterval = 0.1f;    // Time interval between rotations
    private bool hoverSoundPlayed = false;

    // Updated property
    public bool IsPlacingMirror
    {
        get { return isPlacingMirror; }
        private set { isPlacingMirror = value; }
    }
    

    void Start()
    {
        // Cache the player reference at the start
        if (player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
        }

        // Find the GameStateManager in the scene
        gameStateManager = FindObjectOfType<GameStateManager>();
        if (gameStateManager == null)
        {
            Debug.LogError("GameStateManager not found in the scene.");
        }

        IsPlacingMirror = false; // Initially, no mirror is being placed

        // Initially hide the pickup text
        if (pickupText != null)
        {
            pickupText.text = ""; // Clear text at the start
        }
        else
        {
            Debug.LogError("PickupText UI element not assigned.");
        }
    }

    void Update()
    {
        // Check if the game is in a panning state
        if (gameStateManager != null && gameStateManager.IsPanning)
        {
            // If the camera is panning, prevent mirror placement and pickup
            return;
        }

        // Check for mirror placement
        if (Input.GetMouseButtonDown(0) && mirrorsPlaced < maxMirrors && currentMirror == null)
        {
            StartPlacingMirror();
        }

        if (currentMirror != null)
        {
            MoveMirrorToMousePosition();
            HandleMirrorRotation(); // Updated method for rotating mirrors

            if (IsPlacementValid())
            {
                currentMirror.GetComponent<Renderer>().material.color = defaultColor;
            }
            else
            {
                currentMirror.GetComponent<Renderer>().material.color = invalidPlacementColor;
            }

            if (Input.GetMouseButtonDown(1) && IsPlacementValid()) // Right-click to place the mirror if the position is valid
            {
                PlaceMirror();
            }
        }

        // Check for mirror pickup
        if (Input.GetKeyDown(KeyCode.F)) // Change to 'F' key for picking up mirrors
        {
            PickupMirror();
        }

        // Check player proximity to mirrors and mouse over to change color and display text
        if (!IsPlacingMirror) // Only check proximity if not placing a mirror
        {
            CheckProximityAndMouseOverMirrors();
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void StartPlacingMirror()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                // Calculate the correct spawn position above the floor based on the mirror's height
                float mirrorHeightOffset = mirrorPrefab.transform.localScale.y * 0.5f; // Half of the mirror's height
                Vector3 spawnPosition = hit.point + new Vector3(0, mirrorHeightOffset, 0); // Add offset above the ground
                currentMirror = Instantiate(mirrorPrefab, spawnPosition, Quaternion.identity);
                IsPlacingMirror = true; // Set flag to true when starting to place a mirror
            }
        }
    }

    void MoveMirrorToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                // Calculate the correct position above the floor based on the mirror's height
                float mirrorHeightOffset = currentMirror.transform.localScale.y * 0.5f; // Half of the mirror's height
                Vector3 newPosition = hit.point + new Vector3(0, mirrorHeightOffset, 0); // Add offset above the ground
                currentMirror.transform.position = newPosition;
            }
        }
    }

    void HandleMirrorRotation()
    {
        rotationTimer -= Time.deltaTime; // Decrease the timer

        if (rotationTimer <= 0f) // Check if it's time to rotate
        {
            if (Input.GetKey(KeyCode.Q)) // Rotate counterclockwise
            {
                currentMirror.transform.Rotate(Vector3.up, -rotationStep);
                rotationTimer = rotationInterval; // Reset the timer
            }
            else if (Input.GetKey(KeyCode.E)) // Rotate clockwise
            {
                currentMirror.transform.Rotate(Vector3.up, rotationStep);
                rotationTimer = rotationInterval; // Reset the timer
            }
        }
    }

    void PlaceMirror()
    {
        if (currentMirror != null)
        {
            // Ensure the current mirror is a valid object and its renderer is accessible
            Renderer renderer = currentMirror.GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("The mirror does not have a Renderer component. Cannot place mirror.");
                return;
            }

            // Check if the player or animator is missing
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.animator != null)
            {
                playerMovement.animator.SetTrigger("placeMirror");
            }
            else
            {
                Debug.LogWarning("Player or animator is null. Skipping mirror placement animation.");
            }

            renderer.material.color = defaultColor; // Reset to default color
            currentMirror = null; // Deselect mirror after placing
            mirrorsPlaced++;      // Increment the number of mirrors placed
            IsPlacingMirror = false; // Set flag to false after placing a mirror

            // **Play placement sound here**
            if (AudioManager.Instance != null && AudioManager.Instance.mirrorPlaceClip != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.mirrorPlaceClip);
            }
            else
            {
                Debug.LogWarning("Mirror placement sound not set in AudioManager.");
            }
        }
        else
        {
            Debug.LogWarning("No mirror is being placed.");
        }
    }



    void PickupMirror()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Log the start of the pickup process
        Debug.Log("Attempting to pick up a mirror...");

        // Check if the raycast hits something within the pickup range
        if (Physics.Raycast(ray, out hit, pickupRange, placementLayer))
        {
            Debug.Log("Raycast hit detected: " + hit.collider.gameObject.name);

            // Check if the object hit is a mirror
            if (hit.collider.CompareTag("Mirror"))
            {
                Debug.Log("Mirror detected for pickup.");

                GameObject mirrorToPickup = hit.collider.gameObject;

                // Check if this mirror is already picked up
                if (pickedUpMirrors.Contains(mirrorToPickup))
                {
                    Debug.Log("Mirror has already been picked up.");
                    return;
                }

                // Pickup the mirror
                mirrorToPickup.SetActive(false); // Temporarily disable the mirror in the scene
                pickedUpMirrors.Add(mirrorToPickup); // Add to the list of picked-up mirrors
                mirrorsPlaced--; // Decrement mirror count

                // Update pickup text to reflect mirror pickup
                if (pickupText != null)
                {
                    pickupText.text = "Mirror picked up! Place it by right-clicking";
                }

                // Play mirror pickup sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.mirrorPickupClip);
                }

                Debug.Log("Mirror picked up successfully.");
            }
            else
            {
                Debug.Log("Hit object is not a mirror: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any objects within the pickup range.");
        }
    }



    // Check if the current mirror placement is valid
    bool IsPlacementValid()
    {
        // Check for collisions only with objects in the obstacle layer
        Collider[] colliders = Physics.OverlapSphere(currentMirror.transform.position, placementCheckRadius, obstacleLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != currentMirror)
            {
                return false; // Invalid if colliding with any obstacles
            }
        }
        return true; // Valid if not colliding with walls or other objects
    }

    void CheckProximityAndMouseOverMirrors()
    {
        if (player == null)
        {
            return;
        }

        Vector3 playerPosition = player.transform.position;
        bool isNearAnyMirror = false;

        foreach (GameObject mirror in GameObject.FindGameObjectsWithTag("Mirror"))
        {
            float distance = Vector3.Distance(playerPosition, mirror.transform.position);
            Renderer mirrorRenderer = mirror.GetComponent<Renderer>();

            if (mirrorRenderer == null)
            {
                continue;
            }

            if (distance <= colorChangeRange)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
                {
                    if (hit.collider.gameObject == mirror)
                    {
                        mirrorRenderer.material.color = pickupColor;

                        // Play hover sound once
                        if (!hoverSoundPlayed && AudioManager.Instance != null && AudioManager.Instance.mirrorHoverClip != null)
                        {
                            AudioManager.Instance.PlaySound(AudioManager.Instance.mirrorHoverClip);
                            hoverSoundPlayed = true; // Set flag to true so it doesn't replay
                        }

                        // Show pickup prompt
                        if (distance <= pickupRange)
                        {
                            pickupText.text = "Press F to Pick Up";
                        }
                        else
                        {
                            pickupText.text = "Move closer to pick up the mirror";
                        }
                        
                        isNearAnyMirror = true;
                    }
                    else
                    {
                        mirrorRenderer.material.color = defaultColor;
                    }
                }
            }
            else
            {
                mirrorRenderer.material.color = defaultColor;
            }
        }

        // Reset hover sound flag when not hovering
        if (!isNearAnyMirror)
        {
            hoverSoundPlayed = false;
        }

        // Hide the pickup prompt if no mirror is near
        if (!isNearAnyMirror && pickupText != null)
        {
            pickupText.text = "";
        }
    }


    public void ReplaceMirror(Vector3 position)
    {
        // Check if there is a mirror to replace and if replacing wouldn't exceed max mirrors
        if (pickedUpMirrors.Count > 0 && mirrorsPlaced < maxMirrors)
        {
            GameObject mirrorToReplace = pickedUpMirrors[0]; // Replace the first picked-up mirror
            float mirrorHeightOffset = mirrorToReplace.transform.localScale.y * 0.5f; // Half of the mirror's height
            mirrorToReplace.transform.position = position + new Vector3(0, mirrorHeightOffset, 0);
            mirrorToReplace.SetActive(true); // Re-enable the mirror in the scene
            pickedUpMirrors.RemoveAt(0); // Remove it from the list
            mirrorsPlaced++;
            Debug.Log("Mirror replaced. Total mirrors placed: " + mirrorsPlaced);
        }
        else if (pickedUpMirrors.Count == 0)
        {
            Debug.Log("No mirrors available to replace.");
        }
        else if (mirrorsPlaced >= maxMirrors)
        {
            Debug.Log("Cannot place more than " + maxMirrors + " mirrors.");
        }
    }
}
