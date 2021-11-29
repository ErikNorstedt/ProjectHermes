using System;
using UnityEngine;
using TMPro;


public class Ring : MonoBehaviour
{

    private int ringNumber;
    private Race race;
    private ParticleSystem particles;
    private BoxCollider boxCollider;
    private Color activeColor = Color.white;
    private Color inactiveColor = Color.clear;
    private RingText ringText;
    private RingParticles ringParticles;
    private RaceManager raceManager;
    private AudioSource audioSource;
    private RingBestTimesText ringBestTimesText;
    private RingMedalsText ringMedalsText;
    private float ringHitVolume = 0.2f;

    public static Action<Transform> onRingEntered;

    // Start is called before the first frame update
    void Awake()
    {
        raceManager = FindObjectOfType<RaceManager>();
        race = gameObject.GetComponentInParent<Race>();
        particles = gameObject.GetComponentInChildren<ParticleSystem>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        ringText = gameObject.GetComponentInChildren<RingText>();
        ringParticles = gameObject.GetComponentInChildren<RingParticles>();
        audioSource = gameObject.GetComponent<AudioSource>();
        ringBestTimesText = GetComponentInChildren<RingBestTimesText>();
        ringMedalsText = GetComponentInChildren<RingMedalsText>();
        audioSource.volume = ringHitVolume;
        
    }


    private void OnDrawGizmos()
    {
        Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(16, 16, 1));
        Gizmos.matrix = matrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }


    public void SetRingNumber(int i, Color ringColor, Color startAndFinishColor)
    {
        ringNumber = i;

        ///Debug.Log(race);

        if (ringNumber == 0)
        {
            ringText.SetText("START");
            ringText.SetColor(startAndFinishColor);
            ringBestTimesText.SetColor(startAndFinishColor);
            ringMedalsText.SetColor(startAndFinishColor);

        } else if (ringNumber == race.getAmountOfRings()-1)
        {
            ringText.SetText("FINISH");
            ringText.SetColor(startAndFinishColor);

        } else
        {
            ringText.SetText(ringNumber.ToString());
            ringText.SetColor(ringColor);
        }


    }

    public void ResetRing(bool isStartRing)
    {
        //Debug.Log("Ring: " + ringNumber + " reset");
        ringParticles.ResetParticles();
        ringText.FadeIn();
        boxCollider.enabled = true;

        if (isStartRing)
        {
            ringBestTimesText.FadeIn();

        }
    }

    public void AnimateRing(bool isStartRing)
    {
        ringParticles.AnimateScale();
        ringText.FadeOut();
        
        boxCollider.enabled = false;

        if (isStartRing)
        {
            ringBestTimesText.FadeOut();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float rightDir = Vector3.Dot(other.gameObject.transform.forward, transform.forward);
            Debug.Log(rightDir);
            if (rightDir < 0.1f) return;

            if (ringNumber == 0 && !raceManager.RaceIsActive())
            {
                race.StartRace();
                AnimateRing(true);
                audioSource.Play();

                onRingEntered?.Invoke(race.getNextRingToPassTransform());

            } else if (ringNumber == race.getNextRingToPass())
            {
                race.passRing();
                audioSource.Play();
                AnimateRing(false);


                if (ringNumber == race.getAmountOfRings() - 1)
                {
                    Debug.Log("Won race!");
                    race.WinRace();
                }
                else
                    onRingEntered?.Invoke(race.getNextRingToPassTransform());
            }
        }
    }
}
