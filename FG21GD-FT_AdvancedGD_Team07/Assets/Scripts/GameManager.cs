using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private bool gamePaused;
    private PauseMenu pauseMenu;
    private RaceManager raceManager;

    private void Start()
    {
        pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.DeactivatePauseMenu();
        raceManager = FindObjectOfType<RaceManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (gamePaused)
        {
            Time.timeScale = 0;
            pauseMenu.ActivatePauseMenu();
        } else
        {
            Time.timeScale = 1;
            pauseMenu.DeactivatePauseMenu();
        }
        
    }

    public void EndCurrentRace()
    {
        Time.timeScale = 1;
        pauseMenu.DeactivatePauseMenu();
        gamePaused = !gamePaused;
        raceManager.EndCurrentRace();
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
