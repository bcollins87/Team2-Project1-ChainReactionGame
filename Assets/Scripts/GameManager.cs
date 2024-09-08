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
    

    public GameObject activePlayer;

    //UI Update Vars
    public GameObject mirrorPlacement;
    private int mirrorsLeft;
    public MenuLoader menuLoader;
    public GameObject mirrorPlacementTut;
    public GameObject playerTut;
    public GameObject tutorialBoxes;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        enemiesRemaining = totalEnemies;
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
        restartButton.SetActive(false);

        // Update the UI with initial values
        enemyNumber.text = enemiesRemaining.ToString();
        shotNumber.text = shotsRemaining.ToString();
        shotBar.SetMaxShots(maxShots);
        menuLoader.CheckActiveScene();
        menuLoader.UpdateSceneMenu();


        //Set tutorial controls to inactive
        mirrorPlacementTut.SetActive(false);
        playerTut.SetActive(false);
        tutorialBoxes.SetActive(false);
    }


    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {

        }
    }

    public void EnemyKilled()
    {
        player.GetComponent<PlayerMovement>().animator.SetTrigger("celebrate");
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
            if (winMenu != null && loseMenu != null)
            {
                winMenu.SetActive(true);
                loseMenu.SetActive(false);  // Ensure the lose menu is not active
                restartButton.SetActive(true);
                Debug.Log("Win condition met");
               

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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
