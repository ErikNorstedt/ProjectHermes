using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Race : MonoBehaviour
{

    [Header("Rings")]
    [Tooltip("Add all rings GameObjects to the list in the order you expect to player to finish them")]
    private GameObject[] rings;
    [SerializeField] private Transform ringHolder;

    [Header("Specific Race Settings")]
    //[SerializeField] private float raceTime;
    [HideInInspector] public int raceId;
    [SerializeField] private float goldTime;
    [SerializeField] private float silverTime;
    [SerializeField] private float bronzeTime;

    [Header("General Race Settings (Do not change)")]
    [SerializeField] private Color startAndFinishColor;

    [SerializeField] private Color ringColor;

    [SerializeField] private AudioClip winAudio;
    [SerializeField] private AudioClip loseAudio;

    public static event Action onRaceStarted;
    public static event Action onRaceEnded;

    private float audioVolume = 0.2f;

    private float timeBeforeReset = 3f;

    private TimerText timerText;

    private int amountOfRings;

    private int lastRingPassed;

    private int nextRingToPass = 1;

    private bool raceStarted;

    private float currentTime;

    private AudioSource audioSource;

    private RaceManager raceManager;

    private RingBestTimesText ringBestTimesText;

    public Transform resetSpawnPos;

    private void Awake()
    {
        rings = new GameObject[ringHolder.childCount];

        for (int i = 0; i < rings.Length; i++)
            rings[i] = ringHolder.GetChild(i).gameObject;
    }

    void Start()
    {

        amountOfRings = rings.Length;

        timerText = FindObjectOfType<TimerText>();
        raceManager = FindObjectOfType<RaceManager>();

        ringBestTimesText = GetComponentInChildren<RingBestTimesText>();
        GetComponentInChildren<RingMedalsText>().SetTimes(goldTime, silverTime, bronzeTime);

        ringBestTimesText.SetTimes(goldTime, silverTime, goldTime);

        //Debug.Log(rings.Length);

        audioSource = GetComponent<AudioSource>();


        for (int i = 0; i < amountOfRings; i++)
        {
            rings[i].GetComponentInChildren<Ring>().SetRingNumber(i, ringColor, startAndFinishColor);

            if (i != 0)
            {
                rings[i].SetActive(false);

            }
        }
    }

    private void Update()
    {
        if (raceStarted)
        {
            currentTime += Time.deltaTime;
          
            string text = TimeSpan.FromSeconds(currentTime).ToString("mm\\:ss\\.fff");

            timerText.SetText(text);

        }
    }


    public void StartRace()
    {
        ///Debug.Log("Start race");
        raceStarted = true;
        raceManager.SetRaceActive(true, this);

        onRaceStarted?.Invoke();

        timerText.SetText("Race started!");

        for (int i = 0; i < amountOfRings; i++)
        {
            rings[i].SetActive(true);

        }
    }


    public int getLastRingPassed()
    {
        return lastRingPassed;
    }

    public int getNextRingToPass()
    {
        return nextRingToPass;
    }

    public Transform getNextRingToPassTransform()
    {
        if(nextRingToPass >= amountOfRings)
            return rings[0].transform;
        else
            return rings[nextRingToPass].transform;
    }

    public int getAmountOfRings()
    {
        return amountOfRings;
    }

    public void passRing()
    {
        lastRingPassed++;
        nextRingToPass++;
    }

    public void hideStart()
    {
        rings[0].GetComponentInChildren<Ring>().AnimateRing(true);
    }

    public void unhideStart()
    {
        rings[0].GetComponentInChildren<Ring>().ResetRing(true);
    }

    public int GetRaceId()
    {
        return raceId;
    }

    public void LoseRace()
    {
        timerText.SetText("Race Lost!");

        audioSource.volume = audioVolume;
        audioSource.PlayOneShot(loseAudio);

        onRaceEnded?.Invoke();

        for (int i = 0; i < amountOfRings; i++)
        {
            
            if (i != 0)
            {
                rings[i].GetComponentInChildren<Ring>().AnimateRing(false);

            }

        }


        EndRace();
    }

    public void WinRace()
    {
        onRaceEnded?.Invoke();
        audioSource.volume = audioVolume;
        audioSource.PlayOneShot(winAudio);
        timerText.SetText("Race Won!");
        float raceTime = currentTime;
        SaveTime(raceTime);
        EndRace();
    }

    public void SaveTime(float raceTime)
    {

        float currentBest = PlayerPrefs.GetFloat(raceId + "_best");
        float currentSecond = PlayerPrefs.GetFloat(raceId + "_second");
        float currentThird = PlayerPrefs.GetFloat(raceId + "_third");
;

        if (raceTime < currentBest)
        {

            PlayerPrefs.SetFloat(raceId + "_best", raceTime);
            PlayerPrefs.SetFloat(raceId + "_second", currentBest);
            PlayerPrefs.SetFloat(raceId + "_third", currentSecond);


        } else if (raceTime == currentBest)
        {
            PlayerPrefs.SetFloat(raceId + "_second", raceTime);
            PlayerPrefs.SetFloat(raceId + "_third", currentSecond);

        } else if (raceTime < currentSecond && raceTime > currentBest)
        {
            PlayerPrefs.SetFloat(raceId + "_second", raceTime);
            PlayerPrefs.SetFloat(raceId + "_third", currentSecond);

        } else if (raceTime == currentSecond) {

            PlayerPrefs.SetFloat(raceId + "_third", raceTime);

        } else if (raceTime < currentThird && raceTime > currentSecond)
        {
            PlayerPrefs.SetFloat(raceId + "_third", raceTime);

        } else if (currentBest == 0)
        {
            PlayerPrefs.SetFloat(raceId + "_best", raceTime);

        } else if (currentSecond == 0)
        {
            PlayerPrefs.SetFloat(raceId + "_second", raceTime);
        }
        else if (currentThird == 0)
        {
            PlayerPrefs.SetFloat(raceId + "_third", raceTime);

        }

        ringBestTimesText.SetTimes(goldTime, silverTime, goldTime);

        currentBest = PlayerPrefs.GetFloat(raceId + "_best");
        currentSecond = PlayerPrefs.GetFloat(raceId + "_second");
        currentThird = PlayerPrefs.GetFloat(raceId + "_third");

    }


    public void EndRace()
    {
        //Debug.Log("Race ended");
        raceManager.SetRaceActive(false, this);
        raceStarted = false;
        currentTime = 0;
        onRaceEnded?.Invoke();
        StartCoroutine("ResetRace");
    }

 

    public IEnumerator ResetRace()
    {
        yield return new WaitForSeconds(timeBeforeReset);

        ///Debug.Log("Reset race");

        for (int i = 0; i < amountOfRings; i++)
        {
            if (i == 0)
            {
                rings[i].GetComponentInChildren<Ring>().ResetRing(true);

            } else
            {
                rings[i].GetComponentInChildren<Ring>().ResetRing(false);

            }


            if (i != 0)
            {
                rings[i].SetActive(false);

            }
            lastRingPassed = 0;
            nextRingToPass = 1;

        }

        timerText.SetText("");
    }

}
