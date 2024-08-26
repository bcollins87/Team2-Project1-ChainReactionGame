using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winMenu;
    public GameObject loseMenu;
    public int totalEnemies;

    private int enemiesRemaining;
    private int shotsFired = 0;
    public int maxShots = 3;  // Maximum shots allowed

    void Start()
    {
        enemiesRemaining = totalEnemies;
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
    }

    public void EnemyKilled()
    {
        enemiesRemaining--;
        Debug.Log("Enemy killed. Remaining: " + enemiesRemaining);
        // CheckGameState(); // Now called from Laser script after it stops firing
    }

    public void FireLaser()
    {
        shotsFired++;
        Debug.Log("Laser fired. Shots fired: " + shotsFired);
        // CheckGameState(); // Now called from Laser script after it stops firing
    }

    public void CheckGameState()
    {
        Debug.Log("Checking game state. Shots fired: " + shotsFired + ", Enemies remaining: " + enemiesRemaining);
        if (enemiesRemaining <= 0)
        {
            // Player wins if no enemies are left
            winMenu.SetActive(true);
            Debug.Log("Win condition met");
        }
        else if (shotsFired >= maxShots)
        {
            // Player loses if they run out of shots
            loseMenu.SetActive(true);
            Debug.Log("Lose condition met");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
