using UnityEngine;

public class MirrorPlacement : MonoBehaviour
{
    public GameObject mirrorPrefab;  // The mirror prefab to place
    public int maxMirrors = 3;       // Maximum number of mirrors that can be placed
    public LayerMask placementLayer; // Layer on which mirrors can be placed
    private GameObject currentMirror; // Currently selected mirror to place
    private int mirrorsPlaced = 0;   // Current count of placed mirrors
    public float rotationSpeed = 1000f; // Speed at which the mirror rotates

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && mirrorsPlaced < maxMirrors && currentMirror == null) // Right-click to start placing a mirror
        {
            StartPlacingMirror();
        }

        if (currentMirror != null)
        {
            MoveMirrorToMousePosition();
            RotateMirror();

            if (Input.GetMouseButtonDown(0)) // Left-click to place the mirror
            {
                PlaceMirror();
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
                currentMirror = Instantiate(mirrorPrefab, hit.point + new Vector3(0, 0.1f, 0), Quaternion.identity);
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

    void RotateMirror()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
        {
            currentMirror.transform.Rotate(Vector3.up, -rotationStep); // Rotate counterclockwise
        }
        else if (Input.GetKey(KeyCode.E))
        {
            currentMirror.transform.Rotate(Vector3.up, rotationStep); // Rotate clockwise
        }
    }

    void PlaceMirror()
    {
        currentMirror = null; // Deselect mirror after placing
        mirrorsPlaced++;      // Increment the number of mirrors placed
        Debug.Log("Mirror placed. Total mirrors placed: " + mirrorsPlaced);
    }
}
