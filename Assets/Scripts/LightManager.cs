using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public List<Light> lights;  // List of all lights in the scene

    private bool lightsOn = false; // Track the current state of the lights

    void Start()
    {
        // Initialize lights based on the starting state
        UpdateLights();
    }

    public void TurnLightsOn()
    {
        lightsOn = true;
        UpdateLights();
        Debug.Log("Lights turned on!");
    }

    public void TurnLightsOff()
    {
        lightsOn = false;
        UpdateLights();
        Debug.Log("Lights turned off!");
    }

    public bool AreLightsOn()
    {
        return lightsOn;
    }

    private void UpdateLights()
    {
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.enabled = lightsOn;
            }
        }
    }
}
