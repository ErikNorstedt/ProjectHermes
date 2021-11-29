using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RingBestTimesText : MonoBehaviour
{

    private TextMeshPro textMeshPro;
    private Color transparent = Color.clear;
    private Color originalColor;
    private float fadeTime = 1;
    private Race race;

    // Start is called before the first frame update
    void Awake()
    {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
        race = GetComponentInParent<Race>();
        AnimateScale();
    }

    public void AnimateScale()
    {

        LeanTween.value(gameObject, a => textMeshPro.fontSize = a, textMeshPro.fontSize, textMeshPro.fontSize + 2, 2).setLoopPingPong().setEaseInOutSine();

        //LeanTween.scale(gameObject,transform.localScale*1.001f, 2).setLoopPingPong().setEaseInOutSine();
    }


    public void SetTimes(float goldTime, float silverTime, float bronzeTime)
    {
        float currentGold = PlayerPrefs.GetFloat(race.GetRaceId() + "_best");
        float currentSilver = PlayerPrefs.GetFloat(race.GetRaceId() + "_second");
        float currentBronze = PlayerPrefs.GetFloat(race.GetRaceId() + "_third");

        string currentGoldText = GetSprite(goldTime, silverTime, bronzeTime, currentGold) + TimeSpan.FromSeconds(currentGold).ToString("mm\\:ss\\.fff");
        string currentSilverText = GetSprite(goldTime, silverTime, bronzeTime, currentSilver) + TimeSpan.FromSeconds(currentSilver).ToString("mm\\:ss\\.fff");
        string currentBronzeText = GetSprite(goldTime, silverTime, bronzeTime, currentBronze) + TimeSpan.FromSeconds(currentBronze).ToString("mm\\:ss\\.fff");

        textMeshPro.text = currentGoldText + "\n" + currentSilverText + "\n" + currentBronzeText;
    }

    private static string GetSprite(float goldTime, float silverTime, float bronzeTime, float compareValue)
    {
        if(compareValue < goldTime)
            return "<sprite=0>";
        else if(compareValue > goldTime && compareValue < silverTime)
            return "<sprite=1>";
        else if (compareValue < silverTime)
            return "<sprite=2>";
        else if(compareValue < 0.1f)
            return "<sprite=2>";
        else 
            return "<sprite=2>";
    }

    public void SetColor(Color color)
    {
        textMeshPro.color = color;
        originalColor = color;
    }

    public void FadeIn()
    {
        LeanTween.value(gameObject, a => textMeshPro.color = a, transparent, originalColor, fadeTime);
    }

    public void FadeOut()
    {
        LeanTween.value(gameObject, a => textMeshPro.color = a, textMeshPro.color, transparent, fadeTime);
    }

}
