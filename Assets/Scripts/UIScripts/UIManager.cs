using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private Scene scene;
    public GameManager gameManager;

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
            //gameManager.SetVariable();
        }
        else if(scene.name == "LevelTwo")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
           // gameManager.SetVariable();
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