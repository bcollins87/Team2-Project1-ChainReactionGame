using UnityEngine;

public class ElevatorDoorController : MonoBehaviour
{
    public Transform leftDoor;    // Reference to the left door's transform
    public Transform rightDoor;   // Reference to the right door's transform

    public float doorOpenDistance = 2f;  // Distance the doors move when opening
    public float doorCloseDistance = 2f; // Distance the doors move when closing
    public float doorSpeed = 2f;         // Speed at which the doors move

    private Vector3 leftDoorClosedPosition;
    private Vector3 rightDoorClosedPosition;

    private Vector3 leftDoorOpenPosition;
    private Vector3 rightDoorOpenPosition;

    private Vector3 leftDoorClosePosition;
    private Vector3 rightDoorClosePosition;

    private bool doorsOpen = false;
    private bool doorsMoving = false;
    private bool playerExited = false;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Store the initial closed positions
        leftDoorClosedPosition = leftDoor.position;
        rightDoorClosedPosition = rightDoor.position;

        // Calculate the open positions for opening
        leftDoorOpenPosition = leftDoorClosedPosition + new Vector3(-doorOpenDistance, 0, 0);
        rightDoorOpenPosition = rightDoorClosedPosition + new Vector3(doorOpenDistance, 0, 0);

        // Calculate the close positions for closing
        leftDoorClosePosition = leftDoorOpenPosition + new Vector3(doorCloseDistance, 0, 0);
        rightDoorClosePosition = rightDoorOpenPosition + new Vector3(-doorCloseDistance, 0, 0);

        // Open the doors at the start
        OpenDoors();
    }

    void Update()
    {
        // Check if all enemies are dead and the doors are closed
        if (gameManager != null && gameManager.enemiesRemaining <= 0 && !doorsOpen && !doorsMoving)
        {
            OpenDoors();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !playerExited)
        {
            playerExited = true;
            CloseDoors();
        }
    }

    public void OpenDoors()
    {
        if (!doorsOpen && !doorsMoving)
        {
            doorsMoving = true;
            StartCoroutine(MoveDoors(leftDoorOpenPosition, rightDoorOpenPosition));
        }
    }

    public void CloseDoors()
    {
        if (doorsOpen && !doorsMoving)
        {
            doorsMoving = true;
            StartCoroutine(MoveDoors(leftDoorClosePosition, rightDoorClosePosition));
        }
    }

    private System.Collections.IEnumerator MoveDoors(Vector3 leftTarget, Vector3 rightTarget)
    {
        while (Vector3.Distance(leftDoor.position, leftTarget) > 0.01f)
        {
            leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftTarget, doorSpeed * Time.deltaTime);
            rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightTarget, doorSpeed * Time.deltaTime);
            yield return null;
        }

        leftDoor.position = leftTarget;
        rightDoor.position = rightTarget;

        doorsMoving = false;
        doorsOpen = !doorsOpen;
    }
}
