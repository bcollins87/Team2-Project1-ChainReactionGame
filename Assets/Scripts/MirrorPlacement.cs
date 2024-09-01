using UnityEngine;
using System.Collections.Generic;

public class MirrorPlacement : MonoBehaviour
{
    public GameObject mirrorPrefab;          // The mirror prefab to place
    public int maxMirrors = 3;               // Maximum number of mirrors that can be placed
    public LayerMask placementLayer;         // Layer on which mirrors can be placed
    public float rotationSpeed = 10f;        // Speed at which the mirror rotates
    public float pickupRange = 3f;           // Range within which a player can pick up a mirror
    public float colorChangeRange = 2.5f;    // Range for changing mirror color
    public Color pickupColor = Color.green;  // Color to change when player is in range and mouse is over mirror
    public Color defaultColor = Color.white; // Default color of the mirror

    private GameObject currentMirror;        // Currently selected mirror to place
    private int mirrorsPlaced = 0;           // Current count of placed mirrors
    private List<GameObject> pickedUpMirrors = new List<GameObject>(); // List to track picked-up mirrors

    private float rotationStep = 5f;         // Rotate by 5 degrees each step
    private float rotationInterval = 0.1f;   // Time between each rotation step in seconds
    private float rotationTimer = 0f;        // Timer to control rotation frequency

    private GameObject player;               // Reference to the player object

    public bool IsPlacingMirror { get; private set; } // Public property to check if a mirror is being placed

    void Start()
    {
        // Cache the player reference at the start
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
        }

        IsPlacingMirror = false; // Initially, no mirror is being placed
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

            if (Input.GetMouseButtonDown(1)) // Right-click to place the mirror
            {
                PlaceMirror();
            }
        }

        // Check for mirror pickup
        if (Input.GetKeyDown(KeyCode.F)) // Change to 'F' key for picking up mirrors
        {
            PickupMirror();
        }

        // Check player proximity to mirrors and mouse over to change color
        if (!IsPlacingMirror) // Only check proximity if not placing a mirror
        {
            CheckProximityAndMouseOverMirrors();
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
                currentMirror = Instantiate(mirrorPrefab, hit.point + new Vector3(0, 0.1f, 0), Quaternion.identity);
                IsPlacingMirror = true; // Set flag to true when starting to place a mirror
                Debug.Log("Started placing a new mirror.");
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
                currentMirror.transform.position = hit.point + new Vector3(0, 0.1f, 0);
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
            currentMirror = null; // Deselect mirror after placing
            mirrorsPlaced++;      // Increment the number of mirrors placed
            IsPlacingMirror = false; // Set flag to false after placing a mirror
            Debug.Log("Mirror placed. Total mirrors placed: " + mirrorsPlaced);
        }
    }

    void PickupMirror()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Draw the ray in the scene view for debugging
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red, 1.0f);

        if (Physics.Raycast(ray, out hit, pickupRange, placementLayer))
        {
            if (hit.collider.CompareTag("Mirror"))
            {
                GameObject mirrorToPickup = hit.collider.gameObject;

                // Check if this mirror is already picked up
                if (pickedUpMirrors.Contains(mirrorToPickup))
                {
                    Debug.Log("This mirror is already picked up.");
                    return;
                }

                // Pickup the mirror
                mirrorToPickup.SetActive(false);
                pickedUpMirrors.Add(mirrorToPickup);
                mirrorsPlaced--; // Decrement mirror count
                Debug.Log("Mirror picked up: " + mirrorToPickup.name);
            }
            else
            {
                Debug.Log("Hit object is not a mirror: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any object within pickup range.");
        }
    }

    void CheckProximityAndMouseOverMirrors()
    {
        // If player reference is null, exit the method
        if (player == null)
        {
            Debug.LogError("Player reference is null. Cannot check proximity.");
            return;
        }

        Vector3 playerPosition = player.transform.position;

        // Check each mirror's distance to the player and if the mouse is over the mirror
        foreach (GameObject mirror in GameObject.FindGameObjectsWithTag("Mirror"))
        {
            float distance = Vector3.Distance(playerPosition, mirror.transform.position);
            Renderer mirrorRenderer = mirror.GetComponent<Renderer>();

            if (mirrorRenderer == null)
            {
                Debug.Log("No Renderer found on mirror: " + mirror.name);
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
                        Debug.Log("Mirror in range and mouse over: " + mirror.name);
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
    }

    public void ReplaceMirror(Vector3 position)
    {
        // Check if there is a mirror to replace and if replacing wouldn't exceed max mirrors
        if (pickedUpMirrors.Count > 0 && mirrorsPlaced < maxMirrors)
        {
            GameObject mirrorToReplace = pickedUpMirrors[0]; // Replace the first picked-up mirror
            mirrorToReplace.transform.position = position + new Vector3(0, 0.1f, 0);
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
