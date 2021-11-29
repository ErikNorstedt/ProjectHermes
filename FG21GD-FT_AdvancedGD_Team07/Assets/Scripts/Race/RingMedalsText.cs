using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RingMedalsText : MonoBehaviour
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

    }

    public void SetTimes(float goldTime, float silverTime, float bronzeTime)
    {
        string GoldText = "<sprite=0>" + TimeSpan.FromSeconds(goldTime).ToString("mm\\:ss\\.fff");
        string SilverText = "<sprite=1>" + TimeSpan.FromSeconds(silverTime).ToString("mm\\:ss\\.fff");
        string BronzeText = "<sprite=2>" + TimeSpan.FromSeconds(bronzeTime).ToString("mm\\:ss\\.fff");

        textMeshPro.text = GoldText + "\n" + SilverText + "\n" + BronzeText;
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
