using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // TextMesh Pro for UI text

public class MirrorPlacementTutorial : MonoBehaviour
{
    public AudioManager audioManager;  // Direct reference to AudioManager
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

    private bool isPlacingMirror = false;    // Tracks if a mirror is being placed
    private float rotationTimer = 0f;        // Timer for handling rotation intervals
    public float rotationStep = 15f;         // Degrees per rotation step
    public float rotationInterval = 0.1f;    // Time interval between rotations
    private bool hoverSoundPlayed = false;
    public LayerMask ignorePlayerLayer;

    public TMP_Text mirrorNumberText;        // Text for mirrors left
    public PlayerCollisionsHELPSCREEN playerCollisionsHelpScreen;

    public bool IsPlacingMirror
    {
        get { return isPlacingMirror; }
        private set { isPlacingMirror = value; }
    }

    void Start()
    {
        // Initialize AudioManager reference
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
            if (audioManager == null)
            {
                Debug.LogError("AudioManager not found in the scene!");
            }
        }

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

        IsPlacingMirror = false;

        // Initially hide the pickup text
        if (pickupText != null)
        {
            pickupText.text = "";
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
        if (playerCollisionsHelpScreen.mirrorActive == true)//unlocks mirror controls (limited)
        {
            if (Input.GetMouseButtonDown(0) && mirrorsPlaced < maxMirrors && currentMirror == null)
            {
                StartPlacingMirror();
            }
            
            if(playerCollisionsHelpScreen.rotateMirror == true) //unlocks mirror rotation
            {
                if (currentMirror != null)
                {
                    Debug.Log("Mirror can be rotated");
                    MoveMirrorToMousePosition();
                    HandleMirrorRotation();

                    if (IsPlacementValid())
                    {
                        currentMirror.GetComponent<Renderer>().material.color = defaultColor;
                    }
                    else
                    {
                        currentMirror.GetComponent<Renderer>().material.color = invalidPlacementColor;
                    }
                    if (playerCollisionsHelpScreen.placeMirror == true) //unlocks mirror placement
                    {
                        if (Input.GetMouseButtonDown(1) && IsPlacementValid())
                        {
                            PlaceMirror();
                        }

                        // Check for mirror pickup
                        if (playerCollisionsHelpScreen.pickUpMirror == true)
                        {
                            Debug.Log("pick up active");
                            if (Input.GetKeyDown(KeyCode.F))
                            {
                                Debug.Log("Pick Up Attempted");
                                PickupMirror();
                            }
                        }
                    }

                }
            }



            // Check player proximity to mirrors and mouse over to change color and display text
            if (!isPlacingMirror)
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
                Vector3 playerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
                Vector3 hitPosition = new Vector3(hit.point.x, 0, hit.point.z);
                float distanceToPlayer = Vector3.Distance(playerPosition, hitPosition);

                if (distanceToPlayer <= placementRange)
                {
                    float mirrorHeightOffset = mirrorPrefab.transform.localScale.y * 0.5f;
                    Vector3 spawnPosition = hit.point + new Vector3(0, mirrorHeightOffset, 0);

                    currentMirror = Instantiate(mirrorPrefab, spawnPosition, Quaternion.identity);
                    IsPlacingMirror = true;
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer & ~ignorePlayerLayer))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                float mirrorHeightOffset = currentMirror.transform.localScale.y * 0.5f;
                Vector3 newPosition = hit.point + new Vector3(0, mirrorHeightOffset, 0);

                Vector3 playerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
                Vector3 mirrorPosition = new Vector3(newPosition.x, 0, newPosition.z);
                float distanceToPlayer = Vector3.Distance(playerPosition, mirrorPosition);

                if (distanceToPlayer <= placementRange)
                {
                    currentMirror.transform.position = newPosition;
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
        rotationTimer -= Time.deltaTime;

        if (rotationTimer <= 0f)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                currentMirror.transform.Rotate(Vector3.up, -rotationStep);
                rotationTimer = rotationInterval;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                currentMirror.transform.Rotate(Vector3.up, rotationStep);
                rotationTimer = rotationInterval;
            }
        }
    }

    void PlaceMirror()
    {
        if (currentMirror != null)
        {
            if (playerController != null)
            {
                playerController.enabled = true;
            }

            currentMirror = null;
            mirrorsPlaced++;
            mirrorNumberText.text = "" + (maxMirrors - mirrorsPlaced);
            IsPlacingMirror = false;

            // Play mirror placement sound
            if (audioManager != null && audioManager.mirrorPlaceClip != null)
            {
                audioManager.PlaySound(audioManager.mirrorPlaceClip);
            }
            else
            {
                Debug.LogWarning("Mirror placement sound not set in AudioManager.");
            }
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

                if (pickedUpMirrors.Contains(mirrorToPickup))
                {
                    return;
                }

                mirrorToPickup.SetActive(false);
                pickedUpMirrors.Add(mirrorToPickup);
                mirrorsPlaced--;
                mirrorNumberText.text = "" + (maxMirrors - mirrorsPlaced);

                if (pickupText != null)
                {
                    pickupText.text = "Mirror picked up! Place it by right-clicking";
                }

                if (audioManager != null && audioManager.mirrorPickupClip != null)
                {
                    audioManager.PlaySound(audioManager.mirrorPickupClip);
                }
            }
        }
    }

    bool IsPlacementValid()
    {
        Collider[] colliders = Physics.OverlapSphere(currentMirror.transform.position, placementCheckRadius, obstacleLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != currentMirror)
            {
                return false;
            }
        }
        return true;
    }

    void CheckProximityAndMouseOverMirrors()
    {
        Vector3 playerPosition = player.transform.position;
        bool isNearAnyMirror = false;

        foreach (GameObject mirror in GameObject.FindGameObjectsWithTag("Mirror"))
        {
            float distance = Vector3.Distance(playerPosition, mirror.transform.position);
            Renderer mirrorRenderer = mirror.GetComponent<Renderer>();

            if (mirrorRenderer == null) continue;

            if (distance <= colorChangeRange)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
                {
                    if (hit.collider.gameObject == mirror)
                    {
                        mirrorRenderer.material.color = pickupColor;

                        if (!hoverSoundPlayed && audioManager != null && audioManager.mirrorHoverClip != null)
                        {
                            audioManager.PlaySound(audioManager.mirrorHoverClip);
                            hoverSoundPlayed = true;
                        }

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

        if (!isNearAnyMirror)
        {
            hoverSoundPlayed = false;
        }

        if (!isNearAnyMirror && pickupText != null)
        {
            pickupText.text = "";
        }
    }

    public void ReplaceMirror(Vector3 position)
    {
        if (pickedUpMirrors.Count > 0 && mirrorsPlaced < maxMirrors)
        {
            GameObject mirrorToReplace = pickedUpMirrors[0];
            float mirrorHeightOffset = mirrorToReplace.transform.localScale.y * 0.5f;
            mirrorToReplace.transform.position = position + new Vector3(0, mirrorHeightOffset, 0);
            mirrorToReplace.SetActive(true);
            pickedUpMirrors.RemoveAt(0);
            mirrorsPlaced++;
        }
    }
}
