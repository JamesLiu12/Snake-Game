using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    enum MenuStatus
    {
        MainMenu, Pause, InPlay, Ranking, GameEnd, FoodTime, RankingDetail
    }

    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject dieMenu;

    [SerializeField] private Snake snakeScript;
    
    [SerializeField] private GameDataHandler gameDataHandler;

    [SerializeField] private GameObject uploadButton;

    [SerializeField] private GameObject successfulText;

    [SerializeField] private GameObject foodTimePanel;

    [SerializeField] private GameObject rankingPanel;

    [SerializeField] private TextContainer rankTextContainer;
    
    [SerializeField] private TextContainer nameTextContainer;
    
    [SerializeField] private TextContainer scoreTextContainer;

    [SerializeField] private ButtonContainer rankingButtonContainer;

    [SerializeField] private GameObject rankingDetailPanel;

    [SerializeField] private TextContainer rankingDetailTextContainer;

    [SerializeField] private GameObject pauseButton;

    private List<Tuple<int, string, int>> m_PlayerScore = new();    
    private Dictionary<int, List<Tuple<string, string>>> m_PlayerFoodTime = new();

    private MenuStatus m_CurrentStatus = MenuStatus.MainMenu;
    
    void Start()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        dieMenu.SetActive(false);
        foodTimePanel.SetActive(false);
        rankingPanel.SetActive(false);
        rankingDetailPanel.SetActive(false);
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
            case MenuStatus.RankingDetail:
                if (Input.GetKeyDown(KeyCode.Escape)) CloseRankingDetail();
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
        pauseButton.SetActive(true);
    }

    public void ShowRanking()
    {
        RetrieveRanking();
        rankingPanel.SetActive(true);
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
        pauseButton.SetActive(false);
        Time.timeScale = 0;
        m_CurrentStatus = MenuStatus.Pause;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
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
        rankingPanel.SetActive(false);
        m_CurrentStatus = MenuStatus.MainMenu;
    }

    public void UploadScore()
    {
        uploadButton.SetActive(false);

        StartCoroutine(WaitForUpload());
    }

    IEnumerator WaitForUpload()
    {
        float timeout = 3f;
        float timer = 0f;

        while (timer < timeout && gameDataHandler.uploadSuccess != 1 && gameDataHandler.uploadSuccess != 0)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (gameDataHandler.uploadSuccess == 1)
        {
            successfulText.SetActive(true);
        }
        else
        {
            uploadButton.SetActive(true);
        }

        gameDataHandler.uploadSuccess = 2;
    }

    public void GameEnd()
    {
        pauseButton.SetActive(false);
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

    public void OpenRankingDetail(int id)
    {
        DisplayDetailByPlayerID(id);
        rankingDetailPanel.SetActive(true);
        rankingPanel.SetActive(false);
        m_CurrentStatus = MenuStatus.RankingDetail;
    }

    public void CloseRankingDetail()
    {
        rankingDetailPanel.SetActive(false);
        rankingPanel.SetActive(true);
        m_CurrentStatus = MenuStatus.Ranking;
    }

    private void RetrieveRanking()
    {
        gameDataHandler.RetrieveRanking(m_PlayerScore, m_PlayerFoodTime);

        int rank = 1;
        
        rankTextContainer.Clear();
        nameTextContainer.Clear();
        scoreTextContainer.Clear();
        rankingButtonContainer.Clear();
        
        rankTextContainer.AddText("Rank");
        nameTextContainer.AddText("Name");
        scoreTextContainer.AddText("Score");

        foreach (var playerScore in m_PlayerScore)
        {
            rankTextContainer.AddText(rank.ToString());
            nameTextContainer.AddText(playerScore.Item2);
            scoreTextContainer.AddText(playerScore.Item3.ToString());
            rank++;
            
            var newButton = rankingButtonContainer.AddButton("view detail");
            newButton.onClick.AddListener(() => OpenRankingDetail(playerScore.Item1));
        }
    }

    private void DisplayDetailByPlayerID(int id)
    {
        rankingDetailTextContainer.Clear();
        foreach (var playerFoodTime in m_PlayerFoodTime[id])
        {
            rankingDetailTextContainer.AddText(playerFoodTime.Item1 + " " + playerFoodTime.Item2);    
        }
    }
}
