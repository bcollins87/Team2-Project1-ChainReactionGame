using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public Animator animator;
    public int indexHere;
    public ButtonController buttonController;
    public AnimatorFunctions animatorFunctions;
    public Animator transitionAni;

    public GameObject backgroundGlow;

    private Scene scene;

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
        }
        else if (indexHere == 1)
        {
            if (scene.name == "Options")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
            }
        }
        else if (indexHere == 2)
        {
            if (scene.name == "Help")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Quit");
            }
            else 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Help Scene");
            }
        }
        else if (indexHere == 3)
        {
            if (scene.name == "MainMenu")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
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
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        transitionAni.SetTrigger("Start");
   }

    /*void SetIndexButtons()
    {
        scene = SceneManager.GetActiveScene();
        if (scene.name == "Main Menu")
        {
            if (indexHere == 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
            }
            else if (indexHere == 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
            }
            else if (indexHere == 2)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Help Scene");
            }
            else if (indexHere == 3)
            {
                Application.Quit();
            }
            else if (indexHere == 4)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
            }
        }
        else if (scene.name == "Options")
        {
            if (indexHere == 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
            }
            else if (indexHere == 2)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Help Scene");
            }
            else if (indexHere == 3)
            {
                Application.Quit();
            }
            else if (indexHere == 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
            }
            else if (indexHere == 4)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        else if (scene.name == "Help Scene")
        {
            if (indexHere == 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
            }
            else if (indexHere == 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
            }
            else if (indexHere == 2)
            {
                Application.Quit();
            }
            else if (indexHere == 3)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
            }
            else if (indexHere == 4)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        else if (scene.name == "LevelOne")
        {
            if (indexHere == 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
            }
            else if (indexHere == 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
            }
            else if (indexHere == 2)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Help Scene");
            }
            else if (indexHere == 3)
            {
                Application.Quit();
            }
            else if (indexHere == 4)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
            }
            else if (indexHere == 5)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        else if (scene.name == "Credits")
        {
            if (indexHere == 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LevelOne");
            }
            else if (indexHere == 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
            }
            else if (indexHere == 2)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Help Scene");
            }
            else if (indexHere == 3)
            {
                Application.Quit();
            }
            else if (indexHere == 4)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        else
        {
            Debug.Log("Scene not found");
        }
    }*/
}
