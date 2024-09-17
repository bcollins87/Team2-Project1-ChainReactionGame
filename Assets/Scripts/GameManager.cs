using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioManager audioManager;  // Direct reference to AudioManager
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

    public ShotBar shotBar;
    private Scene scene;

    public GameObject player;
    public Animator playerAnimator;
    public Animator elevatorAnimator;
    public Animator transitionAnimator;

    // UI Update Vars
    public GameObject mirrorPlacement;
    private int mirrorsLeft;
    public MenuLoader menuLoader;
    public GameObject mirrorPlacementTut;
    public GameObject playerTut;
    public GameObject tutorialBoxes;

    public Collider elevatorCollider;

    // Detection tracking variables
    private int detectionCount = 0; // Tracks the number of enemies detecting the player

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

        // Get active scene
        scene = SceneManager.GetActiveScene();

        // Initialize game variables
        enemiesRemaining = totalEnemies;
        shotsRemaining = maxShots;
        shotsFired = 0;

        // Update the UI with initial values
        if (enemyNumber != null)
            enemyNumber.text = enemiesRemaining.ToString();

        if (shotNumber != null)
            shotNumber.text = shotsRemaining.ToString();

        if (shotBar != null)
            shotBar.SetMaxShots(maxShots);

        if (menuLoader != null)
        {
            menuLoader.CheckActiveScene();
            menuLoader.UpdateSceneMenu();
        }

        // Set tutorial controls to inactive
        if (scene.name == "Help Scene")
        {
            if (mirrorPlacementTut != null)
                mirrorPlacementTut.SetActive(false);

            if (playerTut != null)
            {
                playerTut.SetActive(false);
            }

            if (tutorialBoxes != null)
                tutorialBoxes.SetActive(false);
        }
        else
        {
            if (mirrorPlacementTut != null)
                mirrorPlacementTut.SetActive(false);

            if (playerTut != null)
            {
                playerTut.SetActive(false);
            }

            if (tutorialBoxes != null)
                tutorialBoxes.SetActive(false);
        }

        // Ensure win and lose menus are hidden at start
        if (winMenu != null)
            winMenu.SetActive(false);

        if (loseMenu != null)
            loseMenu.SetActive(false);

        if (restartButton != null)
            restartButton.SetActive(false);
    }

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
        enemiesRemaining = 0;
        CheckGameState();  // Check and trigger the elevator activation
    }

    public void EnemyKilled()
    {
        if (playerAnimator != null)
            playerAnimator.SetTrigger("celebrate");

        enemiesRemaining--;

        if (enemyNumber != null)
        {
            enemyNumber.text = enemiesRemaining.ToString();
        }
        else
        {
            Debug.LogError("Enemy number text reference is null.");
        }

        Debug.Log("Enemy killed. Remaining: " + enemiesRemaining);

        // Play enemy death sound
        if (audioManager != null && audioManager.enemyDeathClip != null)
        {
            audioManager.PlaySound(audioManager.enemyDeathClip);
        }
        else
        {
            Debug.LogError("AudioManager or enemyDeathClip is null. Cannot play death sound.");
        }

        CheckGameState(); // Check if the game should end
    }

    public void FireLaser()
    {
        shotsFired++;
        shotsRemaining--;
        if (shotBar != null)
            shotBar.SetShots(shotsRemaining);

        if (shotNumber != null)
        {
            shotNumber.text = shotsRemaining.ToString();
        }
        else
        {
            Debug.LogError("Shot number text reference is null.");
        }

        Debug.Log("Laser fired. Shots fired: " + shotsFired);

        // Play laser firing sound
        if (audioManager != null && audioManager.laserShotClip != null)
        {
            audioManager.PlaySound(audioManager.laserShotClip);
        }
        else
        {
            Debug.LogError("AudioManager is null. Cannot play laser shot sound.");
        }
    }

    public void CheckGameState()
    {
        Debug.Log("Checking game state. Shots fired: " + shotsFired + ", Enemies remaining: " + enemiesRemaining);

        // Win condition: No enemies left
        if (enemiesRemaining <= 0)
        {
            if (SceneManager.GetActiveScene().name == "Level3")
            {
                if (winMenu != null && loseMenu != null)
                {
                    winMenu.SetActive(true);
                    loseMenu.SetActive(false);
                    restartButton.SetActive(true);
                    Debug.Log("Win condition met - Final Level Completed");

                    // Play win sound
                    if (audioManager != null && audioManager.winClip != null)
                    {
                        audioManager.PlaySound(audioManager.winClip);
                    }
                    else
                    {
                        Debug.LogError("AudioManager or winClip is null. Cannot play win sound.");
                    }
                }
                else
                {
                    Debug.LogError("Win or Lose menu references are null.");
                }
            }
            else
            {
                Debug.Log("Enemies defeated. Unlocking the elevator for the next level.");
                if (elevatorAnimator != null)
                    elevatorAnimator.SetTrigger("levelComplete");
                if (elevatorCollider != null)
                    elevatorCollider.enabled = true;

                // Play the "all enemies defeated" sound
                if (audioManager != null && audioManager.allEnemiesDefeatedClip != null)
                {
                    audioManager.PlaySound(audioManager.allEnemiesDefeatedClip);
                }
                else
                {
                    Debug.LogError("AudioManager or allEnemiesDefeatedClip is null. Cannot play all enemies defeated sound.");
                }
            }
        }
        else if (shotsFired >= maxShots && enemiesRemaining > 0)
        {
            if (winMenu != null && loseMenu != null)
            {
                loseMenu.SetActive(true);
                winMenu.SetActive(false);
                restartButton.SetActive(true);
                Debug.Log("Lose condition met");

                // Play lose sound
                if (audioManager != null && audioManager.loseClip != null)
                {
                    audioManager.PlaySound(audioManager.loseClip);
                }
                else
                {
                    Debug.LogError("AudioManager or loseClip is null. Cannot play lose sound.");
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
        if (enemiesRemaining <= 0)
        {
            string currentLevel = SceneManager.GetActiveScene().name;
            nextLevelName = "";

            if (currentLevel == "LevelOne")
            {
                nextLevelName = "LevelTwo";
            }
            else if (currentLevel == "LevelTwo")
            {
                nextLevelName = "LevelThree";
            }
            else
            {
                Debug.LogError("No further levels found.");
                return;
            }

            Invoke("LoadLevel", 1);
            if (transitionAnimator != null)
                transitionAnimator.SetTrigger("End");
        }
        else
        {
            Debug.Log("Cannot enter the elevator until all enemies are defeated.");
        }
    }

    public void LoadLevel()
    {
        Debug.Log("Transitioning to next level: " + nextLevelName);
        SceneManager.LoadScene(nextLevelName);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Method to check if the player is detected by any enemy
    public bool IsPlayerDetected()
    {
        return detectionCount > 0;
    }

    // Method for enemies to set the player's detection status
    public void SetPlayerDetected(bool detected)
    {
        if (detected)
        {
            detectionCount++;
            Debug.Log("Player detected by an enemy. Detection count: " + detectionCount);
        }
        else
        {
            detectionCount = Mathf.Max(0, detectionCount - 1);
            Debug.Log("Player lost by an enemy. Detection count: " + detectionCount);
        }
    }
}
