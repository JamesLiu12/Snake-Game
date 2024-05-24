using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    enum MenuStatus
    {
        MainMenu, Pause, InPlay, Ranking
    }

    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject dieMenu;

    [SerializeField] private Snake snakeScript;

    private MenuStatus m_CurrentStatus = MenuStatus.MainMenu;
    
    void Start()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        dieMenu.SetActive(false);
    }
    
    void Update()
    {
        switch (m_CurrentStatus)
        {
            case MenuStatus.Pause:
                if (Input.GetKeyDown(KeyCode.Escape)) Resume();
                break;
            case MenuStatus.InPlay:
                if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
                break;
            case MenuStatus.Ranking:
                if (Input.GetKeyDown(KeyCode.Escape)) CloseRanking();
                break;
        }
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        snakeScript.StartGame();
        m_CurrentStatus = MenuStatus.InPlay;
    }

    public void ShowRanking()
    {
        mainMenu.SetActive(false);
        m_CurrentStatus = MenuStatus.Ranking;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        m_CurrentStatus = MenuStatus.Pause;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        m_CurrentStatus = MenuStatus.InPlay;
    }

    public void RestartGame()
    {
        BackToMainMenu();
        StartGame();
    }
    
    public void BackToMainMenu()
    {
        snakeScript.InitializeGame();
        pauseMenu.SetActive(false);
        dieMenu.SetActive(false);
        mainMenu.SetActive(true);
        Time.timeScale = 1;
        m_CurrentStatus = MenuStatus.MainMenu;
    }

    public void CloseRanking()
    {
        mainMenu.SetActive(true);
        m_CurrentStatus = MenuStatus.MainMenu;
    }
}
