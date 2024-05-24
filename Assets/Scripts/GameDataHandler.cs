using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameDataHandler : MonoBehaviour
{
    private const string k_EatEventUrl = "http://localhost/saveEatEvent.php";
    private const string k_GameOverUrl = "http://localhost/saveGameOver.php";
    
    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerID"))
        {
            string uniqueId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PlayerID", uniqueId);
            PlayerPrefs.Save();
        }
    }

    public void SendEatEvent(string foodName)
    {
        string playerId = PlayerPrefs.GetString("PlayerID");
        StartCoroutine(PostEatEvent(playerId, foodName));
    }

    private IEnumerator PostEatEvent(string playerId, string foodName)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId);
        form.AddField("foodName", foodName);
        form.AddField("eatTime", System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

        using UnityWebRequest www = UnityWebRequest.Post(k_EatEventUrl, form);
        yield return www.SendWebRequest();

        Debug.Log(www.result != UnityWebRequest.Result.Success ? www.error : "Eat event uploaded successfully!");
    }

    public void SendGameOverEvent(string playerName, int score)
    {
        string playerId = PlayerPrefs.GetString("PlayerID");
        StartCoroutine(PostGameOverEvent(playerId, playerName, score));
    }

    private IEnumerator PostGameOverEvent(string playerId, string playerName, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId);
        form.AddField("playerName", playerName);
        form.AddField("score", score);
        form.AddField("gameOverTime", System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

        using UnityWebRequest www = UnityWebRequest.Post(k_GameOverUrl, form);
        yield return www.SendWebRequest();

        Debug.Log(www.result != UnityWebRequest.Result.Success ? www.error : "Game over event uploaded successfully!");
    }
}
