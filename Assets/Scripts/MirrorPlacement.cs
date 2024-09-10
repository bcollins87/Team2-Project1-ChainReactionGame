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
    public float pickupRange = 20f;           // Range within which a player can pick up a mirror
    public float colorChangeRange = 2.5f;    // Range for changing mirror color
    public Color pickupColor = Color.green;  // Color to change when player is in range and mouse is over mirror
    public Color defaultColor = Color.white; // Default color of the mirror
    public Color invalidPlacementColor = Color.red; // Color to indicate invalid placement
    public float placementCheckRadius = 0.5f; // Radius for checking valid placement
    public TMP_Text pickupText;              // Reference to the TextMeshPro UI Text for pickup prompt
    public float placementRange = 5f; // Maximum range within which the player can place mirrors

    private GameObject currentMirror;        // Currently selected mirror to place
    public int mirrorsPlaced = 0;            // Current count of placed mirrors
    private List<GameObject> pickedUpMirrors = new List<GameObject>(); // List to track picked-up mirrors

    public GameObject player;                // Reference to the player object
    private CharacterController playerController; // Reference to the player's CharacterController
    private GameStateManager gameStateManager; // Reference to the GameStateManager

    // New variables
    private bool isPlacingMirror = false;    // Tracks if a mirror is being placed
    private float rotationTimer = 0f;        // Timer for handling rotation intervals
    public float rotationStep = 15f;         // Degrees per rotation step
    public float rotationInterval = 0.1f;    // Time interval between rotations
    private bool hoverSoundPlayed = false;
    public LayerMask ignorePlayerLayer;

    public TMP_Text mirrorNumberText; //text for mirrors left
    

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
                player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    Debug.LogError("Player object with tag 'Player' not found in the scene.");
                }
            }

            // Cache the player's CharacterController
            if (player != null)
            {
                playerController = player.GetComponent<CharacterController>();
                if (playerController == null)
                {
                    Debug.LogError("Player CharacterController not found.");
                }
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
        mirrorNumberText.text = "" + (maxMirrors-mirrorsPlaced);
        
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
        // Disable the player's CharacterController to prevent movement during placement
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer & ~ignorePlayerLayer))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                // Check if the mirror position is within the allowed range from the player
                Vector3 playerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
                Vector3 hitPosition = new Vector3(hit.point.x, 0, hit.point.z);
                float distanceToPlayer = Vector3.Distance(playerPosition, hitPosition);

                if (distanceToPlayer <= placementRange)
                {
                    float mirrorHeightOffset = mirrorPrefab.transform.localScale.y * 0.5f;
                    Vector3 spawnPosition = hit.point + new Vector3(0, mirrorHeightOffset, 0);

                    currentMirror = Instantiate(mirrorPrefab, spawnPosition, Quaternion.identity);
                    IsPlacingMirror = true; // Start placing the mirror
                }
                else
                {
                    Debug.LogWarning("Mirror placement too far from player.");
                }
            }
        }
    }




void MoveMirrorToMousePosition()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    // Raycast that ignores the player layer using the LayerMask
    if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer & ~ignorePlayerLayer))
    {
        if (hit.collider.CompareTag("Floor"))
        {
            // Calculate the correct position above the floor based on the mirror's height
            float mirrorHeightOffset = currentMirror.transform.localScale.y * 0.5f; // Half of the mirror's height
            Vector3 newPosition = hit.point + new Vector3(0, mirrorHeightOffset, 0); // Add offset above the ground

            // Use only X and Z for distance calculation (top-down game)
            Vector3 playerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            Vector3 mirrorPosition = new Vector3(newPosition.x, 0, newPosition.z);
            float distanceToPlayer = Vector3.Distance(playerPosition, mirrorPosition);

            // Restrict movement to within placement range
            if (distanceToPlayer <= placementRange)
            {
                currentMirror.transform.position = newPosition; // Update the mirror position
            }
            else
            {
                Debug.LogWarning("Cannot move mirror beyond placement range.");
            }
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
                Renderer renderer = currentMirror.GetComponent<Renderer>();
                if (renderer == null)
                {
                    Debug.LogError("The mirror does not have a Renderer component. Cannot place mirror.");
                    return;
                }

                // Re-enable the player's CharacterController after placing the mirror
                if (playerController != null)
                {
                    playerController.enabled = true;
                }

                currentMirror = null;
                mirrorsPlaced++;
                mirrorNumberText.text = "" + (maxMirrors - mirrorsPlaced); //displays number of mirrors left 
                IsPlacingMirror = false;

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
                mirrorNumberText.text = "" + (maxMirrors - mirrorsPlaced); //displays number of mirrors left 

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
