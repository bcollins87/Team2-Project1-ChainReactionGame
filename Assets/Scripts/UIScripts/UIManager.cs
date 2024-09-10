using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private Scene scene;
    public GameManager gameManager;
    public MirrorPlacement mirrorPlacement;
    private int mirrorsLeft;
    public MenuLoader menuLoader;

    public void OnClickQuitButton()
    {
        Application.Quit();
    }
    public void OnClickLevelOne()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "MainMenu")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
           // gameManager.SetVariable();
        }
        else if (scene.name == "LevelOne")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelTwo");
            gameManager.tutorialStuff.SetActive(false);
            //Change Variables
            gameManager.winMenu.SetActive(false);
            gameManager.mirrorPlacementTut.SetActive(false);
            gameManager.playerTut.SetActive(false);
            gameManager.mirrorPlacement.SetActive(true);
            gameManager.tutorialBoxes.SetActive(false);
            gameManager.playerTut.SetActive(false);
            gameManager.activePlayer = gameManager.player;
            Cursor.visible = true;
            gameManager.totalEnemies = 3;
            gameManager.enemiesRemaining = gameManager.totalEnemies;
            mirrorPlacement.maxMirrors = 6;
            mirrorsLeft = mirrorPlacement.maxMirrors - mirrorPlacement.mirrorsPlaced;
            gameManager.shotsRemaining = 3;

            Cursor.visible = true;
            gameManager.shotNumber.text = "" + gameManager.shotsRemaining;
            gameManager.enemyNumber.text = "" + gameManager.totalEnemies;
            gameManager.mirrorNumber.text = "" + mirrorsLeft;
            menuLoader.UpdateSceneMenu();
            Debug.Log("Level Two Loaded");
            //gameManager.SetVariable();
        }
        else if(scene.name == "LevelTwo")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelThree");
            gameManager.tutorialStuff.SetActive(false);
            gameManager.winMenu.SetActive(false);
            gameManager.mirrorPlacementTut.SetActive(false);
            gameManager.playerTut.SetActive(false);
            gameManager.mirrorPlacement.SetActive(true);
            gameManager.tutorialBoxes.SetActive(false);
            gameManager.playerTut.SetActive(false);
            gameManager.activePlayer = gameManager.player;
            Cursor.visible = true;
            gameManager.totalEnemies = 4;
            gameManager.enemiesRemaining = gameManager.totalEnemies;
            mirrorPlacement.maxMirrors = 7;
            mirrorsLeft = mirrorPlacement.maxMirrors - mirrorPlacement.mirrorsPlaced;
            gameManager.shotsRemaining = 3;

            Cursor.visible = true;
            gameManager.shotNumber.text = "" + gameManager.shotsRemaining;
            gameManager.enemyNumber.text = "" + gameManager.totalEnemies;
            gameManager.mirrorNumber.text = "" + mirrorsLeft;
            menuLoader.UpdateSceneMenu();
            Debug.Log("Level Three Loaded");
            // gameManager.SetVariable();
        }
        else if(scene.name == "LevelThree")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
           // gameManager.SetVariable();
        }
    }
    public void OnClickMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        //gameManager.SetVariable();
    }
    public void OnClickHelpScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Help");
       // gameManager.SetVariable();
    }
    public void OnClickOptionsScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
       // gameManager.SetVariable();
    }
    public void OnClickWinScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("WinScene");
    }
    public void OnClickCreditsScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }
    public void OnClickRestartScene()
    {
        scene = SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
    }
}