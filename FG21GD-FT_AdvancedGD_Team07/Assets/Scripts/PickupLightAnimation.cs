using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupLightAnimation : MonoBehaviour
{
    private Light light;

    void Start()
    {
        light = GetComponent<Light>();
    }


    public void TurnOff()
    {
        LeanTween.value(gameObject, a => light.intensity = a, light.intensity, 0, 0.2f);
    }

}
