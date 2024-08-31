using UnityEngine;
using System.Collections.Generic;

public class MirrorPlacement : MonoBehaviour
{
    public GameObject mirrorPrefab;  // The mirror prefab to place
    public int maxMirrors = 3;       // Maximum number of mirrors that can be placed
    public LayerMask placementLayer; // Layer on which mirrors can be placed
    public float rotationSpeed = 10f; // Speed at which the mirror rotates
    public float pickupRange = 3f;   // Range within which a player can pick up a mirror

    private GameObject currentMirror; // Currently selected mirror to place
    private int mirrorsPlaced = 0;    // Current count of placed mirrors
    private List<GameObject> pickedUpMirrors = new List<GameObject>(); // List to track picked-up mirrors

    private float rotationStep = 5f;  // Rotate by 5 degrees each step
    private float rotationInterval = 0.1f; // Time between each rotation step in seconds
    private float rotationTimer = 0f; // Timer to control rotation frequency

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
                Debug.Log("Mirror rotated counterclockwise by 5 degrees.");
                rotationTimer = rotationInterval; // Reset the timer
            }
            else if (Input.GetKey(KeyCode.E)) // Rotate clockwise
            {
                currentMirror.transform.Rotate(Vector3.up, rotationStep);
                Debug.Log("Mirror rotated clockwise by 5 degrees.");
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
            Debug.Log("Mirror placed. Total mirrors placed: " + mirrorsPlaced);
        }
    }

    void PickupMirror()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
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
                Debug.Log("Mirror picked up. Total mirrors placed: " + mirrorsPlaced);
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
