using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private Scene scene;
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
        }
        else if (scene.name == "LevelOne")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelTwo");
        }
        else if(scene.name == "LevelTwo")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
        }
    }
    public void OnClickMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void OnClickHelpScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Help Scene");
    }
    public void OnClickOptionsScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
    }
}