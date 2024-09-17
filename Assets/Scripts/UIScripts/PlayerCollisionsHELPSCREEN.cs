using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public GameObject mirrorPickUpPanel;
    public GameObject laserTracePanel;
    //public GameObject winPanel;

    //bools to disable actions
    public bool mirrorActive;
    public bool mirrorPickUp;
    public bool showLaser;
    public bool shootLaser;
    public bool rotateMirror;
    public bool placeMirror;
    public bool pickUpMirror;
    public bool laserTrace;


    // Start is called before the first frame update
    void Start()
    {
        mirrorMovePanel.SetActive(false);
        mirrorPlacePanel.SetActive(false);
        laserShootPanel.SetActive(false);
        mirrorSetPanel.SetActive(false);
        mirrorPickUpPanel.SetActive(false);
        laserTracePanel.SetActive(false);
        mirrorActive = false;
        //mirrorPickUp = false;
        shootLaser = false;
        showLaser = false;
        rotateMirror = false;
        placeMirror = false;
        pickUpMirror = false;
        laserTrace = false;
        Debug.Log("All Objects Loaded");
    }

    private void OnTriggerEnter(Collider other)
    {
        //This event/function handles trigger events (collsion between a game object with a rigid body)
        if (other.gameObject.tag == "Tutorial")
        {
            mirrorActive = true;
            
            if (mirrorActive)
            {
                Debug.Log("Mirrors Active is true");
                instructionsText.text = "Use the right click to place the mirror";
                Debug.Log("Collided");
                helpPanel.SetActive(false);
                mirrorPlacePanel.SetActive(true);
                
            }
        }

        if (other.gameObject.tag == "MPTut")
        {
            instructionsText.text = "Use Q and E to rotate the mirror to the right and the left";
            rotateMirror = true;
            mirrorPlacePanel.SetActive(false);
            mirrorMovePanel.SetActive(true);
        }

        if (other.gameObject.tag == "MMTut")
        {
            instructionsText.text = "Use left click to place the mirror";
            placeMirror = true;
            mirrorMovePanel.SetActive(false);
            mirrorPickUpPanel.SetActive(true);
        }
        if (other.gameObject.tag == "PickUpTut")
        {
            instructionsText.text = "Press the F key to pick up the mirror";
            pickUpMirror = true;
            mirrorPickUpPanel.SetActive(false);
            laserTracePanel.SetActive(true);
        }
        if (other.gameObject.tag == "LaserTraceTut")
        {
            instructionsText.text = "Press L to trace the laser";
            laserTrace = true;
            laserTracePanel.SetActive(false);
            laserShootPanel.SetActive(true);
        }

        if (other.gameObject.tag == "LSTut")
        {
            shootLaser = true;
            instructionsText.text = "Press the space bar to shoot the laser";
            laserShootPanel.SetActive(false);
        }
    }
}
