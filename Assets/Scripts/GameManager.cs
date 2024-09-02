using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winMenu;
    public GameObject loseMenu;
    public int totalEnemies;

    private int enemiesRemaining;
    private int shotsFired = 0;
    private int shotsRemaining = 3;
    public int maxShots = 3;  // Maximum shots allowed

    public TMP_Text enemyNumber;
    public TMP_Text shotNumber;
    public GameObject restartButton;

    public static GameManager instance;

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
    }

    public void EnemyKilled()
    {
        if (enemyNumber != null)
        {
            enemiesRemaining--;
            enemyNumber.text = "" + enemiesRemaining;
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
        else
        {
            Debug.LogError("Enemy number text reference is null.");
        }
    }


    public void FireLaser()
    {
        shotsFired++;
        shotsRemaining--;
        if (shotNumber != null)
        {
            shotNumber.text = "" + shotsRemaining;
        }
        else
        {
            Debug.LogError("Shot Number TMP_Text is not assigned.");
        }
        Debug.Log("Laser fired. Shots fired: " + shotsFired);

        // Play laser firing sound
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.laserShotClip != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.laserShotClip);
            }
            else
            {
                Debug.LogError("Laser shot clip is not assigned in AudioManager.");
            }
        }
        else
        {
            Debug.LogError("AudioManager instance is null. Cannot play laser shot sound.");
        }
    }


    public void CheckGameState()
    {
        Debug.Log("Checking game state. Shots fired: " + shotsFired + ", Enemies remaining: " + enemiesRemaining);
        if (enemiesRemaining <= 0)
        {
            // Player wins if no enemies are left
            winMenu.SetActive(true);
            restartButton.SetActive(true);
            Debug.Log("Win condition met");

            // Play win sound
            AudioManager.Instance.PlaySound(AudioManager.Instance.winClip);
        }
        else if (shotsFired >= maxShots)
        {
            // Player loses if they run out of shots
            loseMenu.SetActive(true);
            restartButton.SetActive(true);
            Debug.Log("Lose condition met");

            // Play lose sound
            AudioManager.Instance.PlaySound(AudioManager.Instance.loseClip);
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
