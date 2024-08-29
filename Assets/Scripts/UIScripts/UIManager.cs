using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void OnClickQuitButton()
    {
        Application.Quit();
    }
    public void OnClickLevelOne()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
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
}