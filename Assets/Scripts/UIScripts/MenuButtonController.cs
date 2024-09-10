using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public Animator animator;
    public int indexHere;
    public ButtonController buttonController;
    public AnimatorFunctions animatorFunctions;
    public Animator transitionAni;

    public GameObject backgroundGlow;
    public GameManager gameManager;

    private Scene scene;
    private int mirrorsLeft;
    public MenuLoader menuLoader;
    public MirrorPlacement mirrorPlacement;
    public PlayerCollisionsHELPSCREEN playerCollisionsHELPSCREEN;
    

    void Update()
    {
        if (buttonController.index == indexHere)
        {
            animator.SetBool("selected", true);
            backgroundGlow.SetActive(true);
            if (Input.GetAxis("Submit") == 1)
            {
                animator.SetBool("pressed", true);
                StartCoroutine(LoadLevel());
            }
            else if(animator.GetBool("pressed"))
            {
                animator.SetBool("pressed", false);
                animatorFunctions.disableOnce = true;
            }
        }
        else
        {
            animator.SetBool("selected", false);
            backgroundGlow.SetActive(false);
        }
    }
    
   IEnumerator LoadLevel()
   {
        scene = SceneManager.GetActiveScene();
        transitionAni.SetTrigger("End");
        yield return new WaitForSeconds(1);
        if (indexHere == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");

            //Change Variables
            gameManager.mirrorPlacementTut.SetActive(false);
            gameManager.playerTut.SetActive(false);
            gameManager.mirrorPlacement.SetActive(true);
            gameManager.tutorialBoxes.SetActive(false);
            gameManager.playerTut.SetActive(false);
            gameManager.activePlayer = gameManager.player;
            Cursor.visible = true;
            gameManager.totalEnemies = 2;
            gameManager.enemiesRemaining = gameManager.totalEnemies;
            mirrorPlacement.maxMirrors = 5;
            mirrorsLeft = mirrorPlacement.maxMirrors - mirrorPlacement.mirrorsPlaced;
            gameManager.shotsRemaining = 3;
            Debug.Log(gameManager.totalEnemies);
            Debug.Log(mirrorsLeft);
            Debug.Log(gameManager.shotsRemaining);
            Cursor.visible = true;
            gameManager.shotNumber.text = "" + gameManager.shotsRemaining;
            gameManager.enemyNumber.text = "" + gameManager.totalEnemies;
            gameManager.mirrorNumber.text = "" + mirrorsLeft;
            menuLoader.UpdateSceneMenu();
            Debug.Log("Level One Loaded");

        }
        else if (indexHere == 1)
        {
            if (scene.name == "Options")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
                gameManager.mirrorPlacementTut.SetActive(false);
                gameManager.playerTut.SetActive(false);
                gameManager.tutorialBoxes.SetActive(false);
                Cursor.visible = false;
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
                gameManager.mirrorPlacementTut.SetActive(false);
                gameManager.playerTut.SetActive(false);
                gameManager.tutorialBoxes.SetActive(false);
                Cursor.visible = false;
            }
        }
        else if (indexHere == 2)
        {
            if (scene.name == "Help Scene")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Quit");
            }
            else 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Help");


                /*
                //Change Variables for Health 
                gameManager.mirrorPlacementTut.SetActive(true);
                gameManager.mirrorPlacement.SetActive(false);
                gameManager.playerTut.SetActive(true);
                gameManager.tutorialBoxes.SetActive(true);

                //Set triggers
                playerCollisionsHELPSCREEN.mirrorMovePanel.SetActive(false);
                playerCollisionsHELPSCREEN.mirrorPlacePanel.SetActive(false);
                playerCollisionsHELPSCREEN.laserShootPanel.SetActive(false);
                playerCollisionsHELPSCREEN.mirrorSetPanel.SetActive(false);
                gameManager.player.SetActive(false);
                gameManager.activePlayer = gameManager.playerTut;
                //Cursor.visible = true;
                gameManager.totalEnemies = 1;
                gameManager.enemiesRemaining = gameManager.totalEnemies;
                mirrorPlacement.maxMirrors = 3;
                mirrorsLeft = mirrorPlacement.maxMirrors - mirrorPlacement.mirrorsPlaced;
                gameManager.shotsRemaining = 3;
                Debug.Log(gameManager.totalEnemies);
                Debug.Log(mirrorsLeft);
                Debug.Log(gameManager.shotsRemaining);
                Cursor.visible = true;
                gameManager.shotNumber.text = "" + gameManager.shotsRemaining;
                gameManager.enemyNumber.text = "" + gameManager.totalEnemies;
                gameManager.mirrorNumber.text = "" + mirrorsLeft;
                menuLoader.UpdateSceneMenu();
                Debug.Log("This is working");
                */
            }
        }
        else if (indexHere == 3)
        {
            if (scene.name == "MainMenu")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
                Cursor.visible = false;
                gameManager.mirrorPlacementTut.SetActive(false);
                gameManager.playerTut.SetActive(false);
                gameManager.tutorialBoxes.SetActive(false);
            }
            
            else
            {
                Application.Quit();
            }
        }
        else if (indexHere == 4)
        {
            if (scene.name == "MainMenu")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
                Cursor.visible = false;
                gameManager.mirrorPlacementTut.SetActive(false);
                gameManager.playerTut.SetActive(false);
                gameManager.tutorialBoxes.SetActive(false);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                Cursor.visible = false;
                gameManager.mirrorPlacementTut.SetActive(false);
                gameManager.playerTut.SetActive(false);
                gameManager.tutorialBoxes.SetActive(false);
            }
        }
        transitionAni.SetTrigger("Start");
   }

   
}
