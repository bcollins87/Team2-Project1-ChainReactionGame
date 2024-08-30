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

    public AudioSource audioSource;
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
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
        enemiesRemaining--;
        enemyNumber.text = "" + enemiesRemaining;
        Debug.Log("Enemy killed. Remaining: " + enemiesRemaining);
        // CheckGameState(); // Now called from Laser script after it stops firing
    }

    public void FireLaser()
    {
        shotsFired++;
        shotsRemaining--;
        shotNumber.text = "" + shotsRemaining;
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
            restartButton.SetActive(true);
            Debug.Log("Win condition met");
        }
        else if (shotsFired >= maxShots)
        {
            // Player loses if they run out of shots
            loseMenu.SetActive(true);
            restartButton.SetActive(true);
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
