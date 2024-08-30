using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public AudioSource audioSource;
    public int index;
    public bool isKeyDown;
    public int maxButton;
    public AudioClip backgroundMusic;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            if (!isKeyDown)
            {
                if (Input.GetAxis("Vertical") < 0)
                {
                    if (index < maxButton)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                else if (Input.GetAxis("Vertical") > 0)
                {
                    if (index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = maxButton;
                    }
                }
                isKeyDown = true;
            }

        }
        else
        {
            isKeyDown = false;
        }

    }
}
