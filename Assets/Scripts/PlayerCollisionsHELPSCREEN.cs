using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollisionsHELPSCREEN : MonoBehaviour
{
    public TMP_Text instructionsText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tutorial")
        {
            instructionsText.text = "Use the left click to place the mirror";
            Debug.Log("Collided");
            if (Input.GetMouseButtonDown(0))
            {
                instructionsText.text = "Use Q and E to rotate the mirror to the right and the left";
                if (Input.GetKeyDown("Q") || Input.GetKeyDown("E"))
                {
                    instructionsText.text = "Use right click to place the mirror";
                    if (Input.GetMouseButtonDown(1))
                    {
                        instructionsText.text = "Press the space bar to shoot the laser";
                    }
                }
            }

        }
    }
}
