using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenu;
    private RaceManager raceManager;
    [SerializeField] private GameObject endCurrentRaceButton;

    void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
    }

  

    public void ActivatePauseMenu()
    {

        endCurrentRaceButton.SetActive(raceManager.RaceIsActive());

        pauseMenu.SetActive(true);
    }

    public void DeactivatePauseMenu()
    {
        pauseMenu.SetActive(false);
    }

}
