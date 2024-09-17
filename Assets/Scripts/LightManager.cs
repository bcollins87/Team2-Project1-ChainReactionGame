using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public List<Light> lights;  // List of all lights in the scene

    void Start()
    {
        // Turn off all lights at the start of the game
        TurnLightsOff();
    }

    public void TurnLightsOn()
    {
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
        Debug.Log("Lights turned on!");
    }

    public void TurnLightsOff()
    {
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
        Debug.Log("Lights turned off!");
    }

    // This is the method you need
    public bool AreLightsOn()
    {
        foreach (Light light in lights)
        {
            if (light.enabled)
            {
                return true; // If any light is on, return true
            }
        }
        return false; // All lights are off
    }
}
