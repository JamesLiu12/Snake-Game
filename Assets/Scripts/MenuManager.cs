using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    enum MenuStatus
    {
        MainMenu, Pause, InPlay, Ranking, GameEnd, FoodTime
    }

    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject dieMenu;

    [SerializeField] private Snake snakeScript;

    [SerializeField] private GameObject uploadButton;

    [SerializeField] private GameObject successfulText;

    [SerializeField] private GameObject foodTimePanel;

    private MenuStatus m_CurrentStatus = MenuStatus.MainMenu;
    
    void Start()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        dieMenu.SetActive(false);
        foodTimePanel.SetActive(false);
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
            case MenuStatus.FoodTime:
                if (Input.GetKeyDown(KeyCode.Escape)) CloseFoodTimePanel();
                break;
        }
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        snakeScript.StartGame();
        m_CurrentStatus = MenuStatus.InPlay;
        uploadButton.SetActive(true);
        successfulText.SetActive(false);
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

    public void UploadScore()
    {
        uploadButton.SetActive(false);
        successfulText.SetActive(true);
    }

    public void GameEnd()
    {
        m_CurrentStatus = MenuStatus.GameEnd;
    }

    public void OpenFoodTimePanel()
    {
        foodTimePanel.SetActive(true);
        m_CurrentStatus = MenuStatus.FoodTime;
    }

    public void CloseFoodTimePanel()
    {
        foodTimePanel.SetActive(false);
        m_CurrentStatus = MenuStatus.GameEnd;
    }
}
