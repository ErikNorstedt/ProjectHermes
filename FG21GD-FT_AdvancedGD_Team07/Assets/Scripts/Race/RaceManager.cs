using System;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private Race[] races;

    private bool raceActive;
    private Race activeRace;

    public static event Action<Transform, bool> onEndRace;
    public static event Action<Transform> onStartRace;
    private void Start()
    {
        races = new Race[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            races[i] = transform.GetChild(i).GetComponent<Race>();
            races[i].raceId = i;
        }
    }

    public bool RaceIsActive()
    {
        return raceActive;
    }

    public void SetRaceActive(bool value, Race race)
    {
        raceActive = value;
        activeRace = race;

        if (value)
        {
            for (int i = 0; i < races.Length; i++)
            {
                if (races[i] != activeRace)
                {
                    races[i].GetComponent<Race>().hideStart();
                }
            }

            onStartRace?.Invoke(activeRace.resetSpawnPos);

        } 
        else
        {
            for (int i = 0; i < races.Length; i++)
            {
                if (races[i] != activeRace)
                {
                    races[i].GetComponent<Race>().unhideStart();
                }
            }
            activeRace = null;
        }
    }

    public void EndCurrentRace()
    {
        onEndRace?.Invoke(activeRace.resetSpawnPos, false);
        activeRace.EndRace();
        SetRaceActive(false, activeRace);
    }
}