using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMesh Pro for UI text

public class MirrorPlacementTutorial : MonoBehaviour
{
    public GameObject mirrorPrefab;          // The mirror prefab to place
    public int maxMirrors = 5;               // Maximum number of mirrors that can be placed
    public LayerMask placementLayer;         // Layer on which mirrors can be placed
    public LayerMask obstacleLayer;          // Layer to check for obstacles (e.g., walls, other objects)
    public float rotationSpeed = 10f;        // Speed at which the mirror rotates
    public float pickupRange = 20f;          // Range within which a player can pick up a mirror
    public float colorChangeRange = 2.5f;    // Range for changing mirror color
    public Color pickupColor = Color.green;  // Color to change when player is in range and mouse is over mirror
    public Color defaultColor = Color.white; // Default color of the mirror
    public Color invalidPlacementColor = Color.red; // Color to indicate invalid placement
    public float placementCheckRadius = 0.5f; // Radius for checking valid placement
    public TMP_Text pickupText;              // Reference to the TextMeshPro UI Text for pickup prompt
    public float placementRange = 5f;        // Maximum range within which the player can place mirrors

    private GameObject currentMirror;        // Currently selected mirror to place
    public int mirrorsPlaced = 0;            // Current count of placed mirrors
    private List<GameObject> pickedUpMirrors = new List<GameObject>(); // List to track picked-up mirrors

    public GameObject player;                // Reference to the player object
    private CharacterController playerController; // Reference to the player's CharacterController

    // New variables
    private bool isPlacingMirror = false;    // Tracks if a mirror is being placed
    private float rotationTimer = 0f;        // Timer for handling rotation intervals
    public float rotationStep = 15f;         // Degrees per rotation step
    public float rotationInterval = 0.1f;    // Time interval between rotations
    private bool hoverSoundPlayed = false;
    public LayerMask ignorePlayerLayer;

    public TMP_Text mirrorNumberText;        // Text for mirrors left
    
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

        isPlacingMirror = false; // Initially, no mirror is being placed

        // Initially hide the pickup text
        if (pickupText != null)
        {
            pickupText.text = ""; // Clear text at the start
        }
        else
        {
            Debug.LogError("PickupText UI element not assigned.");
        }
        mirrorNumberText.text = "" + (maxMirrors - mirrorsPlaced);
    }

    void Update()
    {
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
        if (!isPlacingMirror) // Only check proximity if not placing a mirror
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
        // Start placing a new mirror
        currentMirror = Instantiate(mirrorPrefab);
        isPlacingMirror = true;
    }

    void MoveMirrorToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, placementRange, placementLayer))
        {
            currentMirror.transform.position = hit.point;
        }
    }

    void HandleMirrorRotation()
    {
        rotationTimer += Time.deltaTime;
        if (rotationTimer >= rotationInterval)
        {
            if (Input.GetKey(KeyCode.R))
            {
                currentMirror.transform.Rotate(Vector3.up, rotationStep);
            }
            rotationTimer = 0f;
        }
    }

    bool IsPlacementValid()
    {
        Collider[] hitColliders = Physics.OverlapSphere(currentMirror.transform.position, placementCheckRadius, obstacleLayer);
        return hitColliders.Length == 0;  // If no obstacles found, placement is valid
    }

    void PlaceMirror()
    {
        mirrorsPlaced++;
        isPlacingMirror = false;
        currentMirror = null;  // Reset current mirror after placing
        mirrorNumberText.text = "" + (maxMirrors - mirrorsPlaced);
    }

    void PickupMirror()
    {
        // Logic to pick up mirrors
        // Add mirror to pickedUpMirrors and remove from the scene
    }

    void CheckProximityAndMouseOverMirrors()
    {
        // Logic to change mirror color when player is nearby or mouse is hovering over it
    }
}