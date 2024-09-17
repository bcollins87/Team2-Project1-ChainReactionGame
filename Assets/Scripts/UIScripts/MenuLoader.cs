using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject optionsMenuUI;
    public GameObject creditsMenuUI;
    public GameObject helpMenuUI;
    public GameObject playerUI;
    public GameObject playerTut;
    public GameObject mirrorPlacement;
    private Scene scene;

    public GameManager gameManager;
    public GameObject playerMovement;

    private bool isMenuActive = false;
    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);
        helpMenuUI.SetActive(false);
        playerUI.SetActive(false);
        //CheckActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSceneMenu();
    }

    public void CheckActiveScene()
    {
        scene = SceneManager.GetActiveScene();
        Debug.Log("Active Scene is '" + scene.name + "'.");
    }

    public void UpdateSceneMenu()
    {
        scene = SceneManager.GetActiveScene();
        Debug.Log("Active Scene is '" + scene.name + "'.");

        if (scene.name == "MainMenu")
        {
            mainMenuUI.gameObject.SetActive(true);
            optionsMenuUI.SetActive(false);
            creditsMenuUI.SetActive(false);
            helpMenuUI.SetActive(false);
            playerUI.SetActive(false);
        }
        else if (scene.name == "Options")
        {
            optionsMenuUI.gameObject.SetActive(true);
            mainMenuUI.SetActive(false);
            creditsMenuUI.SetActive(false);
            helpMenuUI.SetActive(false);
            playerUI.SetActive(false);
        }
        else if (scene.name == "Help Scene")
        {
            if(Input.GetKeyDown(KeyCode.M) && !isMenuActive)
            {
                Debug.Log("Help Menu Displayed");
                helpMenuUI.gameObject.SetActive(true);
                isMenuActive = true;
                gameManager.player.SetActive(false);
                playerMovement.SetActive(false);
            }
            else if ((Input.GetKeyDown(KeyCode.M) && isMenuActive))
            {
                helpMenuUI.gameObject.SetActive(false);
                isMenuActive = false;
                gameManager.player.SetActive(true);
                playerMovement.SetActive(true);
            }
            mainMenuUI.SetActive(false);
            optionsMenuUI.SetActive(false);
            creditsMenuUI.SetActive(false);
            playerUI.SetActive(true);
        }
        else if (scene.name == "Help Scene 2")
        {
            if (Input.GetKeyDown(KeyCode.M) && !isMenuActive)
            {
                Debug.Log("Help Menu Displayed");
                helpMenuUI.gameObject.SetActive(true);
                isMenuActive = true;
                gameManager.player.SetActive(false);
                playerMovement.SetActive(false);
            }
            else if ((Input.GetKeyDown(KeyCode.M) && isMenuActive))
            {
                helpMenuUI.gameObject.SetActive(false);
                isMenuActive = false;
                gameManager.player.SetActive(true);
                playerMovement.SetActive(true);
            }
            mainMenuUI.SetActive(false);
            optionsMenuUI.SetActive(false);
            creditsMenuUI.SetActive(false);
            playerUI.SetActive(true);
        }
        else if (scene.name == "LevelOne")
        {
            playerUI.gameObject.SetActive(true);
            mainMenuUI.SetActive(false);
            optionsMenuUI.SetActive(false);
            creditsMenuUI.SetActive(false);
            helpMenuUI.SetActive(false);

        }
        else if (scene.name == "LevelTwo")
        {
            playerUI.gameObject.SetActive(true);
            mainMenuUI.SetActive(false);
            optionsMenuUI.SetActive(false);
            creditsMenuUI.SetActive(false);
            helpMenuUI.SetActive(false);

        }
        else if (scene.name == "Credits")
        {
            creditsMenuUI.gameObject.SetActive(true);
            mainMenuUI.SetActive(false);
            optionsMenuUI.SetActive(false);
            helpMenuUI.SetActive(false);
            playerUI.SetActive(false);
        }
        else
        {
            Debug.Log("Scene not found");
        }
    }
}
