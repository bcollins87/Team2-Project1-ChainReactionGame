using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MirrorPlacementTut : MonoBehaviour

{
    public GameObject mirrorPrefab;          // The mirror prefab to place
    public int maxMirrors = 3;               // Maximum number of mirrors that can be placed
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
    public int mirrorsPlaced = 0;           // Current count of placed mirrors
    private List<GameObject> pickedUpMirrors = new List<GameObject>(); // List to track picked-up mirrors

    public float rotationStep = 5f;         // Rotate by 5 degrees each step
    public float rotationInterval = 0.1f;   // Time between each rotation step in seconds
    private float rotationTimer = 0f;        // Timer to control rotation frequency

    public GameObject player;               // Reference to the player object
    private GameStateManager gameStateManager; // Reference to the GameStateManager

    public bool IsPlacingMirror { get; private set; } // Public property to check if a mirror is being placed

    public GameObject mirrorUIWhite;
    public GameObject mirrorUIGreen;
    public PlayerCollisionsHELPSCREEN playerCollisionsHELPSCREEN;
    private Scene scene;


    void Start()
    {
        // Cache the player reference at the start
        player = GameObject.FindGameObjectWithTag("Player");
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
        mirrorUIGreen.SetActive(false); //Hides the green mirror UI Object at the start of the level

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
        scene = SceneManager.GetActiveScene();
        Debug.Log(scene);
        if (scene.name == "Help Scene")
        {
            Debug.Log("Help Controls Active");
            // Check if the game is in a panning state
            if (gameStateManager != null && gameStateManager.IsPanning)
            {
                // If the camera is panning, prevent mirror placement and pickup
                return;
            }

            // Check for mirror placement
            if (playerCollisionsHELPSCREEN.mirrorActive) //active if player has done the player movement tutorial
            {
                Debug.Log("Mirror Active is true");
                if (Input.GetMouseButtonDown(0) && mirrorsPlaced < maxMirrors && currentMirror == null)
                {
                    StartPlacingMirror();
                    Debug.Log("Placing Mirror");
                }


                if (currentMirror != null)
                {
                    MoveMirrorToMousePosition();
                    if (playerCollisionsHELPSCREEN.rotateMirror == true) //active if player activates mirror
                    {
                        HandleMirrorRotation(); // Updated method for rotating mirrors
                    }

                    if (IsPlacementValid())
                    {
                        currentMirror.GetComponent<Renderer>().material.color = defaultColor;
                    }
                    else
                    {
                        currentMirror.GetComponent<Renderer>().material.color = invalidPlacementColor;
                    }

                    if (playerCollisionsHELPSCREEN.placeMirror == true) //active if player rotates mirror
                    {
                        if (Input.GetMouseButtonDown(1) && IsPlacementValid()) // Right-click to place the mirror if the position is valid
                        {
                            PlaceMirror();
                        }
                    }
                }
            }

            // Check for mirror pickup
            if (playerCollisionsHELPSCREEN.mirrorPickUp == true) //active if player places mirror
            {
                if (Input.GetKeyDown(KeyCode.F)) // Change to 'F' key for picking up mirrors
                {
                    PickupMirror();
                }
            }

            // Check player proximity to mirrors and mouse over to change color
            if (!IsPlacingMirror) // Only check proximity if not placing a mirror
            {
                CheckProximityAndMouseOverMirrors();
            }

            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
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

        if (Physics.Raycast(ray, out hit, pickupRange, placementLayer))
        {
            if (hit.collider.CompareTag("Mirror"))
            {
                GameObject mirrorToPickup = hit.collider.gameObject;

                // Check if this mirror is already picked up
                if (pickedUpMirrors.Contains(mirrorToPickup))
                {
                    return;
                }

                // Pickup the mirror
                mirrorToPickup.SetActive(false); // Temporarily disable the mirror in the scene
                pickedUpMirrors.Add(mirrorToPickup); // Add to the list of picked up mirrors
                mirrorsPlaced--; // Decrement mirror count

                // Hide the pickup prompt
                if (pickupText != null)
                {
                    pickupText.text = "";
                }

                // Play mirror pickup sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.mirrorPickupClip);
                }
            }
        }
    }

    // Check if the current mirror placement is valid
    bool IsPlacementValid()
    {
        if (currentMirror == null)
        {
            return false;
        }

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

        // Check each mirror's distance to the player and if the mouse is over the mirror
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
                // Perform a raycast from the camera to the mirror
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
                {
                    if (hit.collider.gameObject == mirror)
                    {
                        mirrorRenderer.material.color = pickupColor; // Change color if in range and mouse over mirror

                        // Show the pickup prompt
                        pickupText.text = "Press F to Pick Up";
                        isNearAnyMirror = true;
                    }
                    else
                    {
                        mirrorRenderer.material.color = defaultColor; // Revert to default color
                    }
                }
            }
            else
            {
                mirrorRenderer.material.color = defaultColor; // Revert to default color
            }
        }

        // If not near any mirror, hide the pickup prompt
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
