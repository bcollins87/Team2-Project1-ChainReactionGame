using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public Animator animator;
    public int indexHere;
    public ButtonController buttonController;
    public AnimatorFunctions animatorFunctions;
    public Animator transitionAni;

    public GameObject MainMenuUI;
    public GameObject OptionsMenuUI;
    public GameObject CreditsMenuUI;
    public GameObject HelpMenuUI;
    public GameObject PlayerUI;


    void Update()
    {
        if (buttonController.index == indexHere)
        {
            animator.SetBool("selected", true);
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
        }
    }
    
   IEnumerator LoadLevel()
    {
        transitionAni.SetTrigger("End");
        yield return new WaitForSeconds(1);
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
        transitionAni.SetTrigger("Start");
   }
}
