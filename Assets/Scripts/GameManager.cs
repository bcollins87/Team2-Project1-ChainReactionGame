using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject winMenu;
    public GameObject loseMenu;
    public int totalEnemies;
    public string nextLevelName;

    public int enemiesRemaining;
    private int shotsFired = 0;
    public int shotsRemaining = 3;
    public int maxShots = 3;  // Maximum shots allowed

    public TMP_Text enemyNumber;
    public TMP_Text shotNumber;
    public TMP_Text mirrorNumber;
    public GameObject restartButton;

    public static GameManager instance;
    public ShotBar shotBar;
    private Scene scene;

    public GameObject player;
    public Animator playerAnimator;
    public Animator elevatorAnimator;
    

    public GameObject activePlayer;

    //UI Update Vars
    public GameObject mirrorPlacement;
    private int mirrorsLeft;
    public MenuLoader menuLoader;
    public GameObject mirrorPlacementTut;
    public GameObject playerTut;
    public GameObject tutorialBoxes;
    //public GameObject tutorialStuff; //Game object that includes all the tutorial objects

    public Collider elevatorCollider;  // Add this to the top with other public variables



    void Update()
    {
        // Debug: Kill all enemies and activate the elevator when pressing the "K" key
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Debug mode: All enemies killed.");
            KillAllEnemiesDebug();
        }
    }

    private void KillAllEnemiesDebug()
    {
        // Set the remaining enemies to 0 and activate the elevator
        enemiesRemaining = 0;
        CheckGameState();  // Check and trigger the elevator activation
    }



    public void EnemyKilled()
    {
        playerAnimator.SetTrigger("celebrate");
        enemiesRemaining--;

        if (enemyNumber != null)
        {
            enemyNumber.text = "" + enemiesRemaining;
        }
        else
        {
            Debug.LogError("Enemy number text reference is null.");
        }

        Debug.Log("Enemy killed. Remaining: " + enemiesRemaining);

        // Play enemy death sound
        if (AudioManager.Instance != null && AudioManager.Instance.enemyDeathClip != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.enemyDeathClip);
        }
        else
        {
            Debug.LogError("AudioManager instance or enemyDeathClip is null. Cannot play death sound.");
        }

        CheckGameState(); // Check if the game should end
    }

    public void FireLaser()
    {
        shotsFired++;
        shotsRemaining--;
        shotBar.SetShots(shotsRemaining);

        if (shotNumber != null)
        {
            shotNumber.text = "" + shotsRemaining;
        }
        else
        {
            Debug.LogError("Shot number text reference is null.");
        }

        Debug.Log("Laser fired. Shots fired: " + shotsFired);

        // Play laser firing sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.laserShotClip);
        }
        else
        {
            Debug.LogError("AudioManager instance is null. Cannot play laser shot sound.");
        }
    }

   public void CheckGameState()
    {
        Debug.Log("Checking game state. Shots fired: " + shotsFired + ", Enemies remaining: " + enemiesRemaining);

        // Win condition: No enemies left
        if (enemiesRemaining <= 0)
        {
            // Check if you're on the final level (Level 3 in this case)
            if (SceneManager.GetActiveScene().name == "Level3")
            {
                // Show the win screen only on Level 3
                if (winMenu != null && loseMenu != null)
                {
                    winMenu.SetActive(true);
                    loseMenu.SetActive(false);  // Ensure the lose menu is not active
                    restartButton.SetActive(true);
                    Debug.Log("Win condition met - Final Level Completed");

                    // Play win sound
                    if (AudioManager.Instance != null && AudioManager.Instance.winClip != null)
                    {
                        AudioManager.Instance.PlaySound(AudioManager.Instance.winClip);
                    }
                    else
                    {
                        Debug.LogError("AudioManager instance or winClip is null. Cannot play win sound.");
                    }
                }
                else
                {
                    Debug.LogError("Win or Lose menu references are null.");
                }
            }
            else
            {
                // Just unlock the elevator for non-final levels
                Debug.Log("Enemies defeated. Unlocking the elevator for the next level.");
                elevatorAnimator.SetTrigger("levelComplete");

                if (elevatorCollider != null)
                {
                    elevatorCollider.enabled = true;
                }
                else
                {
                    Debug.LogError("Elevator collider is not assigned.");
                }
            }
        }
        // Lose condition: No shots left, but enemies still remain
        else if (shotsFired >= maxShots && enemiesRemaining > 0)
        {
            if (winMenu != null && loseMenu != null)
            {
                loseMenu.SetActive(true);
                winMenu.SetActive(false);  // Ensure the win menu is not active
                restartButton.SetActive(true);
                Debug.Log("Lose condition met");

                // Play lose sound
                if (AudioManager.Instance != null && AudioManager.Instance.loseClip != null)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.loseClip);
                }
                else
                {
                    Debug.LogError("AudioManager instance or loseClip is null. Cannot play lose sound.");
                }
            }
            else
            {
                Debug.LogError("Win or Lose menu references are null.");
            }
        }
    }

    public void PlayerEnteredElevator()
    {
        if (enemiesRemaining <= 0)  // Ensure all enemies are defeated
        {
            // Instead of using the dynamic name, directly load the name of the next level
            SceneManager.LoadScene(nextLevelName);  // Load next level
            Debug.Log("Transitioning to next level: " + nextLevelName);
        }
        else
        {
            // The player is trying to enter the elevator without defeating all enemies
            Debug.Log("Cannot enter the elevator until all enemies are defeated.");
            
            // Do NOT activate the win screen here!
            // Remove the following line that causes the win screen to show prematurely:
            // winMenu.SetActive(true);
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
