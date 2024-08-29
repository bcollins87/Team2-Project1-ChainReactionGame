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

    void Update()
    {
        if (buttonController.index == indexHere)
        {
            animator.SetBool("selected", true);

            if (Input.GetAxis("Submit") == 1)
            {
                animator.SetBool("pressed", true);
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
}
