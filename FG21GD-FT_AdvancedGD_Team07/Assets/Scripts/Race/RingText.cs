using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RingText : MonoBehaviour
{

    private TextMeshPro textMeshPro;
    private Color transparent = Color.clear;
    private Color originalColor;
    private float fadeTime = 1;

    // Start is called before the first frame update
    void Awake()
    {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();

        AnimateScale();
    }

    public void AnimateScale()
    {
        LeanTween.scale(gameObject,transform.localScale*1.5f, 2).setLoopPingPong().setEaseInOutSine();
    }


    public void SetText(string text)
    {
        textMeshPro.text = text;
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
