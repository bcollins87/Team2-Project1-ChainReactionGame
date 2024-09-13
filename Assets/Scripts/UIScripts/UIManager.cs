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
            SceneManager.LoadScene("LevelOne");
           // gameManager.SetVariable();
        }
        else if (scene.name == "LevelOne")
        {
            SceneManager.LoadScene("LevelTwo");

        }
        else if(scene.name == "LevelTwo")
        {
            SceneManager.LoadScene("LevelThree");
           // gameManager.tutorialStuff.SetActive(false);

        }
        else if(scene.name == "LevelThree")
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("LevelOne");
           // gameManager.SetVariable();
        }
    }
    public void OnClickMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        //gameManager.SetVariable();
    }
    public void OnClickHelpScene()
    {
        SceneManager.LoadScene("Help Scene");
       // gameManager.SetVariable();
    }
    public void OnClickOptionsScene()
    {
        SceneManager.LoadScene("Options");
       // gameManager.SetVariable();
    }
    public void OnClickWinScene()
    {
        SceneManager.LoadScene("WinScene");
    }
    public void OnClickCreditsScene()
    {
        
        SceneManager.LoadScene("Credits");
        Debug.Log("Credits Clicked");
    }
    public void OnClickRestartScene()
    {
        scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}