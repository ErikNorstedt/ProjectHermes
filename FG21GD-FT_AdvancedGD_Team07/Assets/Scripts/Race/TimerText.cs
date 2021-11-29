using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerText : MonoBehaviour
{
    private TextMeshProUGUI textMeshProUGUI;


    void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    public TextMeshProUGUI GetText()
    {
        return textMeshProUGUI;
    }

    public void SetText(string text)
    {
        textMeshProUGUI.text = text;
    }

  
}
