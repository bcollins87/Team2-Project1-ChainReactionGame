using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShotBar : MonoBehaviour
{

    public Slider shotSlider;
    // Start is called before the first frame update
    public void SetMaxShots(int maxShots)
    {
        shotSlider.maxValue = maxShots;
        shotSlider.value = maxShots;
    }

    public void SetShots(int shots)
    {
        shotSlider.value = shots;
    }
}
