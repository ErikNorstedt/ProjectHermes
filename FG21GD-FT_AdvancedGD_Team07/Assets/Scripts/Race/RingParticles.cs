using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingParticles : MonoBehaviour
{

    private float scaleMultiplier = 1.5f;
    private ParticleSystem particles;
    private Vector3 originalScale;
    private Color transparent = Color.clear;
    private float fadeTime = 1f;
    private Color currentColor;

    void Awake()
    {
        particles = gameObject.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule psMain = particles.main;

        originalScale = gameObject.transform.localScale;
        currentColor = psMain.startColor.color;
        
    }

    private void Update()
    {
        ParticleSystem.MainModule psMain = particles.main;
        psMain = particles.main;
        psMain.startColor = currentColor;
    }

    public void AnimateScale()
    {
        LeanTween.scale(gameObject, transform.localScale * scaleMultiplier, fadeTime).setEaseOutSine().setLoopOnce();
        
        LeanTween.value(gameObject, (value) =>
        {

            //Debug.Log(value);
            currentColor = value;

        }, Color.white, transparent, fadeTime).setEaseOutSine();
        

    }

    public void ResetParticles()
    {
        ParticleSystem.MainModule psMain = particles.main;

        currentColor = Color.white;
        psMain.startColor = Color.white;
        gameObject.transform.localScale = originalScale;

    }

}
