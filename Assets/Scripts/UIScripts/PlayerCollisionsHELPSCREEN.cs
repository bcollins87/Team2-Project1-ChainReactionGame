using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollisionsHELPSCREEN : MonoBehaviour
{
    public TMP_Text instructionsText;
    public GameObject helpPanel;
    public GameObject mirrorPlacePanel;
    public GameObject mirrorMovePanel;
    public GameObject laserShootPanel;
    public GameObject mirrorSetPanel;
    //public GameObject winPanel;


    // Start is called before the first frame update
    void Start()
    {
        mirrorMovePanel.SetActive(false);
        mirrorPlacePanel.SetActive(false);
        laserShootPanel.SetActive(false);
        mirrorSetPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        //This event/function handles trigger events (collsion between a game object with a rigid body)
        if (other.gameObject.tag == "Tutorial")
        {
            instructionsText.text = "Use the right click to place the mirror";
            Debug.Log("Collided");
            helpPanel.SetActive(false);
            mirrorPlacePanel.SetActive(true);
        }

        if (other.gameObject.tag == "MPTut")
        {
            instructionsText.text = "Use Q and E to rotate the mirror to the right and the left";
            mirrorPlacePanel.SetActive(false);
            mirrorMovePanel.SetActive(true);
        }

        if (other.gameObject.tag == "MMTut")
        {
            instructionsText.text = "Use left click to place the mirror";
            mirrorMovePanel.SetActive(false);
            laserShootPanel.SetActive(true);
        }

        if (other.gameObject.tag == "LSTut")
        {
            instructionsText.text = "Press the space bar to shoot the laser";
            mirrorSetPanel.SetActive(false);
            laserShootPanel.SetActive(false);
        }
    }
}
