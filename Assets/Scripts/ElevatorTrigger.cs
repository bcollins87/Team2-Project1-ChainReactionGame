using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Assuming the player has the tag "Player"
        {
            Debug.Log("Player entered the elevator.");
            GameManager.instance.PlayerEnteredElevator();  // Notify GameManager to load the next level
        }
    }
}
